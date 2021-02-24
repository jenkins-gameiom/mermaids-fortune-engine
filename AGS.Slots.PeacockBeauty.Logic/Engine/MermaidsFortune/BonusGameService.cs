using AGS.Slots.MermaidsFortune.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Enums;
using Autofac.Features.Indexed;

namespace AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune
{
    public class BonusGameService : IBonusGameService
    {
        private readonly IRequestContext _context;
        private readonly Configs _applicationConfig;
        private readonly IRandom _random;

        public BonusGameService(IRequestContext context, Configs applicationConfig, IIndex<RandomizerType, IRandom> random)
        {
            _context = context;
            if (applicationConfig.IsTest)
                _random = random[RandomizerType.Local];
            else
                _random = random[RandomizerType.Remote];
        }
        public BonusGameService(IRequestContext context, Configs applicationConfig, IRandom iRandom)
        {
            _context = context;
            _random = iRandom;
        }

        public static Dictionary<long, long> bonusDuctionaryBase = new Dictionary<long, long>();
        public static Dictionary<long, long> bonusDuctionaryPrize = new Dictionary<long, long>();
        public void SpinAll()
        {
            if (_context.State.BonusGame.totalSpin == 0)
            {
                //foreach (var x in _context.State.BonusGame.MCSymbols.Where(s => s.IsLocked))
                //{
                //    if (!bonusDuctionaryBase.ContainsKey(x.winAmount))
                //    {
                //        bonusDuctionaryBase.Add(x.winAmount, 1);
                //    }
                //    else
                //    {
                //        bonusDuctionaryBase[x.winAmount]++;
                //    }
                //}
                //if (bonusDuctionaryBase.ContainsKey(888) && bonusDuctionaryBase[888] == 3000)
                //{
                //
                //}
                InitBonusGame();
            }
            int reelValOfHandleSpecial = 0;
            //InitMCSymbols();
            //var items = FindPotentialIndexesToLocateMCsymbols();
            _context.State.BonusGame.fsDone++;
            int jpsym;
            var specialDictionary = GetSpecialPerItemDictionay();
            var nonLockedItems = GetNonLockedItems();
            _context.State.BonusGame.specialIndex = GetSpecialIndex(nonLockedItems.Select(s => s.index).ToList());
            var listOfRands = GenerateRandomNumbersList();
            var randomAfterRequest = _random.GetRandomNumbers(listOfRands);
            ReassignTypes();
            foreach (var item in nonLockedItems)
            {
                var myItem = _context.State.BonusGame.MCSymbols.First(x => x.index == item.index);
                
                var type = GetType(myItem.index);
                var val = randomAfterRequest[myItem.index-1].Values[0];
                string force = _context.RequestItems.force == null ? null : _context.RequestItems.force.ToString();
                jpsym = _context.MathFile.GetJackpotItemWithoutRandomizing(type, val, _context.State.BonusGame.X2IndexesToRemove, _context.State.BonusGame.removeX8,  out bool isJackpot, force);
                myItem.symbol = jpsym;
                
                if (isJackpot)
                {
                    _context.State.BonusGame.totalSpin++;
                    //TODO - check if we still need to multiply bet.BetAmount even though we calculate it in GetHoldSpinSymbolValue.
                    myItem.winAmount =
                        _context.MathFile.GetHoldSpinSymbolValue(_context.GetBetAmount(), type, jpsym, out var jpSymbolInString, _random) * _context.GetDenom();
                    if (type == TableTypeEnum.Special)
                    {
                        if (jpsym == 15)
                        {
                            reelValOfHandleSpecial = val;
                        }
                        myItem.IsLocked = false;
                        specialDictionary[jpsym] = true;
                    }
                    else
                    {
                        myItem.IsLocked = true;
                    }
                    if (jpSymbolInString != null)
                    {
                        myItem.JPSymbolIfString = jpSymbolInString;
                    }
                }
            }
            //we do HandleSpecial in the end cause if there is 
            foreach (var kvp in specialDictionary)
            {
                if (kvp.Value)
                {
                    HandleSpecial(kvp.Key, reelValOfHandleSpecial);
                }
            }
            _context.State.BonusGame.fsLeft = _context.State.BonusGame.totalSpin - _context.State.BonusGame.fsDone;
            _context.State.BonusGame.complete = _context.State.BonusGame.fsLeft == 0;
            _context.State.completed = _context.State.BonusGame.complete;
            if (_context.State.BonusGame.MCSymbols.Any(s => s.symbol == -1))
            {

            }
            if (_context.State.BonusGame.complete || _context.State.BonusGame.MCSymbols.Any(s => s.JPSymbolIfString == "paid"))
            {
                Resolve();
            }
        }

        public long GetCashWon()
        {
            var cashWon = _context.State.BonusGame.winAmount;
            return cashWon;
        }

        public void EndBonus()
        {
            _context.State.BonusGame = null;
        }

        private void InitBonusGame()
        {
            _context.State.BonusGame.totalSpin = 5;
            _context.State.BonusGame.fsDone = 0;
            _context.State.BonusGame.fsLeft = 5;
            InitMCSymbols();
        }

        private void InitMCSymbols()
        {
            if (_context.State.BonusGame.MCSymbols.Count < 20)
            {
                for (int reel = 0; reel < 5; reel++)
                {
                    for (int item = 0; item < 4; item++)
                    {

                        var reelItem = new ItemOnReel()
                        {
                            Coordinate = new Tuple<int, int>(reel, item),
                            Index = (reel + 1 + item * 5),
                            Reel = reel,
                        };
                        if (!_context.State.BonusGame.MCSymbols.Any(s => s.index == reelItem.Index))
                        {
                            _context.State.BonusGame.MCSymbols.Add(new MCSymbol(reelItem.Index, reelItem.Reel, item, -1, false, TableTypeEnum.Regular));
                        }
                    }
                }
            }
        }

        private void HandleSpecial(int jpsym, int val)
        {

            if (jpsym == 14)
            {
                //Resolve();
            }

            if (jpsym == 15)
            {
                foreach (var x in _context.State.BonusGame.MCSymbols)
                {
                    if (x != null && x.winAmount > 0)
                    {
                        x.winAmount *= 2;
                    }

                }
                if (_context.State.BonusGame.X2IndexesToRemove == RemoveX2Enum.Both)
                {
                    //Can't happen
                    throw new Exception("Error, You cannot win X2 more than 2 times!");
                }
                if (_context.State.BonusGame.X2IndexesToRemove == RemoveX2Enum.First || _context.State.BonusGame.X2IndexesToRemove == RemoveX2Enum.Second)
                {
                    _context.State.BonusGame.X2IndexesToRemove = RemoveX2Enum.Both;
                }
                if (_context.State.BonusGame.X2IndexesToRemove == RemoveX2Enum.None)
                {
                    //if index of reel is under 30 its the first X2, else its the second. I dont put accurate index cause it can be changed during to 
                    //x2 and x8 removal
                    if (val < 30)
                    {
                        _context.State.BonusGame.X2IndexesToRemove = RemoveX2Enum.First;
                    }
                    else
                    {
                        _context.State.BonusGame.X2IndexesToRemove = RemoveX2Enum.Second;
                    }
                }
            }
            if (jpsym == 16)
            {
                foreach (var x in _context.State.BonusGame.MCSymbols)
                {
                    if (x != null && x.winAmount > 0)
                    {

                        x.winAmount *= 8;
                    }
                }
                _context.State.BonusGame.removeX8 = true;
            }
        }


        private Dictionary<int, bool> GetSpecialPerItemDictionay()
        {
            var specialPerItem = new Dictionary<int, bool>();
            specialPerItem.Add(14, false);
            specialPerItem.Add(15, false);
            specialPerItem.Add(16, false);
            return specialPerItem;
        }
        private ItemOnReel[] FindPotentialIndexesToLocateMCsymbols()
        {
            //TODO - get all indexes that are non mcsymbols
            List<ItemOnReel> potentialLocationForSumbols = new List<ItemOnReel>();
            for (int reel = 0; reel < 5; reel++)
            {
                for (int item = 0; item < 4; item++)
                {
                    //var index = (reel + 1 + item * 5);
                    //if (symbolsList.All(s => s.Item.Index != index))
                    //{
                    var reelItem = new ItemOnReel()
                    {
                        Coordinate = new Tuple<int, int>(reel, item),
                        Index = (reel + 1 + item * 5),
                        Reel = reel,
                    };
                    var x = _context.State.BonusGame.MCSymbols.FirstOrDefault(y => y.index == reelItem.Index);
                    if (x != null)
                    {
                        potentialLocationForSumbols.Add(reelItem);
                    }
                }
            }
            return potentialLocationForSumbols.ToArray();
        }

        private List<MCSymbol> GetNonLockedItems()
        {
            return _context.State.BonusGame.MCSymbols.Where(s => !s.IsLocked).ToList();
        }

        private List<MCSymbol> GetLockedItems()
        {
            return _context.State.BonusGame.MCSymbols.Where(s => s.IsLocked).ToList();
        }

        private void ReassignTypes()
        {
            foreach (var s in _context.State.BonusGame.MCSymbols)
            {
                if (s.Type == TableTypeEnum.Special)
                {
                    s.JPSymbolIfString = null;
                }
                s.Type = TableTypeEnum.Regular;
            }
            _context.State.BonusGame.MCSymbols.First(s => s.index == _context.State.BonusGame.specialIndex).Type = TableTypeEnum.Special;
        }

        private TableTypeEnum GetType(int index)
        {
            TableTypeEnum type;
            if (index == _context.State.BonusGame.specialIndex)
            {
                type = TableTypeEnum.Special;
            }
            else
            {
                type = TableTypeEnum.Regular;
            }
            return type;
        }

        private  List<RandomNumber> GenerateRandomNumbersList()
        {
            var listOfRands = new List<RandomNumber>();
            for (int reelitem = 0; reelitem < 4; reelitem++)
            {
                //TODO - check this index doesn't already have 6 or more mc from the spin that came before the spinall.
                //TODO - this should happen 9 or less times, and not 15.
                for (int reel = 0; reel < 5; reel++)
                {

                    var itemIndex = (reel + 1 + reelitem * 5);
                    var max = 0;
                    if (itemIndex == _context.State.BonusGame.specialIndex)
                    {
                        max = _context.MathFile.GetSpecialReelStrip(_context.State.BonusGame.X2IndexesToRemove, _context.State.BonusGame.removeX8).Count;
                    }
                    else
                    {
                        max = _context.MathFile.GetPrizeReelStrip().Count;
                    }
                    var rand = new RandomNumber
                    {
                        Min = 0,
                        Max = max,
                        Quantity = 1
                    };
                    listOfRands.Add(rand);
                }
            }
            return listOfRands;
        }

        private int GetSpecialIndex(List<int> nonLockedIndexes)
        {
            var specialIndex = _random.Next(1, nonLockedIndexes.Count + 1);
            return nonLockedIndexes[specialIndex - 1];
        }

        public void Resolve()
        {
            if (_context.State.BonusGame.fsLeft == 0 || _context.State.BonusGame.MCSymbols.All(s => s.IsLocked) || _context.State.BonusGame.MCSymbols.Any(s => s.JPSymbolIfString == "paid"))
            {
                foreach (var mcs in _context.State.BonusGame.MCSymbols)
                {
                    _context.State.BonusGame.winAmount += mcs.winAmount;
                }
            }
            else
            {
                throw new Exception("Cannot Resolve unfinished Bonus game");
            }
        }

    }
}
