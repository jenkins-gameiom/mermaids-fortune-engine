using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;

namespace AGS.Slots.MermaidsFortune.Logic.Engine.Math
{
    public static class Randomize
    {
        public static List<List<int>> RandomizeReels(this List<List<int>> reels, IRandom random)
        {
            List<List<int>> res = new List<List<int>>();
            int reelsSize = 4;
            List<RandomNumber> randomNumbers = new List<RandomNumber>();
            //for every reels, we generate object with min 0, max as num of items in reel (i.e 65), quantity 1.
            //we're gonna use this info in the GetRandomNumbers function.
            for (int i = 0; i < reels.Count; i++)
            {
                RandomNumber r = new RandomNumber();
                r.Min = 0;
                r.Max = reels[i].Count;
                r.Quantity = 1;
                randomNumbers.Add(r);
            }
            //gets ONE random number for every reel
            var realRandomNumbers = random.GetRandomNumbers(randomNumbers);

            //for every reel
            for (int i = 0; i < reels.Count; i++)
            {
                List<int> reelResult = new List<int>();
                List<int> currentReel = reels[i];
                int currentReelCount = currentReel.Count();
                //get the random number we did before
                int index = realRandomNumbers[i].Values[0];

                //get the symbol in the reel where the index is equal to the random number we got,
                //and get the two following numbers as well.
                for (int j = 0; j < reelsSize; j++)
                {
                    reelResult.Add(currentReel[(index + j) % currentReelCount]);

                }

                res.Add(reelResult);
            }

            return res;
        }
    }
}
