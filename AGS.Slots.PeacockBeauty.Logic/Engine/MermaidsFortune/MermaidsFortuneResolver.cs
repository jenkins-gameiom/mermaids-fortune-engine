﻿using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using Autofac.Features.Indexed;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AGS.Slots.MermaidsFortune.Common;

namespace AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune
{
    public class MermaidsFortuneResolver : IPayoutResolver
    {
        private readonly IRequestContext _context;
        private readonly IRandom _random;


        public MermaidsFortuneResolver(IRequestContext context, Configs applicationConfig, IIndex<RandomizerType, IRandom> random)
        {
            if (applicationConfig.IsTest)
                _random = random[RandomizerType.Local];
            else
                _random = random[RandomizerType.Remote];
            _context = context;
        }
        public MermaidsFortuneResolver(IRequestContext context, Configs applicationConfig, IRandom random)
        {
            _random = random;
            _context = context;
        }

        public static long SpinsWithoutFSWin = 0;
        public static long SpinsWithFSWin = 0;
        
        public static float FSWinRate = 0;
        public static long MoneyEarnedFromFS = 0;
        public static long MoneyEarnedFromFSBinary = 0;
        //after we got the results and the wins, 
        public void EvaluateResult(Result result)
        {
            if (SpinsWithFSWin > 0)
            {
                FSWinRate = ((float)SpinsWithFSWin / (float)SpinsWithoutFSWin);
            }

            if (_context.State.reelSet == 4 && !_context.RequestItems.isFreeSpin)
            {
                if (result.Wins.Any(x => x.WinType == WinType.FreeSpin))
                {
                    SpinsWithFSWin++;
                }
                else
                {
                    SpinsWithoutFSWin++;
                }
            }
            
            string regular = "Regular - ";
            string fs = "FS - ";
            string stringToChoose = _context.RequestItems.isFreeSpin ? fs : regular;
            List<int> betSteps = _context.Config.stakes;
            var mutliPliers = _context.MathFile.GetLookupPaytable();
            //TODO - check thats how you calculate treasurechestlevel
            var betLevel = betSteps.IndexOf(_context.GetBetAmount());
            AssignMCSymbolsToExisting(result);
            if (!_context.RequestItems.isFreeSpin)
            {
                if (reelSetDictionary == null)
                {
                    reelSetDictionary = new Dictionary<string, double>();
                    reelSetDictionary.Add("0", 0);
                    reelSetDictionary.Add("1", 0);
                    reelSetDictionary.Add("2", 0);
                    reelSetDictionary.Add("3", 0);
                    reelSetDictionary.Add("4", 0);
                }
                reelSetDictionary[_context.State.reelSet.ToString()]++;
            }

            var percentage0 = reelSetDictionary["0"] / reelSetDictionary.Values.Sum();
            var percentage1 = reelSetDictionary["1"] / reelSetDictionary.Values.Sum();
            var percentage2 = reelSetDictionary["2"] / reelSetDictionary.Values.Sum();
            var percentage3 = reelSetDictionary["3"] / reelSetDictionary.Values.Sum();
            var percentage4 = reelSetDictionary["4"] / reelSetDictionary.Values.Sum();


            if (!_context.RequestItems.isFreeSpin)
            {
                if (!myDictionaryTotalSpinsPerSet.ContainsKey(_context.State.reelSet.ToString()))
                {
                    myDictionaryTotalSpinsPerSet.Add(_context.State.reelSet.ToString(), 1);
                }
                else
                {
                    myDictionaryTotalSpinsPerSet[_context.State.reelSet.ToString()]++;
                }
            }
            foreach (var win in result.Wins)
            {
                if (_context.State.holdAndSpin != HoldAndSpin.None)
                {

                }
                //if we got 3 scatters 
                if (win.Symbol == SCATTER)
                {
                    if (!_context.RequestItems.isFreeSpin)
                    {
                        if (!myDictionaryTotalSpinsResultedInFG.ContainsKey(_context.State.reelSet.ToString()))
                        {
                            myDictionaryTotalSpinsResultedInFG.Add(_context.State.reelSet.ToString(), 1);
                        }
                        else
                        {
                            myDictionaryTotalSpinsResultedInFG[_context.State.reelSet.ToString()]++;
                        }
                    }
                    else
                    {

                    }
                    
                    win.GrantedFreeSpins = 8;
                    if (_context.State.freeSpinsLeft == null)
                    {
                        _context.State.freeSpinsLeft = 0;
                    }
                    _context.State.freeSpinsLeft += win.GrantedFreeSpins;
                    _context.State.totalFreeSpins += win.GrantedFreeSpins;
                    var multiPlier = mutliPliers[win.Symbol];
                    var currentLineWinAmount = (long)multiPlier[result.Scatter.Count() - 3] * _context.GetBetAmount() * _context.GetDenom() / _context.MathFile.BetStepsDevider;
                    win.WinAmount = currentLineWinAmount;
                    result.WonAmount += currentLineWinAmount;
                    if (!parts.ContainsKey(stringToChoose +"bn"))
                    {
                        parts.Add(stringToChoose + "bn", win.WinAmount);
                    }
                    else
                    {
                        parts[stringToChoose + "bn"] += win.WinAmount;
                    }
                }
                else if (win.WinType == WinType.FiveOfAKind)
                {
                    _context.State.BonusGame = new BonusGame();
                    _context.State.BonusGame.MCSymbols = new List<MCSymbol>();
                    foreach (var mcSymbol in result.McSymbols)
                    {
                        MCSymbol mcSymbolToAdd = new MCSymbol();
                        mcSymbolToAdd.symbol = mcSymbol.Symbol;
                        if (mcSymbol.Symbol == 10)//JP1 Major
                        {
                            if (_context.RequestItems.isFreeSpin)
                            {

                            }
                            mcSymbolToAdd.winAmount = _context.MathFile.GetProgressiveInformation()[1] * win.Ways * _context.GetDenom();
                            mcSymbolToAdd.JPSymbolIfString = "major";
                            if (!JackpotDistributionDictionary5oak.ContainsKey("major"))
                            {
                                JackpotDistributionDictionary5oak.Add("major", mcSymbolToAdd.winAmount);
                            }
                            else
                            {
                                JackpotDistributionDictionary5oak["major"]+= mcSymbolToAdd.winAmount;
                            }
                        }
                        if (mcSymbol.Symbol == 11)//JP2 Minor
                        {
                            if (_context.RequestItems.isFreeSpin)
                            {

                            }
                            mcSymbolToAdd.winAmount = _context.MathFile.GetProgressiveInformation()[0] * win.Ways * _context.GetDenom();
                            mcSymbolToAdd.JPSymbolIfString = "minor";
                            if (!JackpotDistributionDictionary5oak.ContainsKey("minor"))
                            {
                                JackpotDistributionDictionary5oak.Add("minor", mcSymbolToAdd.winAmount);
                            }
                            else
                            {
                                JackpotDistributionDictionary5oak["minor"] += mcSymbolToAdd.winAmount;
                            }
                        }
                        if (mcSymbol.Symbol == 13)//JP4 Grand
                        {
                            if (_context.RequestItems.isFreeSpin)
                            {

                            }
                            mcSymbolToAdd.winAmount = _context.MathFile.GetProgressiveInformation()[2] * win.Ways * _context.GetDenom();
                            mcSymbolToAdd.JPSymbolIfString = "grand";
                            if (!JackpotDistributionDictionary5oak.ContainsKey("grand"))
                            {
                                JackpotDistributionDictionary5oak.Add("grand", mcSymbolToAdd.winAmount);
                            }
                            else
                            {
                                JackpotDistributionDictionary5oak["grand"]+= mcSymbolToAdd.winAmount;
                                if (JackpotDistributionDictionary5oak["grand"] == 3000000)
                                {

                                }
                            }

                        }
                        if (mcSymbol.Symbol == 12)//JP3 Number
                        {
                            mcSymbolToAdd.winAmount = int.Parse(mcSymbol.MCSymbol) * win.Ways * _context.GetDenom();
                            mcSymbolToAdd.JPSymbolIfString = mcSymbol.MCSymbol;
                            if (!JackpotDistributionDictionary5oak.ContainsKey("money"))
                            {
                                JackpotDistributionDictionary5oak.Add("money", mcSymbolToAdd.winAmount);
                            }
                            else
                            {
                                JackpotDistributionDictionary5oak["money"]+= mcSymbolToAdd.winAmount;
                            }
                        }
                        
                        mcSymbolToAdd.coordinate = mcSymbol.Coordinate;
                        mcSymbolToAdd.index = mcSymbol.Index;
                        win.WinAmount += mcSymbolToAdd.winAmount;
                        result.WonAmount += mcSymbolToAdd.winAmount;
                        _context.State.BonusGame.MCSymbols.Add(mcSymbolToAdd);
                        _context.State.BonusGame.winAmount += mcSymbolToAdd.winAmount;
                        
                    }
                    if (!parts.ContainsKey(stringToChoose + "5ofakind"))
                    {
                        parts.Add(stringToChoose + "5ofakind", win.WinAmount);
                    }
                    else
                    {
                        parts[stringToChoose + "5ofakind"] += win.WinAmount;
                    }
                }
                else
                {
                    //Regular win

                    var multiPlier = mutliPliers[win.Symbol];
                    var currentLineWinAmount = (long)multiPlier[win.LongestSequence - 3] * win.Ways * _context.GetBetAmount() * _context.GetDenom() / _context.MathFile.BetStepsDevider;
                    win.WinAmount += currentLineWinAmount;
                    result.WonAmount += currentLineWinAmount;
                    if (win.LongestSequence == 3)
                    {
                        if (!win.WinningLines.All(y => y.Reel == 0 || y.Reel == 2 || y.Reel == 1))
                        {

                        }
                    }
                    if (win.LongestSequence == 4)
                    {
                        if (!win.WinningLines.All(y => y.Reel == 3 || y.Reel == 0 || y.Reel == 2 || y.Reel == 1))
                        {

                        }
                    }

                    if (win.Symbol == 9)
                    {
                        if (win.LongestSequence == 3)
                        {
                            if (_context.RequestItems.isFreeSpin)
                            {
                                TimesInThreeFS++;
                            }
                            else
                            {
                                TimesInThreeBase++;
                            }
                            if (!parts.ContainsKey(stringToChoose + "3ofakind"))
                            {
                                parts.Add(stringToChoose + "3ofakind", win.WinAmount);
                            }
                            else
                            {
                                parts[stringToChoose + "3ofakind"] += win.WinAmount;
                            }
                        }
                        else if (win.LongestSequence == 4)
                        {
                            if (!win.WinningLines.All(z => z.Reel == 0 || z.Reel == 3 || z.Reel == 2 || z.Reel == 1))
                            {

                            }
                            if (_context.RequestItems.isFreeSpin)
                            {
                                TimesInFourFS++;
                            }
                            else
                            {
                                TimesInFourBase++;
                            }
                            if (!parts.ContainsKey(stringToChoose + "4ofakind"))
                            {
                                parts.Add(stringToChoose + "4ofakind", win.WinAmount);
                            }
                            else
                            {
                                parts[stringToChoose + "4ofakind"] += win.WinAmount;
                            }
                        }

                    }
                    else
                    {
                        if (!parts.ContainsKey(stringToChoose + "regular"))
                        {
                            parts.Add(stringToChoose + "regular", win.WinAmount);
                        }
                        else
                        {
                            parts[stringToChoose + "regular"] += win.WinAmount;
                        }
                    }

                    foreach (var k in parts.Keys)
                    {
                        if (!partsPercentage.ContainsKey(k))
                        {
                            partsPercentage.Add(k, 0);
                        }
                        
                    }

                    foreach (var k in parts.Keys)
                    {
                        if (parts.Values.Sum() > 0)
                        {
                            partsPercentage[k] = (parts[k] / parts.Values.Sum()) * 100;
                        }
                    }

                    var x = partsPercentage.Values.Sum();
                }

                if (_context.RequestItems.isFreeSpin)
                {
                    if (_context.State.isReSpin != null && _context.State.isReSpin.Value)
                    {
                        MoneyEarnedFromFSBinary += result.WonAmount;
                    }
                    else
                    {
                        MoneyEarnedFromFS += result.WonAmount;
                    }
                }
                if (_context.RequestItems.isFreeSpin)
                {
                    RTPFromFS += win.WinAmount;
                }
                else
                {
                    RTPFromBase += win.WinAmount;
                }
            }
            _context.State.completed = _context.State.freeSpinsLeft <= 0  && result.Wins.All(x => x.GrantedFreeSpins == 0);
        }

        public static long RTPFromBase;
        public static long RTPFromFS;
        public static long TimesInThreeFS;
        public static long TimesInFourFS;
        public static long TimesInThreeBase;
        public static long TimesInFourBase;
        public static Dictionary<string, double> reelSetDictionary = null;
        public static Dictionary<int, Dictionary<int, double>> prizesDictionary = new Dictionary<int, Dictionary<int, double>>();
        public static Dictionary<int, Dictionary<int, double>> prizesDictionaryFS = new Dictionary<int, Dictionary<int, double>>();
        public static Dictionary<string, long> parts = new Dictionary<string, long>();
        public static Dictionary<string, double> partsPercentage = new Dictionary<string, double>();
        public static Dictionary<string, double> myDictionaryTotalSpinsPerSet = new Dictionary<string, double>();
        public static Dictionary<string, double> JackpotDistributionDictionary5oak = new Dictionary<string, double>();
        public static Dictionary<string, double> JackpotDistributionDictionary = new Dictionary<string, double>();
        public static Dictionary<string, double> myDictionaryTotalSpinsResultedInFG = new Dictionary<string, double>();
        

        //this method assign value (500, "MINI" etc) to all the mcsymbols (items with value 13)
        private void AssignMCSymbolsToExisting(Result result)
        {
            foreach (var item in result.McSymbols)
            {
                if (item.Symbol == 10)//MAJOR JP1
                {
                    item.MCSymbol = "major";
                    if (!JackpotDistributionDictionary.ContainsKey("major"))
                    {
                        JackpotDistributionDictionary.Add("major", 1);
                    }
                    else
                    {
                        JackpotDistributionDictionary["major"]++;
                    }

                }
                if (item.Symbol == 11)//MINOR JP2
                {
                    item.MCSymbol = "minor";
                    if (!JackpotDistributionDictionary.ContainsKey("minor"))
                    {
                        JackpotDistributionDictionary.Add("minor", 1);
                    }
                    else
                    {
                        JackpotDistributionDictionary["minor"]++;
                    }

                }
                if (item.Symbol == 12)//NUMBER JP3
                {
                    
                    item.MCSymbol = (_context.MathFile.MoneyChargeSymbol(_context.RequestItems.isFreeSpin, _context.State.reelSet, _random) * _context.GetBetAmount()  / _context.MathFile.BetStepsDevider).ToString();// * _context.GetBetAmount()).ToString();
                    SetPrizeStatisticsByBet(_context, item);

                    

                }
                if (item.Symbol == 13)//GRAND JP4
                {
                    item.MCSymbol = "grand";
                    if (!JackpotDistributionDictionary.ContainsKey("grand"))
                    {
                        JackpotDistributionDictionary.Add("grand", 1);
                    }
                    else
                    {
                        JackpotDistributionDictionary["grand"]++;
                        if (JackpotDistributionDictionary["grand"] == 30)
                        {

                        }
                    }

                }
            }
        }

        private void SetPrizeStatisticsByBet(IRequestContext context, ItemOnReel item)
        {
            var reelSet = _context.State.reelSet;
            var mcSymbolInt = int.Parse(item.MCSymbol);
            if (!_context.RequestItems.isFreeSpin)
            {
                if (!prizesDictionary.ContainsKey(reelSet))
                {
                    prizesDictionary.Add(reelSet, new Dictionary<int, double>());
                }
                if (!prizesDictionary[reelSet].ContainsKey(mcSymbolInt))
                {
                    prizesDictionary[reelSet].Add(mcSymbolInt, 1);
                }
                else
                {
                    prizesDictionary[reelSet][mcSymbolInt]++;
                }

                if (reelSet == 0 && item.MCSymbol == "100" && prizesDictionary[reelSet][mcSymbolInt] == 173100)
                {
                    //foreach (var keyValuePair in prizesDictionary)
                    //{
                    //    prizesDictionary[keyValuePair.Key] =  keyValuePair.Value.OrderBy(x => x.Key)
                    //        .ToDictionary(x => x.Key, x => x.Value);
                    //}

                    prizesDictionary = prizesDictionary.OrderBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Value);

                }
                if (reelSet == 1 && item.MCSymbol == "100" && prizesDictionary[reelSet][mcSymbolInt] == 120000)
                {
                    //foreach (var keyValuePair in prizesDictionary)
                    //{
                    //    keyValuePair.Value.OrderBy(x => x.Key)
                    //        .ToDictionary(x => x.Key, x => x.Value);
                    //}

                    prizesDictionary = prizesDictionary.OrderBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Value);

                }
                if (reelSet == 2 && item.MCSymbol == "100" && prizesDictionary[reelSet][mcSymbolInt] == 20000)
                {
                    //foreach (var keyValuePair in prizesDictionary)
                    //{
                    //    keyValuePair.Value.OrderBy(x => x.Key)
                    //        .ToDictionary(x => x.Key, x => x.Value);
                    //}

                    prizesDictionary = prizesDictionary.OrderBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Value);

                }
                if (reelSet == 3 && item.MCSymbol == "100" && prizesDictionary[reelSet][mcSymbolInt] == 30000)
                {
                    //foreach (var keyValuePair in prizesDictionary)
                    //{
                    //    keyValuePair.Value.OrderBy(x => x.Key)
                    //        .ToDictionary(x => x.Key, x => x.Value);
                    //}

                    prizesDictionary = prizesDictionary.OrderBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Value);

                }
                if (reelSet == 4 && item.MCSymbol == "100" && prizesDictionary[reelSet][mcSymbolInt] == 34500)
                {
                    //foreach (var keyValuePair in prizesDictionary)
                    //{
                    //    keyValuePair.Value.OrderBy(x => x.Key)
                    //        .ToDictionary(x => x.Key, x => x.Value);
                    //}

                    prizesDictionary = prizesDictionary.OrderBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Value);

                }
            }
            if (_context.RequestItems.isFreeSpin)
            {
                if (!prizesDictionaryFS.ContainsKey(reelSet))
                {
                    prizesDictionaryFS.Add(reelSet, new Dictionary<int, double>());
                }
                if (!prizesDictionaryFS[reelSet].ContainsKey(mcSymbolInt))
                {
                    prizesDictionaryFS[reelSet].Add(mcSymbolInt, 1);
                }
                else
                {
                    prizesDictionaryFS[reelSet][mcSymbolInt]++;
                }

                if (reelSet == 0 && item.MCSymbol == "100" && prizesDictionaryFS[reelSet][mcSymbolInt] == 173100)
                {
                    //foreach (var keyValuePair in prizesDictionaryFS)
                    //{
                    //    keyValuePair.Value.OrderBy(x => x.Key)
                    //        .ToDictionary(x => x.Key, x => x.Value);
                    //}

                    prizesDictionaryFS = prizesDictionaryFS.OrderBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Value);

                }
                if (reelSet == 1 && item.MCSymbol == "100" && prizesDictionaryFS[reelSet][mcSymbolInt] == 120000)
                {
                    //foreach (var keyValuePair in prizesDictionaryFS)
                    //{
                    //    keyValuePair.Value.OrderBy(x => x.Key)
                    //        .ToDictionary(x => x.Key, x => x.Value);
                    //}

                    prizesDictionaryFS = prizesDictionaryFS.OrderBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Value);

                }
                if (reelSet == 2 && item.MCSymbol == "100" && prizesDictionaryFS[reelSet][mcSymbolInt] == 20000)
                {
                    //foreach (var keyValuePair in prizesDictionaryFS)
                    //{
                    //    keyValuePair.Value.OrderBy(x => x.Key)
                    //        .ToDictionary(x => x.Key, x => x.Value);
                    //}

                    prizesDictionaryFS = prizesDictionaryFS.OrderBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Value);

                }
                if (reelSet == 3 && item.MCSymbol == "100" && prizesDictionaryFS[reelSet][mcSymbolInt] == 30000)
                {
                    //foreach (var keyValuePair in prizesDictionaryFS)
                    //{
                    //    keyValuePair.Value.OrderBy(x => x.Key)
                    //        .ToDictionary(x => x.Key, x => x.Value);
                    //}

                    prizesDictionaryFS = prizesDictionaryFS.OrderBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Value);

                }
                if (reelSet == 4 && item.MCSymbol == "100" && prizesDictionaryFS[reelSet][mcSymbolInt] == 34500)
                {
                    //foreach (var keyValuePair in prizesDictionaryFS)
                    //{
                    //    keyValuePair.Value.OrderBy(x => x.Key)
                    //        .ToDictionary(x => x.Key, x => x.Value);
                    //}

                    prizesDictionaryFS = prizesDictionaryFS.OrderBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Value);

                }
            }
        }

        public const int SCATTER = 14;
        public const int WILD = 0;
        public const int MCSymbol = 9;
        public bool IsWildCard(ItemOnReel item)
        {
            return item.Symbol == WILD;
        }

        public bool IsWildOrMCSymbol(ItemOnReel item)
        {
            return item.Symbol == WILD || item.Symbol == MCSymbol;
        }


        public bool IsScatter(ItemOnReel item)
        {
            return item.Symbol == SCATTER;
        }

        public bool IsMCSymbol(ItemOnReel item)
        {
            return item.Symbol == MCSymbol;
        }
    }
}
