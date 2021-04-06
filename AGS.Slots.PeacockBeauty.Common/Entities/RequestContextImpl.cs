using System;
using System.Collections.Generic;
using System.Text;
using AGS.Slots.MermaidsFortune.Common.Interfaces;

namespace AGS.Slots.MermaidsFortune.Common.Entities
{
    public class RequestConextImpl : IRequestContext
    {
        public IStateItems State { get; set; }
        public Common.Entities.Config Config { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IMathFile MathFile { get; set; }
        public RequestItems RequestItems { get; set; }
        public int GetDenom()
        {
            if (RequestItems.isFreeSpin)
            {
                return State.lastState.denom.Value;
            }
            else
            {
                return RequestItems.denom;
            }
        }
    
        public int GetBetAmount()
        {
            if (RequestItems.isFreeSpin)
            {
                return State.lastState.betAmount.Value;
            }
            else
            {
                return RequestItems.betAmount;
            }
        }
    }
}
