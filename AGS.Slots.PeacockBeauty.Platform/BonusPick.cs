using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic;
using AGS.Slots.MermaidsFortune.Logic.Engine;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;
using Autofac.Features.Indexed;
using Newtonsoft.Json;

namespace AGS.Slots.MermaidsFortune.Platform
{
    public class BonusPick
    {
        private readonly GameEngine _engineLogic;
        private readonly IBonusGameService _bonusGameService;
        private readonly IRequestContext _context;
        public BonusPick(IRequestContext context,  IBonusGameService bonusGameService)
        {
            _context = context;
            
            _bonusGameService = bonusGameService;
        }

        public dynamic Pick(dynamic obj)
        {
            try
            {
                SpinPublicStateRequest spinRequest = Json.ConvertDynamic<SpinPublicStateRequest>(obj.publicState);
                SpinPrivateState spinPrivateStateRequest = Json.ConvertDynamic<SpinPrivateState>(obj.privateState);

                // Validate has bonusGame in private state
                if (_context.State.BonusGame == null)//spinPrivateStateRequest.bonusGame == null
                {
                    throw new Exception("Error in validating bonus game reason is spinPrivateStateRequest.bonusGame == null");
                }
                //if (spinPrivateStateRequest.bonusGame.winAmount == 0)//spinPrivateStateRequest.bonusGame.winAmount == 0
                //{
                //    throw new Exception("Error in validating bonus game reason is spinPrivateStateRequest.bonusGame.winAmount == 0");
                //}
                //if (spinPrivateStateRequest.bonusGame.MCSymbols.Count() == 0)//spinPrivateStateRequest.bonusGame.MCSymbols.Count() == 0
                //{
                //    throw new Exception("Error in validating bonus game reason is spinPrivateStateRequest.bonusGame.MCSymbols.Count()==0");
                //}

                SpinPublicStateResponse lastPublicState = spinPrivateStateRequest.lastState;


                if (lastPublicState != null && lastPublicState.spin != null && !lastPublicState.spin.complete && !_context.State.completed.Value)
                {
                    var lastChildSpin = lastPublicState.spin.childFeature.LastOrDefault();
                    if (!(lastChildSpin != null && lastChildSpin.wins != null && lastChildSpin.wins.Any(x => x.featureType == FeatureType.pick.ToString()))) //Was on a root Spin
                    {
                        if (lastPublicState.spin.wins.Any(x => x.featureType == FeatureType.pick.ToString())) //Make sure there was a bonus win
                            lastChildSpin = lastPublicState.spin;
                        else
                            throw new Exception("Error in validating bonus game");
                    }


                    Spin spn = new Spin();
                    spn.type = SpinType.pick.ToString();
                    //spn.index = spinRequest.index.Value;



                    //retrieve saved Bonus game

                    _bonusGameService.SpinAll();
                    BuildPublicResponse(spn);
                    if (lastChildSpin.childFeature.Count() > 2 && lastChildSpin.childFeature.All(x => x.type != "jackpotPick"))
                    {
                        lastChildSpin.childFeature.RemoveAt(0);
                    }
                    lastChildSpin.childFeature.Add(spn);
                    spinPrivateStateRequest.lastState = lastPublicState;
                    _context.State.completed = _context.State.BonusGame.complete && spn.complete && !spinPrivateStateRequest.lastState.spin.wins.Any(x => x.featureType == FeatureType.freeSpinWin.ToString());
                    lastPublicState.spin.complete = _context.State.completed.Value;
                    if (spn.complete)
                    {
                        if (spn.cashWon == null)
                        {
                            spn.cashWon = 0;
                        }

                        var bonusWinAmount = _bonusGameService.GetCashWon();
                        _bonusGameService.EndBonus();
                        spn.cashWon += bonusWinAmount;
                        lastPublicState.spin.totalSpinWin += bonusWinAmount;
                    }
                    else
                    {
                        spn.cashCurrentlyAwarded = _bonusGameService.GetCashWon();
                    }
                    //Only credit as the debit is done on the root spin
                    if (_context.State.completed.Value)
                    {
                        obj.transactions = Json.ObjectToDynamic(new Transaction() { credits = new long[] { lastPublicState.spin.totalSpinWin.Value } });
                        lastPublicState.spin.complete = true;
                    }

                    obj.config = null;
                    lastPublicState.action = SpinType.pick.ToString();
                    obj.publicState = Json.ObjectToDynamic(lastPublicState);
                    obj.privateState = Json.ObjectToDynamic(_context.State);

                }
                else
                {
                    throw new Exception("Error in validating bonus game");
                }
            }
            catch (Exception ex)
            {
                obj = Json.ObjectToDynamic(new ErrorObject() { error = new Error() { message = ex.Message, stackTrace = ex.StackTrace } });
            }

            return obj;
        }

        private void BuildPublicResponse(Spin spn)
        {
            spn.MCSymbols = new List<Common.Entities.MCSymbol>();
            foreach (var item in _context.State.BonusGame.MCSymbols)
            {
                var mcSymbol = new MCSymbol(item.index, item.coordinate.Item1, item.coordinate.Item2, item.symbol, item.IsLocked, item.Type, item.winAmount);
                spn.MCSymbols.Add(mcSymbol);
            }
            spn.freeSpinsLeft = _context.State.BonusGame.fsLeft;
            spn.freeSpinsspun = _context.State.BonusGame.fsDone;
            spn.complete = _context.State.BonusGame.complete;
        }
    }
}

