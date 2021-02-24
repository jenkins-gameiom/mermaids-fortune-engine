using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGS.Slots.PeacockBeauty.Commons
{
    public static class GameExtentionMethods
    {
        public static string JackpotNameBySymbol(this int symbol)
        {
            if (symbol == 1)
                return ("mini");
            else if (symbol == 2)
                return ("midi");
            else if (symbol == 3)
                return ("maxi");
            else if (symbol == 4)
                return ("grand");

            return "";
        }
    }
}
