using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;

namespace TestSlotsConsole
{
    public class Statistics
    {
        public Collector Collector { get; set; }
        public int Spins { get; set; }
        public int TotalBetAmount { get; set; }
        public int Errors { get; set; }
        public string LastErrorMessage { get; set; }
        public Statistics()
        {
            Collector = new Collector();
        }
    }

    public class Collector
    {
        public long TotalBonusMoneyWonAmount { get; set; }
        public long TotalJackpotMoneyWonAmount { get; set; }
        public long TotalRegularSpinsMoneyWonAmount { get; set; }
        public long TotalFreeSpinsMoneyWonAmount { get; set; }
        public long TotalBetMoneyAmount { get; set; }
        public long TotalWinMoneyAmount
        {
            get
            {
                return TotalBonusMoneyWonAmount + TotalJackpotMoneyWonAmount + TotalRegularSpinsMoneyWonAmount + TotalFreeSpinsMoneyWonAmount;
            }
        }

        public int TotalJackpotWinTimes { get; set; }
        public int TotalBonusGameWinTimes { get; set; }
        public int TotalWinTimesInFS { get; set; }
        public int TotalWinTimesInBase { get; set; }
        public int AmountOfFreeSpinsWon { get; set; }
    }

    public enum ResponseType
    {
        Spin, Pick, FSpin, Jackpot
    }
    public class SimulatorLogic
    {

        public void Collect(Statistics stats, ResponseType responseType, int totalWinAmount)
        {
            switch (responseType)
            {
                case ResponseType.Spin:
                    stats.Collector.TotalRegularSpinsMoneyWonAmount += totalWinAmount;
                    break;
                case ResponseType.Pick:
                    stats.Collector.TotalBonusMoneyWonAmount += totalWinAmount;
                    break;
                case ResponseType.FSpin:
                    stats.Collector.TotalFreeSpinsMoneyWonAmount += totalWinAmount;
                    break;
                case ResponseType.Jackpot:
                    stats.Collector.TotalJackpotMoneyWonAmount += totalWinAmount;
                    break;
                default:
                    break;
            }
        }

        public string CalculateRTP(Statistics stats)
        {
            long totalCredit = stats.Collector.TotalWinMoneyAmount;
            long totalDebit = stats.TotalBetAmount;


            string ret = string.Format("{4}\n{0}\n{1}\n{2}\n{3}\n{5}\n{6}",
                string.Format("Base Contribute: {0}", (stats.Collector.TotalRegularSpinsMoneyWonAmount / (double)totalDebit) * 100),
                string.Format("FS Contribute: {0}", (stats.Collector.TotalFreeSpinsMoneyWonAmount / (double)totalDebit) * 100),
                string.Format("Bonus Contribute: {0}", (stats.Collector.TotalBonusMoneyWonAmount / (double)totalDebit) * 100),
                string.Format("Jackpot Contribute: {0}", (stats.Collector.TotalJackpotMoneyWonAmount / (double)totalDebit) * 100),
                 string.Format("**Total RTP**: {0}", (totalCredit / (double)totalDebit) * 100),
                string.Format("Spins: {0}", stats.Spins),
                string.Format("Errors: {0}", stats.Errors)
                 );

            return ret;

        }

        public bool CheckIfBonusGame(dynamic response)
        {
            if (response.publicState.spin.wins.Count > 0)
            {
                Win[] wins = Json.ConvertDynamic<Win[]>(response.publicState.spin.wins);
                var firstWinFs = wins.Where(w => w.featureType == "pick");
                if (firstWinFs.Count() > 0)
                    return true;
            }
            return false;
        }

        public void AddSpin(Statistics stats)
        {
            stats.Spins++;
        }

        public void AddTotalBetAmount(Statistics stats, int totalBetAmount)
        {
            stats.TotalBetAmount += totalBetAmount;
        }

        private long? GetDebits(dynamic response)
        {
            if (response.transactions != null)
            {
                if (response.transactions.debits != null)
                {
                    return Convert.ToInt64(response.transactions.debits[0]);
                }
            }
            return null;
        }

        private long? GetCredits(dynamic response)
        {
            if (response.transactions != null)
            {
                if (response.transactions.credits != null)
                {
                    return Convert.ToInt64(response.transactions.credits[0]);
                }
            }
            return null;
        }

        public int GetRemainingFS(dynamic newFsRes)
        {
            return Json.ConvertDynamic<int>(newFsRes.publicState.spin.freeSpinsLeft);
        }

        public void setPrivateStateFromPrevResponse(dynamic previousResponse, dynamic newRequest)
        {
            newRequest.privateState = previousResponse.privateState;
            //newRequest.Config = previousResponse.Config;
        }

        public void ResetTransactions(dynamic response)
        {
            if (response != null && response.transactions != null)
                response.transactions = null;
        }

        public int? CheckIfNeedToChangeToFs(dynamic response)
        {
            var fsleft = Convert.ToInt32(response.privateState.freeSpinsLeft);
            if (fsleft > 0)
                return fsleft;
            return null;
        }

        public string ShowLastError(Statistics stat)
        {
            return stat.LastErrorMessage;
        }

        public void AddError(Statistics stat, string error)
        {
            stat.Errors++;
            stat.LastErrorMessage = error;
        }
    }
}
