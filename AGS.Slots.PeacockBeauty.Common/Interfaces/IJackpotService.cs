using System;
using System.Collections.Generic;
using System.Text;

namespace AGS.Slots.MermaidsFortune.Common.Interfaces
{
    public interface IJackpotService
    {

        string HandleJackpot();
        long GetCashWonAndEndJackpot();
    }
}
