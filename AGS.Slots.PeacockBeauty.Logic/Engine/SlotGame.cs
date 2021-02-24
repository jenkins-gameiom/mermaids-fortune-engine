using AGS.Slots.PeacockBeauty.Logic.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGS.Slots.PeacockBeauty.Logic
{
    public abstract class SlotGame
    {

        protected abstract List<List<int>> ReelsIndexList { get; }

        public abstract Result Spin(Bet bet, IRandom random, ISlotConfig config, List<List<int>> spinResult = null, int reelSize = 3);

        protected abstract void Init();

        public ISlotConfig Config96 { get; set; }

        public ISlotConfig Config94 { get; set; }
        public abstract ISlotConfig GetConfig(string math = null);

    }
}
