using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

using Newtonsoft.Json;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.Providers;
using AGS.Slots.MermaidsFortune.Logic;
using Autofac.Features.Indexed;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using Win = AGS.Slots.MermaidsFortune.Common.Entities.Win;
using AGS.Slots.MermaidsFortune.Logic.Engine;

namespace AGS.Slots.MermaidsFortune.Platform
{
    public class Spins
    {

        //Random only for visualization
        private static System.Random VisualiztionRandom = new System.Random();
        private readonly Configs _configs;
        private readonly GameEngine _gameEngine;
        private readonly IRequestContext _context;
        public static IRandom _random = new RandomGeneratorCrypro();

        public Spins(IRequestContext context, Configs configs, GameEngine engineLogic)
        {
            _context = context;
            _gameEngine = engineLogic;
            _configs = configs;
        }

        //Platform RNG

        //Propability of fireworks visualization
        private static readonly decimal[] SparkPropibility = new decimal[] { 0.0442477876106195M, 0.0892857142857143M, 0.1785714285714286M, 0.4M, 1 };

        //Force (Gaffing) only on test 
        private Result ForceSymbolsWithResult(dynamic force, List<List<int>> spinResult, bool isFreeSpin, out bool goToRegularSpin)
        {
            Result res = null;
            goToRegularSpin = false;
            try
            {
                if (_configs.IsTest)
                {
                    if (force != null)
                    {
                        //good old force
                        string forceInString = force.ToString();
                        string jsonToGive = "";
                        if (forceInString.Contains(","))
                        {
                            var selectedIndexes = (List<int>)Json.ConvertDynamic<List<int>>(force);
                            var reels = _context.MathFile.GetFullReels(_context, selectedIndexes).reels;
                            for (int i = 0; i < reels.Count; i++)
                            {
                                int sNum = i == 2 ? 4 : 3;
                                var currReel = new List<int>();
                                for (int j = 0; j < sNum; j++)
                                    currReel.Add(reels[i][(j + selectedIndexes[i]) % reels[i].Count]);

                                spinResult.Add(currReel);
                            }

                            goToRegularSpin = true;
                            return null;
                        }
                        if (GameEngine.FileExists(forceInString))
                        {
                            res = GameEngine.DeserializeAndReturnResult(forceInString);
                            SetState(res);
                        }
                        else
                        {
                            res = null;
                        }
                        return res;
                    }
                }
            }
            catch
            {
                throw new Exception("Error occured in ForceSymbols");
            }
            return res;
        }

        private void SetState(Result res)
        {
            SetFreeSpin(res);
            SetHoldAndSpin(res);
            if (res.Wins.Any(x => x.WinType == WinType.FiveOfAKind))
            {
                SetFiveOfAKind(res);
            }
            _context.State.completed = !_context.State.isReSpin && _context.State.freeSpinsLeft <= 0;
        }

        private void SetFreeSpin(Result res)
        {
            if (res.Wins.Any(x => x.WinType == WinType.FreeSpin))
            {
                if (_context.State.freeSpinsLeft == null)
                {
                    _context.State.freeSpinsLeft = 0;
                }

                _context.State.freeSpinsLeft += res.Wins.First(x => x.WinType == WinType.FreeSpin).GrantedFreeSpins;
                _context.State.totalFreeSpins += res.Wins.First(x => x.WinType == WinType.FreeSpin).GrantedFreeSpins;
            }
        }

        private void SetHoldAndSpin(Result res)
        {
            if (_context.RequestItems.isFreeSpin)
            {
                if (_context.State.holdAndSpin == HoldAndSpin.First && !res.Reels[1].All(x => x == 0))
                {
                    for (int i = 0; i < res.Reels[1].Count; i++)
                    {
                        res.Reels[1][i] = 0;
                    }
                }
                if (_context.State.holdAndSpin == HoldAndSpin.Second && !res.Reels[3].All(x => x == 0))
                {
                    for (int i = 0; i < res.Reels[3].Count; i++)
                    {
                        res.Reels[3][i] = 0;
                    }
                }

                _context.State.isReSpin = false;
                //first reel is 0'd
                if (res.Reels[1].All(x => x == 0) && !res.Reels[3].All(x => x == 0))
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

                //second reel is 0'd
                if (res.Reels[3].All(x => x == 0) && !res.Reels[1].All(x => x == 0))
                {
                    if (_context.State.holdAndSpin != HoldAndSpin.None)
                    {
                        if (!_context.State.isReSpin)
                        {
                        }

                        _context.State.holdAndSpin = HoldAndSpin.None;
                    }
                    else
                    {
                        if (_context.State.freeSpinsLeft == 1)
                        {

                        }
                        _context.State.holdAndSpin = HoldAndSpin.Second;
                        _context.State.isReSpin = true;
                    }
                }

                //both reel are 0'd
                if (res.Reels[1].All(x => x == 0) && res.Reels[3].All(x => x == 0))
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

        private void SetFiveOfAKind(Result res)
        {
            var win = res.Wins.First(x => x.WinType == WinType.FiveOfAKind);
            _context.State.BonusGame = new BonusGame();
            _context.State.BonusGame.MCSymbols = new List<MCSymbol>();
            foreach (var mcSymbol in res.McSymbols)
            {
                MCSymbol mcSymbolToAdd = new MCSymbol();
                mcSymbolToAdd.symbol = mcSymbol.Symbol;
                if (mcSymbol.Symbol == 10) //JP1 Major
                {
                    mcSymbolToAdd.winAmount = _context.MathFile.GetProgressiveInformation()[1] * win.Ways *
                                              _context.GetDenom();
                    mcSymbolToAdd.JPSymbolIfString = "major";
                }

                if (mcSymbol.Symbol == 11) //JP2 Minor
                {
                    if (_context.RequestItems.isFreeSpin)
                    {

                    }

                    mcSymbolToAdd.winAmount = _context.MathFile.GetProgressiveInformation()[0] * win.Ways *
                                              _context.GetDenom();
                    mcSymbolToAdd.JPSymbolIfString = "minor";
                }

                if (mcSymbol.Symbol == 13) //JP4 Grand
                {
                    if (_context.RequestItems.isFreeSpin)
                    {

                    }

                    mcSymbolToAdd.winAmount = _context.MathFile.GetProgressiveInformation()[2] * win.Ways *
                                              _context.GetDenom();
                    mcSymbolToAdd.JPSymbolIfString = "grand";

                }

                if (mcSymbol.Symbol == 12) //JP3 Number
                {
                    mcSymbolToAdd.winAmount = int.Parse(mcSymbol.MCSymbol) * win.Ways * _context.GetDenom();
                    mcSymbolToAdd.JPSymbolIfString = mcSymbol.MCSymbol;
                }

                mcSymbolToAdd.coordinate = mcSymbol.Coordinate;
                mcSymbolToAdd.index = mcSymbol.Index;
                
                _context.State.BonusGame.MCSymbols.Add(mcSymbolToAdd);
                _context.State.BonusGame.winAmount += mcSymbolToAdd.winAmount;
            }
        }

        //Check the bet amount and chips perplay to make suere its correct.
        private void CheckBetsAndChipsPerPlay (List<int> stacksFromPlatform, List<int> denominationsFromPlatform, ref int betAmount, ref int chipsPerPlay)
        {
            betAmount = _context.GetBetAmount();
            chipsPerPlay = _context.GetDenom();

            var betIndex = stacksFromPlatform.IndexOf(betAmount);
            var denomIndex = denominationsFromPlatform.IndexOf(chipsPerPlay);
            if (betIndex < 0)
                throw new Exception("Error betAmount not valid " + betAmount);
            if (!_context.Config.denominations.Contains(chipsPerPlay))
                throw new Exception("Error chipsPerPlay not valid " + chipsPerPlay);
        }

        //private void SetJackpotGame(JackpotGame jackpotGame)
        //{
        //    _context.State.JackpotGame = new JackpotGame();
        //    _context.State.JackpotGame.outcome = jackpotGame.outcome;
        //    var listOfValues = _context.MathFile.GetRandomJackpotCombination(_context.State.JackpotGame.outcome, _random);
        //    _context.State.JackpotGame.leftItems = listOfValues.Count;
        //    foreach (var x in listOfValues)
        //    {
        //        _context.State.JackpotGame.selectedItems.Add(new JackpotItem()
        //        {
        //            Symbol = x
        //        });
        //    }
        //}


        private Spin CreateSpinObject(bool isFreeSpin, List<Win> wins, Result result,
            IStateItems state, ref long freeSpinsTotalWin)
        {
            if (isFreeSpin)
            {
                freeSpinsTotalWin += result.WonAmount;
            }

            //Set all wins
            foreach (var win in result.Wins)
            {
                Win w = null;
                if (win.WinType == WinType.Regular)
                {
                    w = new Win()
                    {
                        featureType = FeatureType.PowerXStream.ToString(),
                        evaluationType = EvaluationType.PowerXStream.ToString(),
                        winAmount = win.WinAmount,
                        symbolId = win.Symbol,
                        ways = win.Ways,
                        winningSymbolsPositions = win.WinningLines
                            .Select(x => new int[] { x.Coordinate.Item1, x.Coordinate.Item2 }).ToArray()
                    };
                    w.multiplier = win.WinAmount / win.Ways;
                }
                else if (win.WinType == WinType.FreeSpin)
                {
                    //Set FS Win on base 
                    if (win.GrantedFreeSpins > 0) //remove right part of if
                    {
                        w = new Win()
                        {
                            featureType = FeatureType.freeSpinWin.ToString(),
                            evaluationType = FeatureType.PowerXStream.ToString(),
                            freeSpinsAmount = win.GrantedFreeSpins,
                            winAmount = win.WinAmount,
                            winningSymbolsPositions = win.WinningLines.Select(x => new int[] { x.Coordinate.Item1, x.Coordinate.Item2 }).ToArray(),
                            symbolId = win.Symbol
                        };
                    }
                }

                else if (win.WinType == WinType.FiveOfAKind)
                {
                    w = new Win()
                    {
                        featureType = FeatureType.fiveofakind.ToString(),
                        evaluationType = EvaluationType.random.ToString(),
                        ways = win.Ways,
                        symbolId = win.Symbol,
                        winAmount = win.WinAmount,
                        winningSymbolsPositions = win.WinningLines.Select(x => new int[] { x.Coordinate.Item1, x.Coordinate.Item2 }).ToArray(),
                        MCSymbols = win.MCSymbols.ToList()
                        
                    };
                    //SetJackpotGame();
                }
                wins.Add(w);
            }

            var spin = new Spin()
            {
                reels = result.Reels.Select(a => a.ToArray()).ToArray(),
                sumWins = result.WonAmount,
                wins = wins,
                totalSpinWin = result.WonAmount,
                type = isFreeSpin ? "freeSpin" : "spin",
                childFeature = new List<Spin>(),
                events = new List<Event>(),
                holdAndSpin = _context.State.holdAndSpin

            };
            spin.MCSymbols = new List<MCSymbol>();
            if (result.McSymbols.Count() > 0 && _context.State.BonusGame == null)
            {
                foreach (var item in result.McSymbols)
                {
                    spin.MCSymbols.Add(new MCSymbol(item.Index, item.Coordinate.Item1, item.Coordinate.Item2, item.Symbol, TableTypeEnum.Regular, 0, item.MCSymbol));
                }
            }
            if (_context.State.BonusGame != null && _context.State.BonusGame.MCSymbols.Count > 0)
            {
                foreach (var item in _context.State.BonusGame.MCSymbols)
                {
                    spin.MCSymbols.Add(new MCSymbol(item.index, item.coordinate.Item1, item.coordinate.Item2, item.symbol, TableTypeEnum.Regular, item.winAmount, item.JPSymbolIfString));
                }
            }


            return spin;
        }
        public dynamic Spin(dynamic obj)

        {
            try
            {
                var denominations = obj.config.denominations;
                string newDenominations = denominations.ToString();
                var denominationsFromPlatform = JsonConvert.DeserializeObject<List<int>>(newDenominations);
                var stacks = obj.config.stakes;
                string newStacks = stacks.ToString();
                var stacksFromPlatform = JsonConvert.DeserializeObject<List<int>>(newStacks);
                //SpinPublicStateRequest spinRequest = Json.ConvertDynamic<SpinPublicStateRequest>(obj.publicState);
                //SpinPrivateState spinPrivateStateRequest = Json.ConvertDynamic<SpinPrivateState>(obj.privateState);

                if (_configs.IsTest && _context.RequestItems.cleanState.HasValue && _context.RequestItems.cleanState.Value == true)
                {
                    _context.State = new SpinPrivateState();
                    //spinPrivateStateRequest = new SpinPrivateState();
                    //spinPrivateStateRequest.completed = false;
                    //obj.privateState = Json.ObjectToDynamic(spinPrivateStateRequest);
                }

                int betAmount = 0, chipsPerPlay = 0;

                bool isFreeSpin = _context.RequestItems.isFreeSpin;
                var wins = new List<Win>();


                // Check if freeSpin or regular spin
                //isFreeSpin = spinRequest.action == ActionType.freespin.ToString();
                _context.State.freeSpinsLeft = _context.State.freeSpinsLeft == null ? 0 : _context.State.freeSpinsLeft;
                _context.State.totalFreeSpins = _context.State.totalFreeSpins == null ? 0 : _context.State.totalFreeSpins;
                if (_context.State.completed == true)
                {
                    _context.State.totalFreeSpins = 0;
                    _context.State.sumWinsFreeSpins = null;
                }
                long sumWinsFreeSpins = _context.State.sumWinsFreeSpins ?? 0;
                int freeSpinsExpandIndex = _context.State.totalFreeSpins.Value - _context.State.freeSpinsLeft.Value;
                _gameEngine.ValidateSpins();
                //If test enviroment check for force
                string force = "";
                //Get betAmount and ChipsPerPlay from public or private State
                CheckBetsAndChipsPerPlay(stacksFromPlatform, denominationsFromPlatform, ref betAmount, ref chipsPerPlay);
                Result result = null;
                bool goToRegularSpin = true;
                List<List<int>> spinResult = null;
                if (_context.RequestItems.force != null)
                {
                    spinResult = new List<List<int>>();
                    result = ForceSymbolsWithResult(_context.RequestItems.force, spinResult, isFreeSpin, out goToRegularSpin);
                }

                if (goToRegularSpin)
                {
                    result = _gameEngine.Spin(spinResult);
                }
                //Create spin object
                Spin spin, rootSpin;
                var fsToSave = 0;
                spin = rootSpin = CreateSpinObject(isFreeSpin, wins, result, _context.State, ref sumWinsFreeSpins);

                //Check privateState if there is a lastState
                if (_context.State.lastState != null && isFreeSpin)
                {
                    rootSpin = _context.State.lastState.spin;
                    rootSpin.totalSpinWin += spin.sumWins;
                    rootSpin.childFeature.Add(spin);
                }


                //Create PublicState
                var spinPublicResponse = new SpinPublicStateResponse();
                spinPublicResponse.action = isFreeSpin ? ActionType.freespin.ToString() : ActionType.spin.ToString();
                spinPublicResponse.betAmount = betAmount;
                spinPublicResponse.denom = chipsPerPlay;
                spinPublicResponse.spin = rootSpin;
                if (!isFreeSpin)
                {
                    _context.State.transactionId = Guid.NewGuid();
                }
                else
                {
                    rootSpin.sumWinsFreeSpins = _context.State.sumWinsFreeSpins = sumWinsFreeSpins;
                }

                rootSpin.freeSpinsLeft = _context.State.freeSpinsLeft;
                rootSpin.totalFreeSpins = _context.State.totalFreeSpins;
                spinPublicResponse.spin.complete = _context.State.completed.Value;
                _context.State.lastState = spinPublicResponse;


                //Debit user if goes into freeSpins or bonus win
                if (_context.State.completed.Value == false && isFreeSpin == false && (_context.State.lastState.spin.wins.Any(x => x.featureType == FeatureType.freeSpinWin.ToString()) || _context.State.BonusGame != null))
                {
                    obj.transactions = Json.ObjectToDynamic(new Transaction() { debits = new long[] { betAmount * chipsPerPlay } });
                }
                else if
                //If spin is freeSpin and ended onlycredit user
                (_context.State.completed.Value && isFreeSpin)
                {
                    obj.transactions = Json.ObjectToDynamic(new Transaction() { credits = new long[] { rootSpin.totalSpinWin.Value } });
                }
                else if //If spin is regular and ended credit and debit user
                (_context.State.completed.Value && !isFreeSpin)
                {
                    obj.transactions = Json.ObjectToDynamic(new Transaction() { credits = new long[] { rootSpin.totalSpinWin.Value }, debits = new long[] { betAmount * chipsPerPlay } });
                }

                obj.config = null;
                //Dont let json grow to much if many FS
                if (spinPublicResponse.spin.childFeature != null && spinPublicResponse.spin.childFeature.Where(x => x.type != "pick").Count() > 2)
                {
                    var a = spinPublicResponse.spin.childFeature.Where(x => x.type != "pick").First();
                    spinPublicResponse.spin.childFeature.Remove(a);
                }
                if (_context.State.lastState != null && _context.State.lastState.spin.childFeature != null && _context.State.lastState.spin.childFeature.Where(x => x.type != "pick").Count() > 2)
                {
                    var a = _context.State.lastState.spin.childFeature.Where(x => x.type != "pick").First();
                    _context.State.lastState.spin.childFeature.Remove(a);
                }

                obj.publicState = Json.ObjectToDynamic(spinPublicResponse);
                if (spinPublicResponse.spin.holdAndSpin != HoldAndSpin.None)
                {

                }
                if (_context.State.holdAndSpin != HoldAndSpin.None)
                {

                }
                obj.privateState = Json.ObjectToDynamic(_context.State);
            }
            catch (Exception ex)
            {
                var er = new ErrorObject() { error = new Error() { message = ex.Message, stackTrace = ex.StackTrace } };
                if (ex.Message == "Player has insufficient funds")
                {
                    er.error.code = 1003;
                }
                obj = Json.ObjectToDynamic(er);
            }
            return obj;
        }


    }
}
