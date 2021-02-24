using System;
using System.Collections.Generic;
using System.Text;

namespace AGS.Slots.MermaidsFortune.Common.Interfaces
{
    public interface IBonusGameService
    {
        void SpinAll();
        long GetCashWon();
        void EndBonus();
    }
}
