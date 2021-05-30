using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;

namespace TestSlotsConsole
{
    public class Statistics
    {
        public Collector Collector { get; set; }
        public int Spins { get; set; }
        public long TotalBetAmount { get; set; }
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
        public long TotalGrandMoneyWonAmount { get; set; }
        public long TotalMajorMoneyWonAmount { get; set; }
        public long TotalMinorMoneyWonAmount { get; set; }
        public long TotalNumberMoneyWonAmount { get; set; }
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
        Spin, FSpin
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
                case ResponseType.FSpin:
                    stats.Collector.TotalFreeSpinsMoneyWonAmount += totalWinAmount;
                    break;
                default:
                    break;
            }
        }

        public string CalculateRTP(Statistics stats)
        {
            long totalCredit = stats.Collector.TotalWinMoneyAmount;
            long totalDebit = stats.TotalBetAmount;


            string ret = string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}",
                string.Format("**Total RTP**: {0}", (totalCredit / (double)totalDebit) * 100),
                string.Format("Base Contribute: {0}", (stats.Collector.TotalRegularSpinsMoneyWonAmount / (double)totalDebit) * 100),
                string.Format("FS Contribute: {0}", (stats.Collector.TotalFreeSpinsMoneyWonAmount / (double)totalDebit) * 100),
                string.Format("Bonus Contribute: {0}", (stats.Collector.TotalBonusMoneyWonAmount / (double)totalDebit) * 100),
                string.Format("Jackpot Contribute: {0}", (stats.Collector.TotalJackpotMoneyWonAmount / (double)totalDebit) * 100),
                string.Format("**Total Credit**: {0}", (totalCredit)),
                string.Format("**Total Debit**: {0}", ((double)totalDebit)),
                string.Format("Spins: {0}", stats.Spins),
                string.Format("Errors: {0}", stats.Errors + "\n\n\n\n\n\n")
                 );
            string parts = null;
            string regular = "Regular - ";
            string fs = "FS - ";
            var regularregular = "regular regular - " + (MermaidsFortuneResolver.parts[regular + "regular"] / (double)totalDebit) * 100 + "\n";
            var regularbn = "regular bn - " + (MermaidsFortuneResolver.parts[regular + "bn"] / (double)totalDebit) * 100 + "\n";
            var regularfive = "regular five - " + (MermaidsFortuneResolver.parts[regular + "5ofakind"] / (double)totalDebit) * 100 + "\n";
            var regularfour = "regular four - " + (MermaidsFortuneResolver.parts[regular + "4ofakind"] / (double)totalDebit) * 100 + "\n";
            var regularthree = "regular three - " + (MermaidsFortuneResolver.parts[regular + "3ofakind"] / (double)totalDebit) * 100 + "\n";
            var regulartotal = "regular total - " + ((MermaidsFortuneResolver.parts[regular + "bn"] +
                                                     MermaidsFortuneResolver.parts[regular + "regular"] +
                                                     MermaidsFortuneResolver.parts[regular + "5ofakind"] +
                                                     MermaidsFortuneResolver.parts[regular + "4ofakind"] +
                                                     MermaidsFortuneResolver.parts[regular + "3ofakind"]) / (double)totalDebit) * 100 + "\n\n\n";
            var fsregular = "fs regular - " + (MermaidsFortuneResolver.parts[fs + "regular"] / (double)totalDebit) * 100 + "\n";
            var fsbn = "fs bn - " + (MermaidsFortuneResolver.parts[fs + "bn"] / (double)totalDebit) * 100 + "\n";
            var fsfive = "fs five - " + (MermaidsFortuneResolver.parts[fs + "5ofakind"] / (double)totalDebit) * 100 + "\n";
            var fsfour = "fs four - " + (MermaidsFortuneResolver.parts[fs + "4ofakind"] / (double)totalDebit) * 100 + "\n";
            var fsthree = "fs three - " + (MermaidsFortuneResolver.parts[fs + "3ofakind"] / (double)totalDebit) * 100 + "\n";
            var fstotal = "fs total - " + ((MermaidsFortuneResolver.parts[fs + "bn"] +
                                             MermaidsFortuneResolver.parts[fs + "regular"] +
                                             MermaidsFortuneResolver.parts[fs + "5ofakind"] +
                                             MermaidsFortuneResolver.parts[fs + "4ofakind"] +
                                             MermaidsFortuneResolver.parts[fs + "3ofakind"]) / (double)totalDebit) * 100 + "\n\n\n";
            var totalregular = "total regular - " + ((MermaidsFortuneResolver.parts[regular + "regular"] + MermaidsFortuneResolver.parts[fs + "regular"]) / (double)totalDebit) * 100 + "\n";
            var totalbn = "total bn - " + ((MermaidsFortuneResolver.parts[regular + "bn"] + MermaidsFortuneResolver.parts[fs + "bn"]) / (double)totalDebit) * 100 + "\n";
            var totalfive = "total five - " + ((MermaidsFortuneResolver.parts[regular + "5ofakind"] + MermaidsFortuneResolver.parts[fs + "5ofakind"]) / (double)totalDebit) * 100 + "\n";
            var totalfour = "total four - " + ((MermaidsFortuneResolver.parts[regular + "4ofakind"] + MermaidsFortuneResolver.parts[fs + "4ofakind"]) / (double)totalDebit) * 100 + "\n";
            var totalthree = "total three - " + ((MermaidsFortuneResolver.parts[regular + "4ofakind"] + MermaidsFortuneResolver.parts[fs + "3ofakind"]) / (double)totalDebit) * 100 + "\n";
            var totaltotal = "total total - " + ((MermaidsFortuneResolver.parts[regular + "bn"] + MermaidsFortuneResolver.parts[fs + "bn"] +
                                                  MermaidsFortuneResolver.parts[regular + "regular"] + MermaidsFortuneResolver.parts[fs + "regular"] +
                                                  MermaidsFortuneResolver.parts[regular + "5ofakind"] + MermaidsFortuneResolver.parts[fs + "5ofakind"] +
                                                  MermaidsFortuneResolver.parts[regular + "4ofakind"] + MermaidsFortuneResolver.parts[fs + "4ofakind"] +
                                                  MermaidsFortuneResolver.parts[regular + "3ofakind"] + MermaidsFortuneResolver.parts[fs + "3ofakind"])
                / (double)totalDebit) * 100 + "\n\n\n";
            
            parts += regularregular + regularbn + regularfive + regularfour + regularthree + regulartotal;
            parts += fsregular + fsbn + fsfive + fsfour + fsthree + fstotal;
            parts += totalregular + totalbn + totalfive + totalfour + totalthree + totaltotal;
            parts += "FS Break Down:\n";
            parts += "Respin 01000  First- " + MermaidsFortuneResolver.RespinBreakDown01000 / (double)totalDebit * 100 + "\n";
            parts += "Respin 00010  Second - " + MermaidsFortuneResolver.RespinBreakDown00010 / (double)totalDebit * 100 + "\n";
            parts += "Respin 01010 - Both " + MermaidsFortuneResolver.RespinBreakDown01010 / (double)totalDebit * 100 + "\n";
            parts += "No respin " + MermaidsFortuneResolver.RespinBreakDownNoRespin / (double)totalDebit * 100 + "\n";
            parts += "All respin sum = " + (MermaidsFortuneResolver.RespinBreakDown01000 / (double) totalDebit +
                                            MermaidsFortuneResolver.RespinBreakDown00010 / (double) totalDebit +
                                            MermaidsFortuneResolver.RespinBreakDown01010 / (double) totalDebit +
                                            MermaidsFortuneResolver.RespinBreakDownNoRespin / (double) totalDebit) * 100 + "\n";
            parts += "FG_BONUS_WEIGHTS_1:" + "\n";
            float z = (float)1731 / (float)MermaidsFortuneResolver.fsMCSymbolsWeightsRS1["100"];
            var y = MermaidsFortuneResolver.fsMCSymbolsWeightsRS1.Select(x => new { ArgKey = int.Parse(x.Key), ArgValue = x.Value * z }).OrderBy(x => x.ArgKey).ToDictionary(x => x.ArgKey);
            foreach (var xx in y)
            {
                parts += xx.Key + " : " + xx.Value.ArgValue + "\n";
            }
            parts += "FG_BONUS_WEIGHTS_2:" + "\n";
            z = (float)1200 / (float)MermaidsFortuneResolver.fsMCSymbolsWeightsRS2["100"];
            y = MermaidsFortuneResolver.fsMCSymbolsWeightsRS2.Select(x => new { ArgKey = int.Parse(x.Key), ArgValue = x.Value * z }).OrderBy(x => x.ArgKey).ToDictionary(x => x.ArgKey);
            foreach (var xx in y)
            {
                parts += xx.Key + " : " + xx.Value.ArgValue + "\n";
            }
            parts += "FG_BONUS_WEIGHTS_3:" + "\n";
            z = (float)200 / (float)MermaidsFortuneResolver.fsMCSymbolsWeightsRS3["100"];
            y = MermaidsFortuneResolver.fsMCSymbolsWeightsRS3.Select(x => new { ArgKey = int.Parse(x.Key), ArgValue = x.Value * z }).OrderBy(x => x.ArgKey).ToDictionary(x => x.ArgKey);
            foreach (var xx in y)
            {
                parts += xx.Key + " : " + xx.Value.ArgValue + "\n";
            }
            ret += parts;
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
