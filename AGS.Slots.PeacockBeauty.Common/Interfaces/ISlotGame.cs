using System;

namespace AGS.Slots.PeacockBeauty.Common.Interfaces
{
    public interface ISlotGame
    {
        dynamic InitSlot(dynamic obj);
        dynamic Spin(dynamic obj);

        dynamic Pick(dynamic obj);

    }
}
