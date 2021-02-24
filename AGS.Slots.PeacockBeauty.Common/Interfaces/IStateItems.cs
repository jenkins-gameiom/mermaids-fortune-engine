using System;
using System.Collections.Generic;
using System.Text;
using AGS.Slots.MermaidsFortune.Common.Entities;

namespace AGS.Slots.MermaidsFortune.Common.Interfaces
{
    public interface IStateItems
    {

        public string state { get; set; }
        public bool? completed { get; set; }
        public int animationState { get; set; }
        public int? freeSpinsLeft { get; set; }
        public int? totalFreeSpins { get; set; }
        public long? sumWinsFreeSpins { get; set; }
        public int TreasureChestTurnOver { get; set; }
        public int TreasureChestTurnState { get; set; }
        public Guid? transactionId { get; set; }
        public string userName { get; set; }
        public string sessionId { get; set; }
        public string mode { get; set; }
        public BonusGame BonusGame { get; set; }
        public JackpotGame JackpotGame { get; set; }
        public SpinPublicStateResponse lastState { get; set; }


    }
}
