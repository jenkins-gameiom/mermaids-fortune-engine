using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGS.Slots.PeacockBeauty.Logic.Engine.Interfaces
{
  

    public class RandomNumber
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int Quantity { get; set; }
        public List<int> Values { get; set; }
    }
    public interface IRandom
    {
        List<RandomNumber> GetRandomNumbers(List<RandomNumber> rnds);

        int Next(int minValue, int maxValue);

        double NextPercentage();
    }
}
