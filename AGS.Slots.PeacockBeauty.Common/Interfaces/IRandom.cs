using System;
using System.Collections.Generic;
using System.Text;

namespace AGS.Slots.MermaidsFortune.Common.Interfaces
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
