using System;
using System.Collections.Generic;
using System.Text;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using Autofac.Features.Indexed;

namespace AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune
{
    public class JackpotService : IJackpotService
    {
        private readonly IRequestContext _context;
        private readonly Configs _applicationConfig;

        public JackpotService(IRequestContext context)
        {
            _context = context;
        }
        public string HandleJackpot()
        {
            var symbolToReturn = _context.State.JackpotGame.selectedItems[_context.State.JackpotGame.selectedItems.Count - _context.State.JackpotGame.leftItems]
                .Symbol;
            _context.State.JackpotGame.leftItems--;
            _context.State.completed = _context.State.BonusGame == null ?  _context.State.JackpotGame.complete : _context.State.BonusGame.complete && _context.State.JackpotGame.complete;
            return symbolToReturn;
        }

        private long getWinAmount(string outcome)
        {
            long totalWinAmount = 0;
            var charArray = outcome.ToCharArray();
            foreach (var c in charArray)
            {
                totalWinAmount += _context.MathFile.GetProgressiveInformation()[int.Parse(c.ToString())];
            }
            return totalWinAmount;
        }

        public long GetCashWonAndEndJackpot()
        {
            var amountToReturn = getWinAmount(_context.State.JackpotGame.outcome) * _context.GetDenom();
            _context.State.JackpotGame = null;
            return amountToReturn;
        }
    }
}
