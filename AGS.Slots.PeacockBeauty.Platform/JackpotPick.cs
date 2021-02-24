using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic;
using AGS.Slots.MermaidsFortune.Logic.Engine;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using Autofac.Features.Indexed;

namespace AGS.Slots.MermaidsFortune.Platform
{
    public class JackpotPick
    {
        private readonly GameEngine _engineLogic;
        private readonly IRequestContext _context;
        private readonly IJackpotService _jackpotService;
        public JackpotPick(IRequestContext context, IJackpotService jackpotService)
        {
            _context = context;
            _jackpotService = jackpotService;
        }

        public dynamic PickJackpot(dynamic obj)
        {
            try
            {
                SpinPublicStateRequest spinRequest = Json.ConvertDynamic<SpinPublicStateRequest>(obj.publicState);
                SpinPrivateState spinPrivateStateRequest = Json.ConvertDynamic<SpinPrivateState>(obj.privateState);

                //Validate has jackpotGame in private state
                SpinPublicStateResponse lastPublicState = spinPrivateStateRequest.lastState;

                var lastChildSpin = ValidateJackpotAndGetLastChildFeature(spinPrivateStateRequest);
                _context.State.JackpotGame.index = spinRequest.index;

                Spin spn = new Spin();
                spn.type = SpinType.jackpotPick.ToString();
                spn.index = spinRequest.index;
                spn.value = _jackpotService.HandleJackpot();
                spn.complete = _context.State.JackpotGame.complete;
                lastChildSpin.childFeature.Add(spn);

                if (spn.complete)
                {
                    spn.cashWon = _jackpotService.GetCashWonAndEndJackpot();
                    lastPublicState.spin.totalSpinWin += spn.cashWon;
                }
                //Only credit as the debit is done on the root spin
                if (_context.State.completed.Value)
                {
                    obj.transactions = Json.ObjectToDynamic(new Transaction() { credits = new long[] { lastPublicState.spin.totalSpinWin.Value } });
                }

                obj.config = null;
                lastPublicState.action = SpinType.jackpotPick.ToString();
                lastPublicState.spin.complete = _context.State.completed.Value;
                obj.publicState = Json.ObjectToDynamic(lastPublicState);
                obj.privateState = Json.ObjectToDynamic(_context.State);
            }
            catch (Exception ex)
            {
                obj = Json.ObjectToDynamic(new ErrorObject() { error = new Error() { message = ex.Message, stackTrace = ex.StackTrace } });
            }

            return obj;
        }

        private Spin ValidateJackpotAndGetLastChildFeature(SpinPrivateState spinPrivateStateRequest)
        {
            Spin lastChildSpin = null;
            if (_context.State.JackpotGame == null)
            {
                throw new Exception("Error in validating jackpot game");
            }
            SpinPublicStateResponse lastPublicState = spinPrivateStateRequest.lastState;

            if (lastPublicState != null && lastPublicState.spin != null &&
                !lastPublicState.spin.complete && !spinPrivateStateRequest.completed.Value)
            {
                lastChildSpin = lastPublicState.spin.childFeature.LastOrDefault();
                if (!(lastChildSpin != null && lastChildSpin.wins != null && lastChildSpin.wins.Any(x => x.featureType == FeatureType.jackpot.ToString()))) //Was on a root Spin
                {
                    if (lastPublicState.spin.wins.Any(x => x.featureType == FeatureType.jackpot.ToString()))
                    {
                        lastChildSpin = lastPublicState.spin;
                    }
                    else
                    {
                        throw new Exception("Error in validating jackpot game");
                    }
                    //Make sure there was a jackpot win
                }
            }
            else
            {
                throw new Exception("Error in validating jackpot game");
            }
            return lastChildSpin;
        }

        static int[] mcWeightsReturnValue = new int[] { 1, 1, 1, 1, 0, 0, 0, 1, 2, 2 };
    }
}
