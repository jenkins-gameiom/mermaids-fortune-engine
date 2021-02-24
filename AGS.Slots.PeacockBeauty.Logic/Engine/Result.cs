using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Collections;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;
using Autofac.Features.Indexed;

namespace AGS.Slots.MermaidsFortune.Logic
{
    public class Bet
    {
        public int BetAmount { get; set; }
        public int Chipsperplay { get; set; }
        public bool IsFreeSpin { get; set; }
        public int BonusLevel { get; set; }
        public string Force { get; set; }

        public Bet()
        {
        }
    }
    public class Result
    {

        public List<List<ItemOnReel>> WinningLines { get; set; }
        private List<ItemOnReel> _sequence;
        private List<ItemOnReel> _scatter;
        private List<ItemOnReel> _wildCards;
        private List<ItemOnReel> _mcSymbols;
        private List<ItemOnReel> _nonMcSymbols;

        public long WonAmount { get; set; }

        public JackpotGame JackpotGame { get; set; }

        public List<Win> Wins { get; set; }
        public List<List<int>> Reels { get; set; }

        public Result()
        {
            _sequence = new List<ItemOnReel>();
            _scatter = new List<ItemOnReel>();
            _wildCards = new List<ItemOnReel>();
            _mcSymbols = new List<ItemOnReel>();
            _nonMcSymbols = new List<ItemOnReel>();
            WinningLines = new List<List<ItemOnReel>>();
        }

        public List<ItemOnReel> Sequence
        {
            get
            {
                return _sequence;

            }
        }

        public void ResetSequences()
        {
            _sequence.Clear();
        }


        public void AddCurrentSequence()
        {
            ItemOnReel[] clone = new ItemOnReel[Sequence.Count];
            Sequence.CopyTo(clone);
            WinningLines.Add(new List<ItemOnReel>(clone));
        }

        public string PrintWinningLines()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var line in WinningLines)
            {
                int i = 0;
                foreach (var item in line)
                {
                    sb.Append(string.Format("[{0}]({1})", item.Index, item.Symbol));
                    if (i < line.Count - 1)
                        sb.Append("-");
                    i++;
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        public List<ItemOnReel> Scatter
        {
            get { return _scatter; }
        }
        public List<ItemOnReel> McSymbols
        {
            get { return _mcSymbols; }
        }
        public List<ItemOnReel> WildCards
        {
            get { return _wildCards; }
        }

        public List<ItemOnReel> NonMcSymbols
        {
            get { return _nonMcSymbols; }
        }
        public ItemOnReel LastSequenceItem
        {
            get
            {
                if (_sequence.Count == 0)
                    return null;
                return _sequence[_sequence.Count - 1];

            }
        }

    }

    public enum WinType
    {
        Regular,
        FreeSpin,
        BonusGames,
        JackPot
    }

    public class Win
    {
        public WinType WinType { get; set; }
        public long WinAmount { get; set; }
        public int Ways { get; set; }
        public HashSet<ItemOnReel> WinningLines { get; set; }
        public int GrantedFreeSpins { get; set; }
        public int GrantedWilds { get; set; }
        public List<int> FreeSpinsExpand { get; set; }
        public int Symbol { get; set; }

        [JsonIgnore]
        public int LongestSequence
        {
            get
            {
                return CalcLongestSequence(WinningLines);
            }

        }

        public static int CalcLongestSequence(HashSet<ItemOnReel> itemsOnReels)
        {

            int ret = 0;
            foreach (var item in itemsOnReels)
            {
                if (item.Symbol == MermaidsFortuneResolver.SCATTER)
                    return itemsOnReels.Count;

                ret = System.Math.Max(ret, item.Reel);
            }
            return ret + 1;
        }



    }

    //public class BonusGameService
    //{
    //    private readonly IRequestContext _context;
    //    private readonly IRandom _random;
    //    public long WonAmount { get; set; }
    //    //public int fsLeft = 3;
    //    //public int fsDone = 0;
    //    //public int totalspin = 3;
    //    //private MCSymbol[] allSymbols = new MCSymbol[20];
    //    //List<MCSymbol> lockedSymbols = new List<MCSymbol>();
    //    //public RemoveX2Enum X2IndexesToRemove { get; set; }
    //    //public bool RemoveX8 { get; set; }
    //    //public List<MCSymbol> MCSymbols { get; set; }

    //    public BonusGameService()
    //    {
    //    }

    //    public BonusGameService(IRequestContext context, Configs applicationConfig, IIndex<RandomizerType, IRandom> random)
    //    {
    //        _context = context;
    //        if (applicationConfig.IsTest)
    //            _random = random[RandomizerType.Local];
    //        else
    //            _random = random[RandomizerType.Remote];

    //        //fsDone = res.fsDone;
    //        //fsLeft = res.fsLeft;
    //        //res.JPSymbols.CopyTo(allSymbols, 0);
    //        //X2IndexesToRemove = res.X2IndexesToRemove;
    //        //RemoveX8 = res.RemoveX8;
    //        //lockedSymbols = allSymbols.Where(s => s.IsLocked).ToList();
    //        //totalspin = res.totalSpin;
    //        //SpecialIndex = res.specialIndex;
    //    }

    //    private ItemOnReel[] FindPotentialIndexesToLocateMCsymbols(List<MCSymbol> symbolsList)
    //    {
    //        //TODO - get all indexes that are non mcsymbols
    //        List<ItemOnReel> potentialLocationForSumbols = new List<ItemOnReel>();
    //        for (int reel = 0; reel < 5; reel++)
    //        {
    //            for (int item = 0; item < 4; item++)
    //            {
    //                //var index = (reel + 1 + item * 5);
    //                //if (symbolsList.All(s => s.Item.Index != index))
    //                //{
    //                var reelItem = new ItemOnReel()
    //                {
    //                    Coordinate = new Tuple<int, int>(reel, item),
    //                    Index = (reel + 1 + item * 5),
    //                    Reel = reel,
    //                };
    //                potentialLocationForSumbols.Add(reelItem);
    //            }
    //        }
    //        return potentialLocationForSumbols.ToArray();
    //    }
    //    private int SpecialIndex;
    //    private static int timesOfInstantwin;
    //    private static int timesOfx2;
    //    private static int timesOfx8;
    //    public static int totalForBonus;
    //    public static Dictionary<long, long> bonusDuctionaryBase = new Dictionary<long, long>();
    //    public static Dictionary<long, long> bonusDuctionaryPrize = new Dictionary<long, long>();

    //    public void SpinAll(Bet bet, IRandom random)
    //    {
    //        Dictionary<int, bool> specialPerItem = new Dictionary<int, bool>();
    //        int reelValOfHandleSpecial = 0;
    //        timesOfInstantwin = 0;
    //        timesOfx2 = 0;
    //        timesOfx8 = 0;
    //        int betLevel = _context.Config.stakes.IndexOf(bet.BetAmount);
    //        //Returns all the matrix
    //        var items = FindPotentialIndexesToLocateMCsymbols(_context.State.BonusGame.MCSymbols.ToList());
    //        int jpsym;
    //        _context.State.BonusGame.fsDone++;
    //        //totalspin++;
    //        //bool awarded = false;
    //        //int grandIndex, typeBIndex;
    //        var nonLockedItems = GetNonLockedItems(_context.State.BonusGame.MCSymbolsFromSpin.ToList());
    //        SpecialIndex = GetSpecialIndex(random, nonLockedItems);
    //        //RandomizeWheels(random, MCSymbols, out _context.State.BonusGame.specialIndex);

    //        var listOfRands = GenerateRandomNumbersList(_context.MathFile, betLevel, SpecialIndex);
    //        var randomAfterRequest = random.GetRandomNumbers(listOfRands);
    //        foreach (var item in items)
    //        {
    //            var existingItem = _context.State.BonusGame.MCSymbolsFromSpin.FirstOrDefault(s => s.index == item.Index);
    //            if (existingItem != null)
    //            {
    //                _context.State.BonusGame.MCSymbols[item.Index - 1] =
    //                    new MCSymbol(item.Index, item.Coordinate.Item1, item.Coordinate.Item2, 13, true, TableTypeEnum.Regular);
    //                _context.State.BonusGame.MCSymbols[item.Index - 1].winAmount = existingItem.symbol;
    //                if (!bonusDuctionaryBase.ContainsKey(_context.State.BonusGame.MCSymbols[item.Index - 1].winAmount))
    //                {
    //                    bonusDuctionaryBase.Add(_context.State.BonusGame.MCSymbols[item.Index - 1].winAmount, 1);
    //                }
    //                else
    //                {
    //                    bonusDuctionaryBase[_context.State.BonusGame.MCSymbols[item.Index - 1].winAmount]++;
    //                }
    //            }
    //            else
    //            {
    //                var type = GetType(item.Index);
    //                //jpsym = _config.GetJackpotItem(type, betLevel, random, out bool isJackpot);//TODO remove here and in respin
    //                var val = randomAfterRequest[item.Coordinate.Item2 * 5 + item.Coordinate.Item1].Values[0];

    //                jpsym = _context.MathFile.GetJackpotItemWithoutRandomizing(type, betLevel, val, _context.State.BonusGame.X2IndexesToRemove, _context.State.BonusGame.removeX8, out bool isJackpot);
    //                _context.State.BonusGame.MCSymbols[item.Index - 1] =
    //                    new MCSymbol(item.Index, isJackpot ? 13 : jpsym, item.Coordinate.Item1, item.Coordinate.Item2, false, type);
    //                if (isJackpot)
    //                {
    //                    _context.State.BonusGame.totalSpin++;
    //                    string jpSymbolInString = null;
    //                    //TODO - check if we still need to multiply bet.BetAmount even though we calculate it in GetHoldSpinSymbolValue.
    //                    _context.State.BonusGame.MCSymbols[item.Index - 1].winAmount =
    //                        _context.MathFile.GetHoldSpinSymbolValue(random, bet.BetAmount, type, jpsym, out jpSymbolInString) * bet.Chipsperplay;
    //                    if (_context.State.BonusGame.MCSymbols[item.Index - 1].winAmount == 0 && _context.State.BonusGame.MCSymbols[item.Index - 1].Type == TableTypeEnum.Regular)
    //                    {

    //                    }
    //                    if (type == TableTypeEnum.Special)
    //                    {
    //                        if (jpsym == 14)
    //                        {
    //                            timesOfInstantwin++;
    //                        }
    //                        if (jpsym == 15)
    //                        {
    //                            timesOfx2++;
    //                            reelValOfHandleSpecial = val;
    //                        }
    //                        if (jpsym == 16)
    //                        {
    //                            timesOfx8++;
    //                        }

    //                        specialPerItem[jpsym] = true;
    //                        _context.State.BonusGame.MCSymbols[item.Index - 1].IsLocked = false;
    //                    }
    //                    else
    //                    {
    //                        _context.State.BonusGame.MCSymbols[item.Index - 1].IsLocked = true;
    //                        if (!bonusDuctionaryPrize.ContainsKey(_context.State.BonusGame.MCSymbols[item.Index - 1].winAmount))
    //                        {
    //                            bonusDuctionaryPrize.Add(_context.State.BonusGame.MCSymbols[item.Index - 1].winAmount, 1);
    //                        }
    //                        else
    //                        {
    //                            bonusDuctionaryPrize[_context.State.BonusGame.MCSymbols[item.Index - 1].winAmount] += 1;
    //                        }
    //                    }

    //                    if (_context.State.BonusGame.MCSymbols.Any(x => x != null && x.IsLocked &&
    //                                                                        x.Type == TableTypeEnum.Regular &&
    //                                                                        x.winAmount == 0))
    //                    {

    //                    }
    //                    if (jpSymbolInString != null)
    //                    {
    //                        _context.State.BonusGame.MCSymbols[item.Index - 1].JPSymbolIfString = jpSymbolInString;
    //                    }
    //                }
    //            }
    //            _context.State.BonusGame.MCSymbols[item.Index - 1] = _context.State.BonusGame.MCSymbols[item.Index - 1];
    //            _context.State.BonusGame.MCSymbolsDelta.Add(_context.State.BonusGame.MCSymbols[item.Index - 1]);
    //        }

    //        foreach (var kvp in specialPerItem)
    //        {
    //            if (kvp.Value)
    //            {
    //                HandleSpecial(kvp.Key, reelValOfHandleSpecial);
    //            }
    //        }
    //        _context.State.BonusGame.fsLeft = _context.State.BonusGame.totalSpin - _context.State.BonusGame.fsDone;
    //        _context.State.BonusGame.complete = _context.State.BonusGame.fsLeft == 0;
    //        //note that the sum of all spins for bonusgame will be fsLeft + totalspin. (totalspin means all the spin I ALREADY DID)
    //    }

    //    private void HandleSpecial(int jpsym, int val)
    //    {

    //        if (jpsym == 14)
    //        {
    //        }

    //        if (jpsym == 15)
    //        {
    //            foreach (var mcs in _context.State.BonusGame.MCSymbols)
    //            {
    //                if (mcs != null && mcs.winAmount > 0)
    //                {
    //                    mcs.winAmount *= 2;
    //                }

    //            }
    //            if (_context.State.BonusGame.X2IndexesToRemove == RemoveX2Enum.Both)
    //            {
    //                //Can't happen
    //                throw new Exception("Error, You cannot win X2 more than 2 times!");
    //            }
    //            if (_context.State.BonusGame.X2IndexesToRemove == RemoveX2Enum.First || _context.State.BonusGame.X2IndexesToRemove == RemoveX2Enum.Second)
    //            {
    //                _context.State.BonusGame.X2IndexesToRemove = RemoveX2Enum.Both;
    //            }
    //            if (_context.State.BonusGame.X2IndexesToRemove == RemoveX2Enum.None)
    //            {
    //                //if index of reel is under 30 its the first X2, else its the second. I dont put accurate index cause it can be changed during to 
    //                //x2 and x8 removal
    //                if (val < 30)
    //                {
    //                    _context.State.BonusGame.X2IndexesToRemove = RemoveX2Enum.First;
    //                }
    //                else
    //                {
    //                    _context.State.BonusGame.X2IndexesToRemove = RemoveX2Enum.Second;
    //                }
    //            }
    //        }
    //        if (jpsym == 16)
    //        {
    //            foreach (var mcs in _context.State.BonusGame.MCSymbols)
    //            {
    //                if (mcs != null && mcs.winAmount > 0)
    //                {

    //                    mcs.winAmount *= 8;
    //                }
    //            }
    //            _context.State.BonusGame.removeX8 = true;
    //        }
    //    }

    //    public void ReSpin(Bet bet, IRandom random)
    //    {
    //        Dictionary<int, bool> specialPerItem = new Dictionary<int, bool>();
    //        bool handleSpecial = false;
    //        int reelValOfHandleSpecial = 0;
    //        int betLevel = _context.Config.stakes.IndexOf(bet.BetAmount);
    //        //Returns all the matrix
    //        int jpsym;
    //        _context.State.BonusGame.fsDone++;
    //        SpecialIndex = -1;
    //        var nonLockedItems = GetNonLockedItems(_context.State.BonusGame.MCSymbolsFromSpin.ToList());
    //        SpecialIndex = GetSpecialIndex(random, nonLockedItems);
    //        var listOfRands = GenerateRandomNumbersList(_context.MathFile, betLevel, SpecialIndex);
    //        var randomAfterRequest = random.GetRandomNumbers(listOfRands);
    //        ReassignTypes(_context.State.BonusGame.MCSymbols);
    //        foreach (var item in _context.State.BonusGame.MCSymbols)
    //        {
    //            if (!item.IsLocked)
    //            {
    //                if (_context.State.BonusGame.X2IndexesToRemove == RemoveX2Enum.Both)
    //                {

    //                }
    //                //jpsym = _config.GetJackpotItem(type, betLevel, random, out bool isJackpot);//TODO remove here and in respin
    //                var val = randomAfterRequest[item.coordinate.Item2 * 5 + item.coordinate.Item1].Values[0];
    //                jpsym = _context.MathFile.GetJackpotItemWithoutRandomizing(item.Type, betLevel, val, _context.State.BonusGame.X2IndexesToRemove, _context.State.BonusGame.removeX8, out bool isJackpot);
    //                _context.State.BonusGame.MCSymbols[item.index - 1] =
    //                    new MCSymbol(item.index, item.coordinate.Item1, item.coordinate.Item2, isJackpot ? 13 : jpsym, false, item.Type);

    //                if (isJackpot)
    //                {
    //                    _context.State.BonusGame.totalSpin++;
    //                    string jpSymbolInString = null;
    //                    //TODO - check if we still need to multiply bet.BetAmount even though we calculate it in GetHoldSpinSymbolValue.
    //                    _context.State.BonusGame.MCSymbols[item.index- 1].winAmount =
    //                        _context.MathFile.GetHoldSpinSymbolValue(random, bet.BetAmount, item.Type, jpsym,
    //                            out jpSymbolInString) * bet.Chipsperplay;
    //                    if (item.Type == TableTypeEnum.Special)
    //                    {

    //                        if (jpsym == 14)
    //                        {
    //                            timesOfInstantwin++;
    //                        }

    //                        if (jpsym == 15)
    //                        {
    //                            timesOfx2++;
    //                            reelValOfHandleSpecial = val;
    //                        }

    //                        if (jpsym == 16)
    //                        {
    //                            timesOfx8++;
    //                        }
    //                        specialPerItem[jpsym] = true;
    //                        _context.State.BonusGame.MCSymbols[item.index - 1].IsLocked = false;
    //                    }
    //                    else
    //                    {
    //                        _context.State.BonusGame.MCSymbols[item.index - 1].IsLocked = true;
    //                        _context.State.BonusGame.MCSymbols[item.index - 1].IsLocked = true;
    //                        if (!bonusDuctionaryPrize.ContainsKey(_context.State.BonusGame.MCSymbols[item.index - 1].winAmount))
    //                        {
    //                            bonusDuctionaryPrize.Add(_context.State.BonusGame.MCSymbols[item.index - 1].winAmount, 1);
    //                        }
    //                        else
    //                        {
    //                            bonusDuctionaryPrize[_context.State.BonusGame.MCSymbols[item.index - 1].winAmount] += 1;
    //                        }
    //                    }
    //                    if (jpSymbolInString != null)
    //                    {
    //                        _context.State.BonusGame.MCSymbols[item.index - 1].JPSymbolIfString = jpSymbolInString;
    //                    }
    //                }
    //            }
    //        }
    //        foreach (var kvp in specialPerItem)
    //        {
    //            if (kvp.Value)
    //            {
    //                HandleSpecial(kvp.Key, reelValOfHandleSpecial);
    //            }
    //        }
    //        _context.State.BonusGame.fsLeft = _context.State.BonusGame.totalSpin - _context.State.BonusGame.fsDone;
    //        _context.State.BonusGame.complete = _context.State.BonusGame.fsLeft == 0;
    //        //note that the sum of all spins for bonusgame will be fsLeft + totalspin. (totalspin means all the spin I ALREADY DID)
    //    }

    //    private void ReassignTypes(MCSymbol[] mcSymbols)
    //    {
    //        foreach (var s in _context.State.BonusGame.MCSymbols)
    //        {
    //            if (s.Type == TableTypeEnum.Special)
    //            {
    //                if (s.JPSymbolIfString != null)
    //                {

    //                }
    //                s.JPSymbolIfString = null;
    //            }
    //            s.Type = TableTypeEnum.Regular;
    //        }
    //        foreach (var s in _context.State.BonusGame.MCSymbols)
    //        {
    //            if (s.index == SpecialIndex)
    //            {
    //                s.Type = TableTypeEnum.Special;
    //            }
    //        }

    //    }

    //    private TableTypeEnum GetType(int index)
    //    {
    //        TableTypeEnum type;
    //        if (index == SpecialIndex)
    //        {
    //            type = TableTypeEnum.Special;
    //        }
    //        else
    //        {
    //            type = TableTypeEnum.Regular;
    //        }
    //        return type;
    //    }

    //    private List<RandomNumber> GenerateRandomNumbersList(IMathFile _config, int betLevel, int prizeIndex)
    //    {
    //        var listOfRands = new List<RandomNumber>();
    //        for (int reelitem = 0; reelitem < 4; reelitem++)
    //        {
    //            //TODO - check this index doesn't already have 6 or more mc from the spin that came before the spinall.
    //            //TODO - this should happen 9 or less times, and not 15.
    //            for (int reel = 0; reel < 5; reel++)
    //            {

    //                var itemIndex = (reel + 1 + reelitem * 5);
    //                var max = 0;
    //                if (itemIndex == prizeIndex)
    //                {
    //                    max = _config.GetSpecialReelStrip(_context.State.BonusGame.X2IndexesToRemove, _context.State.BonusGame.removeX8).Count;
    //                }
    //                else
    //                {
    //                    max = _config.GetPrizeReelStrip().Count;
    //                }
    //                var rand = new RandomNumber
    //                {
    //                    Min = 0,
    //                    Max = max,
    //                    Quantity = 1
    //                };
    //                listOfRands.Add(rand);
    //            }
    //        }



    //        return listOfRands;
    //    }

    //    private List<int> GetNonLockedItems(List<Common.Entities.MCSymbol> lockedSymbolsFromSpin)
    //    {
    //        var nonLockedSymbolsFromSpin = new List<int>();
    //        for (int i = 1; i <= 20; i++)
    //        {
    //            if (!lockedSymbolsFromSpin.Any(x => x.index == i))
    //            {
    //                nonLockedSymbolsFromSpin.Add(i);
    //            }
    //        }

    //        return nonLockedSymbolsFromSpin;

    //    }
    //    private int GetSpecialIndex(IRandom random, List<int> nonLockedIndexes)
    //    {
    //        var specialIndex = random.Next(1, nonLockedIndexes.Count + 1);
    //        return nonLockedIndexes[specialIndex - 1];
    //    }
    //    //private void RandomizeWheels(IRandom random, List<MCSymbol> symbolsList, out int specialIndex)
    //    //{
    //    //    if (symbolsList.Any(x => x.IsLocked))
    //    //    {
    //    //
    //    //    }
    //    //    specialIndex = -1;
    //    //    int special = specialIndex;
    //    //    //TODO - check if the indexes are from 0 to 20
    //    //    var itemsInTable = 20;
    //    //    do
    //    //    {
    //    //
    //    //        specialIndex = random.Next(1, itemsInTable + 1);
    //    //        special = specialIndex;
    //    //    }
    //    //    while (symbolsList.Any(mc => mc.Item.Index == special));
    //    //
    //    //    if (symbolsList.All(x => x.Item.Index != special))
    //    //    {
    //    //        var xxx = symbolsList.Any(x => x.IsLocked && x.Item.Index == special);
    //    //    }
    //    //    if (symbolsList.Any(x => !x.IsLocked))
    //    //    {
    //    //        var xxx = symbolsList.Any(x => x.IsLocked && x.Item.Index == special);
    //    //    }
    //    //
    //    //}
    //    public void Resolve(Bet bet, IMathFile _config)
    //    {
    //        int betLevel = _config.BetSteps.IndexOf(bet.BetAmount);
    //        if (_context.State.BonusGame.fsLeft == 0 || _context.State.BonusGame.MCSymbols.All(s => s.IsLocked) || _context.State.BonusGame.MCSymbols.Any(s => s.JPSymbolIfString == "paid"))
    //        {
    //            for (int reel = 0; reel < 5; reel++)
    //            {
    //                //bool jp = true;
    //                for (int reelitem = 0; reelitem < 4; reelitem++)
    //                {
    //                    var index = (reel + 1 + reelitem * 5);
    //                    WonAmount += _context.State.BonusGame.MCSymbols[index - 1].winAmount;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            throw new Exception("Cannot Resolve unfinished Bonus game");
    //        }
    //    }
    //}
}

