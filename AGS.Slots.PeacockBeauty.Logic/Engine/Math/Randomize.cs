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
        public static List<List<int>> RandomizeReels(this List<List<int>> reels, List<int> reelsLayout, List<int> selectedIndexes)
        {
            List<List<int>> res = new List<List<int>>();
            for (int i = 0; i < reels.Count; i++)
            {
                List<int> reelResult = new List<int>();
                List<int> currentReel = reels[i];
                int currentReelCount = currentReel.Count();
                int index = selectedIndexes[i];

                for (int j = 0; j < reelsLayout[i]; j++)
                {
                    reelResult.Add(currentReel[(index + j) % currentReelCount]);

                }
                res.Add(reelResult);
            }
            return res;
        }
    }
}
