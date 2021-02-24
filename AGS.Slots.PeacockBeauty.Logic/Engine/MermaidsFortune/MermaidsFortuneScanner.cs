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
        private readonly IRandom _random ;
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
                    if (reelItem.Symbol == 12)
                        result.Scatter.Add(reelItem);
                    if (reelItem.Symbol == 13)
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
        bool isBW(int winSymbol)
        {
            if (winSymbol == 11)
                return true;

            return false;
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

        public static int AmountOfMoneyInJackpots;
        public static int AmountOfWonJackpot;
        public static HashSet<int> excludes = new HashSet<int>(new int[3] { 0, 1, 11 });
        //this function scan the matrix we got and checks whats going on.
        //note that we do have 3*5 matrix and now we want to analyze it.
        public  void Scan(Result result)
        {
            if (_context.RequestItems.isFreeSpin)
            {
                MakeWholeReelWild();
            }

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
                if (symbol == 0)
                {

                }
                if (symbol != 12 && symbol != 13)
                {
                    HandleReel(symbol, 1, resultMatrix, lines, ways);

                }
            }



            //now we have 3 options of winning:
            //1 - we get 3 or more same symbols starting from the left reel.
            foreach (var symbol in lines.Keys)
            {
                if (symbol == 12)
                {

                }

                var win = new Win()
                {
                    Ways = ways[symbol],
                    WinningLines = new HashSet<ItemOnReel>(lines[symbol]),
                    Symbol = symbol,
                    WinType = WinType.Regular
                };
                if (win.LongestSequence > 2)
                {
                    if (win.Symbol == 12)
                    {

                    }
                    result.Wins.Add(win);

                }
                if (win.WinningLines.Count == 2)
                {

                }

                if (win.LongestSequence == 2 && win.Symbol == 1)
                {
                    if (win.WinningLines.Any(x => x.Symbol != 1 && x.Symbol != 0))
                    {

                    }
                    result.Wins.Add(win);

                }
            }
            //2
            if (resultMatrix.SelectMany(x => x).ToList().Any(y => y.Symbol == 0))
            {
                var betLevel = _context.MathFile.BetSteps.IndexOf(_context.GetBetAmount());
                var isTrigger = _context.MathFile.WildTriggerJackpot(_context.RequestItems.isFreeSpin, betLevel, _random);
                if (isTrigger)
                {
                    var win = new Win()
                    {
                        Ways = 0,
                        Symbol = 0,
                        WinType = WinType.JackPot
                    };
                    result.Wins.Add(win);
                }
            }

            //3 - if we got 3 or more scatter (12), we go to Gateway to chose Gateway-FS or gateway-MC.
            if (result.Scatter.Count() > 2)
            {
                var win = new Win()
                {
                    Ways = 0,
                    WinningLines = new HashSet<ItemOnReel>(result.Scatter),
                    Symbol = 12,
                    WinType = WinType.FreeSpin
                };
                result.Wins.Add(win);
            }
            //3 - if we got 6+ mcsymbols we go to regular-MC.
            if (result.McSymbols.Count() > 5)
            {
                var win = new Win()
                {
                    Ways = 0,
                    WinningLines = new HashSet<ItemOnReel>(result.McSymbols),
                    Symbol = 13,
                    WinType = WinType.BonusGames
                };
                result.Wins.Add(win);
            }



        }
        private void MakeWholeReelWild()
        {
            //CHECKED - WORKS WELL
            for (int i = 0; i < resultMatrix.Count; i++)
            {
                if (resultMatrix[i].Any(item => item.Symbol == 0))
                {
                    resultMatrix[i].ForEach(x => x.Symbol = 0);
                }
            }
        }
    }
}
