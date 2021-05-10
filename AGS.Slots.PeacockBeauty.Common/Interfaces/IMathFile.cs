using AGS.Slots.MermaidsFortune.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using AGS.Slots.MermaidsFortune.Common.Entities;

namespace AGS.Slots.MermaidsFortune.Common.Interfaces
{
    public interface IMathFile
    {
        SpinBagResult GetFullReels(IRequestContext _context, List<int> chosenIndexes);
        SpinBagResult GetReels(IRequestContext _context ,IRandom random);
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
        int MoneyChargeSymbol(bool isFreeSpin, int reelSet, IRandom random);

        List<int> JackpotTableValues { get; }

        List<int> GetProgressiveInformation();

        string GetProgressiveValueFromNumber(int number);


        void AssignReelSet(IRequestContext context, IRandom random);
    }
}
