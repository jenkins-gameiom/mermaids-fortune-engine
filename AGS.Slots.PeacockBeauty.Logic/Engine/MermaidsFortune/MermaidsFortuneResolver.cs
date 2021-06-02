using AGS.Slots.MermaidsFortune.Common.Entities;
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
using Microsoft.VisualBasic;

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
        //public static bool isRespinNext = false;
        //public static HoldAndSpin respinType = HoldAndSpin.None;

        public static long RespinBreakDown01000 = 0;
        public static long RespinBreakDown00010 = 0;
        public static long RespinBreakDown01010 = 0;
        public static long HitRateRespinBreakDown01000 = 0;
        public static long HitRateRespinBreakDown00010 = 0;
        public static long HitRateRespinBreakDown01010 = 0;
        public static long TotalFS = 0;
        public static long RespinBreakDownNoRespin = 0;
        public static Dictionary<string, long> parts = new Dictionary<string, long>();
        public static Dictionary<string, long> fsMCSymbolsWeightsRS1 = new Dictionary<string, long>();
        public static Dictionary<string, long> fsMCSymbolsWeightsRS2 = new Dictionary<string, long>();
        public static Dictionary<string, long> fsMCSymbolsWeightsRS3 = new Dictionary<string, long>();
        //after we got the results and the wins, 
        public void EvaluateResult(Result result)
        {
            string regular = "Regular - ";
            string fs = "FS - ";
            string stringToChoose = _context.RequestItems.isFreeSpin ? fs : regular;
            List<int> betSteps = _context.Config.stakes;
            var mutliPliers = _context.MathFile.GetLookupPaytable();
            //TODO - check thats how you calculate treasurechestlevel
            AssignMCSymbolsToExisting(result);
            if (_context.RequestItems.isFreeSpin && _context.State.isRespinResolver)
            {
                TotalFS++;
                if (_context.State.respinTypeResolver == HoldAndSpin.First)
                {
                    HitRateRespinBreakDown01000++;
                }
                if (_context.State.respinTypeResolver == HoldAndSpin.Second)
                {
                    HitRateRespinBreakDown00010++;
                }
                if (_context.State.respinTypeResolver == HoldAndSpin.Both)
                {
                    HitRateRespinBreakDown01010++;
                }
            }
            foreach (var win in result.Wins)
            {
                //if we got 3 scatters 
                if (win.Symbol == SCATTER)
                {
                    win.GrantedFreeSpins = 8;
                    if (_context.State.freeSpinsLeft == null)
                    {
                        _context.State.freeSpinsLeft = 0;
                    }
                    _context.State.freeSpinsLeft += win.GrantedFreeSpins;
                    if (_context.State.totalFreeSpins == null)
                    {
                        _context.State.totalFreeSpins = 0;
                    }
                    _context.State.totalFreeSpins += win.GrantedFreeSpins;
                    var multiPlier = mutliPliers[win.Symbol];
                    var currentLineWinAmount = (long)multiPlier[result.Scatter.Count() - 3] * _context.GetBetAmount() * _context.GetDenom() / _context.MathFile.BetStepsDevider;
                    win.WinAmount = currentLineWinAmount;
                    if (_context.RequestItems.isFreeSpin)
                    {
                        if (_context.State.isRespinResolver)
                        {
                            if (_context.State.respinTypeResolver == HoldAndSpin.First)
                            {
                                RespinBreakDown01000 += win.WinAmount;
                            }
                            if (_context.State.respinTypeResolver == HoldAndSpin.Second)
                            {
                                RespinBreakDown00010 += win.WinAmount;
                            }
                            if (_context.State.respinTypeResolver == HoldAndSpin.Both)
                            {
                                RespinBreakDown01010 += win.WinAmount;
                            }
                        }
                        else
                        {
                            RespinBreakDownNoRespin += win.WinAmount;
                        }
                    }
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
                    win.MCSymbols = new List<MCSymbol>();
                    foreach (var mcSymbol in result.McSymbols)
                    {
                        MCSymbol mcSymbolToAdd = new MCSymbol();
                        mcSymbolToAdd.symbol = mcSymbol.Symbol;
                        if (mcSymbol.Symbol == 10)//JP1 Major
                        {
                            mcSymbolToAdd.winAmount = _context.MathFile.GetProgressiveInformation()[1] * win.Ways * _context.GetDenom();
                            mcSymbolToAdd.JPSymbolIfString = "major";
                        }
                        if (mcSymbol.Symbol == 11)//JP2 Minor
                        {
                            mcSymbolToAdd.winAmount = _context.MathFile.GetProgressiveInformation()[0] * win.Ways * _context.GetDenom();
                            mcSymbolToAdd.JPSymbolIfString = "minor";
                        }
                        if (mcSymbol.Symbol == 13)//JP4 Grand
                        {
                            mcSymbolToAdd.winAmount = _context.MathFile.GetProgressiveInformation()[2] * win.Ways * _context.GetDenom();
                            mcSymbolToAdd.JPSymbolIfString = "grand";

                        }
                        if (mcSymbol.Symbol == 12)//JP3 Number
                        {
                            mcSymbolToAdd.winAmount = int.Parse(mcSymbol.MCSymbol) * win.Ways * _context.GetDenom();
                            mcSymbolToAdd.JPSymbolIfString = mcSymbol.MCSymbol;
                        }
                        
                        mcSymbolToAdd.coordinate = mcSymbol.Coordinate;
                        mcSymbolToAdd.index = mcSymbol.Index;
                        win.WinAmount += mcSymbolToAdd.winAmount;
                        if (_context.RequestItems.isFreeSpin)
                        {
                            if (_context.State.isRespinResolver)
                            {
                                if (_context.State.respinTypeResolver == HoldAndSpin.First)
                                {
                                    RespinBreakDown01000 += mcSymbolToAdd.winAmount;
                                }
                                if (_context.State.respinTypeResolver == HoldAndSpin.Second)
                                {
                                    RespinBreakDown00010 += mcSymbolToAdd.winAmount;
                                }
                                if (_context.State.respinTypeResolver == HoldAndSpin.Both)
                                {
                                    RespinBreakDown01010 += mcSymbolToAdd.winAmount;
                                }
                            }
                            else
                            {
                                RespinBreakDownNoRespin += mcSymbolToAdd.winAmount;
                            }
                            var dic = fsMCSymbolsWeightsRS1;

                            if (_context.State.reelSet == 0)
                            {
                                dic = fsMCSymbolsWeightsRS1;
                                if (dic.ContainsKey("100") && dic["100"] == 1731)
                                {

                                }
                            }
                            if (_context.State.reelSet == 1)
                            {
                                dic = fsMCSymbolsWeightsRS2;
                                if (dic.ContainsKey("100"))
                                {
                                    float z = (float)1200 / (float)dic["100"];
                                    var y = dic.Select(x => new { ArgKey = int.Parse(x.Key), ArgValue = x.Value * z }).OrderBy(x => x.ArgKey);
                                }
                                if (dic.ContainsKey("100") && dic["100"] == 1200)
                                {
                                }
                            }
                            if (_context.State.reelSet == 2)
                            {
                                dic = fsMCSymbolsWeightsRS3;
                                if (dic.ContainsKey("100"))
                                {
                                    float z = (float)200 / (float)dic["100"];
                                    var y = dic.Select(x => new{ArgKey = int.Parse(x.Key), ArgValue = x.Value * z}).OrderBy(x => x.ArgKey);
                                }
                                if (dic.ContainsKey("100") && dic["100"] == 200)
                                {
                                }
                            }

                            
                            if (!dic.ContainsKey(mcSymbolToAdd.JPSymbolIfString))
                            {
                                dic.Add(mcSymbolToAdd.JPSymbolIfString, 0);
                            }
                            dic[mcSymbolToAdd.JPSymbolIfString]++;
                        }
                        result.WonAmount += mcSymbolToAdd.winAmount;
                        win.MCSymbols.Add(mcSymbolToAdd);
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
                    if (_context.RequestItems.isFreeSpin)
                    {
                        if (_context.State.isRespinResolver)
                        {
                            if (_context.State.respinTypeResolver == HoldAndSpin.First)
                            {
                                RespinBreakDown01000 += win.WinAmount;
                            }
                            if (_context.State.respinTypeResolver == HoldAndSpin.Second)
                            {
                                RespinBreakDown00010 += win.WinAmount;
                            }
                            if (_context.State.respinTypeResolver == HoldAndSpin.Both)
                            {
                                RespinBreakDown01010 += win.WinAmount;
                            }
                        }
                        else
                        {
                            RespinBreakDownNoRespin += win.WinAmount;
                        }
                    }
                    if (win.Symbol == 9)
                    {
                        if (win.LongestSequence == 3)
                        {
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
                }
            }

            _context.State.respinTypeResolver = HoldAndSpin.None;
            _context.State.isRespinResolver = false;
            if (_context.RequestItems.isFreeSpin)
            {
                if (_context.State.isReSpin)
                {
                    _context.State.isRespinResolver = true;
                    _context.State.respinTypeResolver = _context.State.holdAndSpin;
                }
                else
                {
                    _context.State.isRespinResolver = false;
                    _context.State.respinTypeResolver = HoldAndSpin.None;
                }
            }
            _context.State.completed = _context.State.freeSpinsLeft <= 0  && !_context.State.isReSpin && result.Wins.All(x => x.GrantedFreeSpins == 0);
        }
        

        //this method assign value (500, "MINI" etc) to all the mcsymbols (items with value 13)
        private void AssignMCSymbolsToExisting(Result result)
        {
            foreach (var item in result.McSymbols)
            {
                if (item.Symbol == 10)//MAJOR JP1
                {
                    item.MCSymbol = "major";
                }
                if (item.Symbol == 11)//MINOR JP2
                {
                    item.MCSymbol = "minor";
                }
                if (item.Symbol == 12)//NUMBER JP3
                {
                    
                    item.MCSymbol = (_context.MathFile.MoneyChargeSymbol(_context.RequestItems.isFreeSpin, _context.State.reelSet, _random, _context.State.respinTypeResolver) * _context.GetBetAmount()  / _context.MathFile.BetStepsDevider).ToString();// * _context.GetBetAmount()).ToString();
                }
                if (item.Symbol == 13)//GRAND JP4
                {
                    item.MCSymbol = "grand";
                }
            }
        }

        public const int SCATTER = 14;
        public const int WILD = 0;
        public const int MCSymbol = 9;
    }
}
