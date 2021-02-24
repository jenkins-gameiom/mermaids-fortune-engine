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
        //after we got the results and the wins, 
        public void EvaluateResult(Result result)
        {
            bool isFreeSpins = _context.RequestItems.isFreeSpin;
            List<int> betSteps = _context.Config.stakes;
            var mutliPliers = _context.MathFile.GetLookupPaytable();
            //TODO - check thats how you calculate treasurechestlevel
            _context.State.TreasureChestTurnOver += _context.GetBetAmount() * _context.GetDenom();
            if (result.Reels.SelectMany(x => x).ToList().Any(x => x == 0))
            {
                _context.MathFile.GetCurrentLongTermPersistence(_context.State);
            }
            var betLevel = betSteps.IndexOf(_context.GetBetAmount());
            AssignMCSymbolsToExisting(result);
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
                    _context.State.totalFreeSpins += win.GrantedFreeSpins;
                    var multiPlier = mutliPliers[win.Symbol];
                    var currentLineWinAmount = (long)multiPlier[result.Scatter.Count() - 2] * _context.GetBetAmount() * 1 *
                        _context.GetDenom() / betSteps[0];
                    win.WinAmount += currentLineWinAmount;
                    result.WonAmount += currentLineWinAmount;
                }
                else if (win.WinType == WinType.JackPot)
                {
                    SetJackpotGame(result);
                    
                }
                //we got 6+ mc-symbols.
                else if (IsMCSymbol(win.WinningLines.First()))
                {
                    SetBonusGame(result.McSymbols);
                }
                else
                {
                    //Regular win
                    var multiPlier = mutliPliers[win.Symbol];
                    var currentLineWinAmount = (long)multiPlier[win.LongestSequence - 2] * _context.GetBetAmount() * win.Ways *
                        _context.GetDenom() / betSteps[0];
                    win.WinAmount += currentLineWinAmount;
                    result.WonAmount += currentLineWinAmount;
                }
            }
            _context.State.completed = _context.State.freeSpinsLeft <= 0 && _context.State.BonusGame == null && _context.State.JackpotGame == null && result.Wins.All(x => x.GrantedFreeSpins == 0);
        }

        private void SetJackpotGame(Result result)
        {
            result.JackpotGame = new JackpotGame();
            result.JackpotGame.outcome = _context.MathFile.GetJackpotBonusOutcome(_random);
            var listOfValues = _context.MathFile.GetRandomJackpotCombination(result.JackpotGame.outcome, _random);
            result.JackpotGame.leftItems = listOfValues.Count;
            foreach (var x in listOfValues)
            {
                result.JackpotGame.selectedItems.Add(new JackpotItem()
                {
                    Symbol = x
                });
            }
            SetJackpotToContext(result.JackpotGame, _context.State);
            
        }

        private void SetJackpotToContext(JackpotGame resultJackpotGame, IStateItems contextState)
        {
            _context.State.JackpotGame = new JackpotGame();
            _context.State.JackpotGame.outcome = resultJackpotGame.outcome;
            _context.State.JackpotGame.leftItems = resultJackpotGame.selectedItems.Count;
            foreach (var x in resultJackpotGame.selectedItems)
            {
                _context.State.JackpotGame.selectedItems.Add(new JackpotItem()
                {
                    Symbol = x.Symbol
                });
            }
            _context.State.TreasureChestTurnOver = 0;
        }

        private void SetBonusGame(List<ItemOnReel> mcSymbols)
        {
            _context.State.BonusGame = new BonusGame();
            _context.State.BonusGame.bet = _context.GetBetAmount();
            _context.State.BonusGame.denom = _context.GetDenom();
            _context.State.BonusGame.MCSymbols = new List<MCSymbol>();
            foreach (var x in mcSymbols)
            {
                MCSymbol mcSymbol = new MCSymbol(x.Index, x.Reel, x.Coordinate.Item2, 13, true, TableTypeEnum.Regular);
                mcSymbol.winAmount = int.Parse(x.MCSymbol) * _context.GetDenom();
                _context.State.BonusGame.MCSymbols.Add(mcSymbol);
            }
        }


        //this method assign value (500, "MINI" etc) to all the mcsymbols (items with value 13)
        private void AssignMCSymbolsToExisting(Result result)
        {
            var betLevel = _context.Config.stakes.IndexOf(_context.GetBetAmount());
            foreach (var item in result.McSymbols)
            {
                item.MCSymbol = _context.MathFile.MoneyChargeSymbol(betLevel, 0, _random);
            }
        }

        public const int SCATTER = 12;
        public const int WILD = 0;
        public const int MCSymbol = 13;
        public bool IsWildCard(ItemOnReel item)
        {
            return item.Symbol == WILD;
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
