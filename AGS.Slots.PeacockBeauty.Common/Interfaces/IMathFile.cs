using AGS.Slots.MermaidsFortune.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGS.Slots.MermaidsFortune.Common.Interfaces
{
    public interface IMathFile
    {
        List<List<int>> GetReels();
        int BetStepsDevider { get; }
        List<List<int>> GetLookupPaytable();

        List<int> BetSteps { get; }

        List<int> Denoms { get; }



        /// <summary>
        /// 0 for mc
        /// 1 for fs
        /// </summary>
        /// <param name="selectedNumber"></param>
        /// <returns></returns>
        //int ResolveBonusType(int selectedNumber);
        //gold_symbols_machine_bonus_game_jackpot_table
        string MoneyChargeSymbol(int betLevel, int valueToFind, IRandom random = null);

        List<int> JackpotTableValues { get; }

        List<int> GetProgressiveInformation();

        string GetProgressiveValueFromNumber(int number);

        int GetBasePrize(int betLevel, IRandom random);
        int GetPrizeSpinPrize(int betLevel, IRandom random);

        List<int> GetPrizeReelStrip();
        List<int> GetSpecialReelStrip(RemoveX2Enum removeX2, bool removeX8);

        int GetJackpotItemWithoutRandomizing(TableTypeEnum type, int valueToFind, RemoveX2Enum removeX2, bool removeX8,  out bool isJackpot, string force = null);
        int GetHoldSpinSymbolValue(int betAmount, TableTypeEnum type, int symbol, out string jackpotSymbol, IRandom random);

        bool WildTriggerJackpot(bool isFreeSpin, int betLevel, IRandom random = null);
        string GetJackpotBonusOutcome(IRandom random);
        List<string> GetRandomJackpotCombination(string outcome, IRandom random);

        void GetCurrentLongTermPersistence(IStateItems stateItems);
    }
}
