using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using Autofac.Features.Indexed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune
{
    public class MermaidsFortuneScanner
    {
        private List<List<ItemOnReel>> resultMatrix = new List<List<ItemOnReel>>();


        private readonly IRequestContext _context;
        private readonly IRandom _random;
        public MermaidsFortuneScanner(IRequestContext context, IIndex<RandomizerType, IRandom> random, Configs configs)
        {
            _context = context;
            if (configs.IsTest)
                _random = random[RandomizerType.Local];
            else
                _random = random[RandomizerType.Remote];

        }

        public MermaidsFortuneScanner(IRequestContext context, IRandom random, Configs configs)
        {
            _context = context;
            _random = random;
        }




        //this function gets the 3*5 matrix of numbers and creates 3*5 matrix of objects with:
        //coordinate, index (0,1,2,3,4,5,6..), reel number, symbol. so it will be more clear.
        //this function aggregate the type of symbols we got into 3 categories:
        //scatter (we need 3-5 to get into pick), mcsymbols and non mcsymbols.
        public void ApplyResultion(List<List<int>> reels, Result result)
        {
            for (int i = 0; i < reels.Count; i++)
            {
                resultMatrix.Add(new List<ItemOnReel>());
            }
            for (int reel = 0; reel < reels.Count; reel++)
            {
                for (int item = 0; item < reels[reel].Count; item++)
                {
                    var reelItem = new ItemOnReel()
                    {
                        Coordinate = new Tuple<int, int>(reel, item),
                        Index = (reel + 1 + item * 5),
                        Reel = reel,
                        Symbol = reels[reel][item]
                    };
                    resultMatrix[reel].Add(reelItem);
                    if (reelItem.Symbol == 14)
                        result.Scatter.Add(reelItem);
                    if (new int[] { 10, 11, 12, 13 }.Contains(reelItem.Symbol))
                    {
                        result.McSymbols.Add(reelItem);
                    }
                    else
                    {
                        result.NonMcSymbols.Add(reelItem);
                    }
                }
            }
        }

        bool isWild(int winSymbol)
        {
            if (winSymbol == 0)
                return true;

            return false;
        }

        public bool IsWildOrMCSymbol(int winSymbol)
        {
            return winSymbol == 0 || winSymbol == 9;
        }
        public bool IsMCSymbol(int winSymbol)
        {
            return winSymbol == 9;
        }
        //this function scans the second to the last reel, in order to check regular winnings (not scatter or bonus or fs)
        //(just regular 3 to 5 same symbols (or with wild) winning).
        void HandleReel(int winningSymbol, int currentReelIndex, List<List<ItemOnReel>> spinResultMatrix, Dictionary<int, List<ItemOnReel>> resultDictionary, Dictionary<int, int> ways)
        {
            int tempWays = 0;

            if (currentReelIndex < spinResultMatrix.Count)
            {
                bool foundRes = false;
                foreach (var item in spinResultMatrix[currentReelIndex])
                {
                    if (winningSymbol != MermaidsFortuneResolver.SCATTER && item.Symbol == MermaidsFortuneResolver.WILD)
                    {
                        resultDictionary[winningSymbol].Add(item);
                        foundRes = true;
                        tempWays++;
                    }
                    else if (item.Symbol == winningSymbol)
                    {
                        resultDictionary[item.Symbol].Add(item);
                        foundRes = true;
                        tempWays++;
                    }
                }
                if (foundRes)
                {
                    currentReelIndex++;
                    if (tempWays > 1)
                        ways[winningSymbol] *= tempWays;
                    HandleReel(winningSymbol, currentReelIndex, spinResultMatrix, resultDictionary, ways);
                }
            }

        }
        //this function scan the matrix we got and checks whats going on.
        //note that we do have 3*5 matrix and now we want to analyze it.

        public static Dictionary<string, int> dic = new Dictionary<string, int>();
        public void Scan(Result result)
        {
            //this part is just to get the items in the first reel. note that wins (3 to 5 same symbols)
            //can start only from the first reel.
            Dictionary<int, List<ItemOnReel>> lines = new Dictionary<int, List<ItemOnReel>>();
            Dictionary<int, int> ways = new Dictionary<int, int>();
            for (int i = 0; i < resultMatrix[0].Count; i++)
            {
                if (!lines.ContainsKey(resultMatrix[0][i].Symbol))
                    lines.Add(resultMatrix[0][i].Symbol, new List<ItemOnReel>());
                lines[resultMatrix[0][i].Symbol].Add(resultMatrix[0][i]);

                if (!ways.ContainsKey(resultMatrix[0][i].Symbol))
                    ways.Add(resultMatrix[0][i].Symbol, 1);
                else
                {
                    ways[resultMatrix[0][i].Symbol]++;
                }
            }
            result.Wins = new List<Win>();

            //now after we got the first reel, lets check the others to see if it contains the same symbols as the first reel.
            foreach (var symbol in lines.Keys.Distinct())
            {
                if (symbol != 9 && symbol != 14)
                {
                    HandleReel(symbol, 1, resultMatrix, lines, ways);
                }
            }

            //now we have 3 options of winning:
            //1 - we get 3 or more same symbols starting from the left reel.
            foreach (var symbol in lines.Keys)
            {
                var win = new Win()
                {
                    Ways = ways[symbol],
                    WinningLines = new HashSet<ItemOnReel>(lines[symbol]),
                    Symbol = symbol,
                    WinType = WinType.Regular
                };
                if (win.LongestSequence > 2)
                {
                    result.Wins.Add(win);
                }
            }

            //3 scatter wins a freespin
            if (result.Scatter.Count() > 2)
            {
                var win = new Win()
                {
                    Ways = 0,
                    WinningLines = new HashSet<ItemOnReel>(result.Scatter),
                    Symbol = 14,
                    WinType = WinType.FreeSpin
                };
                result.Wins.Add(win);
            }
            HandleRespin();
            HandleOak(result);
        }

        private void HandleOak(Result result)
        {
            //3 - if we have 5 diamonds in a row
            if (resultMatrix[0].Any(x => IsWildOrMCSymbol(x.Symbol)) &&
                resultMatrix[1].Any(x => IsWildOrMCSymbol(x.Symbol)) &&
                resultMatrix[2].Any(x => new int[] { 10, 11, 12, 13 }.Contains(x.Symbol)) &&
                resultMatrix[3].Any(x => IsWildOrMCSymbol(x.Symbol)) &&
                resultMatrix[4].Any(x => IsWildOrMCSymbol(x.Symbol)))
            {
                var fiveOfAkindWays =
                    resultMatrix[0].Count(x => IsWildOrMCSymbol(x.Symbol)) *
                    resultMatrix[1].Count(x => IsWildOrMCSymbol(x.Symbol)) *
                    1 *
                    resultMatrix[3].Count(x => IsWildOrMCSymbol(x.Symbol)) *
                    resultMatrix[4].Count(x => IsWildOrMCSymbol(x.Symbol));
                var win = new Win()
                {
                    Ways = fiveOfAkindWays,
                    WinningLines = new HashSet<ItemOnReel>(resultMatrix.SelectMany(x => x)
                        .Where(x => new int[] { 9, 10, 11, 12, 13, 0 }.Contains(x.Symbol)).ToList()),
                    Symbol = 9,
                    WinType = WinType.FiveOfAKind
                };
                result.Wins.Add(win);
            }
            //3 - get powerextream of diamonds (25 for 3, 50 for 4)
            else if (resultMatrix[0].Any(x => IsWildOrMCSymbol(x.Symbol)) &&
                     resultMatrix[1].Any(x => IsWildOrMCSymbol(x.Symbol)) &&
                     resultMatrix[2].Any(x => new int[] { 10, 11, 12, 13, 0 }.Contains(x.Symbol)))
            {
                var reelsOfWinAmount = 3;
                var threeOrFourOAKWays = resultMatrix[0].Count(x => IsWildOrMCSymbol(x.Symbol)) *
                            resultMatrix[1].Count(x => IsWildOrMCSymbol(x.Symbol)) *
                            resultMatrix[2].Count(x => new int[] { 10, 11, 12, 13, 0 }.Contains(x.Symbol));
                if (resultMatrix[3].Any(x => IsWildOrMCSymbol(x.Symbol)))
                {
                    threeOrFourOAKWays *= resultMatrix[3].Count(x => IsWildOrMCSymbol(x.Symbol));
                    reelsOfWinAmount = 4;
                }

                var win = new Win()
                {
                    Ways = threeOrFourOAKWays,
                    WinningLines = new HashSet<ItemOnReel>(resultMatrix.SelectMany(x => x).Where(x =>
                        new int[] { 9, 10, 11, 12, 13, 0 }.Contains(x.Symbol) && x.Reel < reelsOfWinAmount).ToList()),
                    Symbol = 9,
                    WinType = WinType.Regular
                };
                result.Wins.Add(win);
            }
        }

        private void HandleRespin()
        {
            _context.State.isReSpin = false;
            //3 scatter wins a freespin
            //here we calculate the ReSpin feature
            if (_context.RequestItems.isFreeSpin)
            {
                //first reel is 0'd 01000 => 01000 01010
                if (resultMatrix[1].All(x => x.Symbol == 0) && !resultMatrix[3].All(x => x.Symbol == 0))
                {
                    if (_context.State.holdAndSpin != HoldAndSpin.None)
                    {
                        _context.State.holdAndSpin = HoldAndSpin.None;
                    }
                    else
                    {
                        _context.State.holdAndSpin = HoldAndSpin.First;
                        _context.State.isReSpin = true;
                    }
                }

                //second reel is 0'd 00010
                if (resultMatrix[3].All(x => x.Symbol == 0) && !resultMatrix[1].All(x => x.Symbol == 0))
                {
                    if (_context.State.holdAndSpin != HoldAndSpin.None)
                    {
                        _context.State.holdAndSpin = HoldAndSpin.None;
                    }
                    else
                    {
                        _context.State.holdAndSpin = HoldAndSpin.Second;
                        _context.State.isReSpin = true;
                    }
                }

                //both reel are 0'd 01010
                if (resultMatrix[1].All(x => x.Symbol == 0) && resultMatrix[3].All(x => x.Symbol == 0))
                {
                    if (_context.State.holdAndSpin == HoldAndSpin.None)
                    {
                        _context.State.holdAndSpin = HoldAndSpin.Both;
                        _context.State.isReSpin = true;
                    }
                    else if (_context.State.holdAndSpin == HoldAndSpin.First)
                    {
                        _context.State.holdAndSpin = HoldAndSpin.Both;
                        _context.State.isReSpin = true;
                    }
                    else if (_context.State.holdAndSpin == HoldAndSpin.Second)
                    {
                        _context.State.holdAndSpin = HoldAndSpin.Both;
                        _context.State.isReSpin = true;
                    }
                    else
                    {
                        _context.State.holdAndSpin = HoldAndSpin.None;
                    }
                }
            }
            else
            {
                _context.State.holdAndSpin = HoldAndSpin.None;
            }
        }
    }
}
