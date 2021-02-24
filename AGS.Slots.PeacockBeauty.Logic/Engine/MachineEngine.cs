using AGS.Slots.PeacockBeauty.Logic.Engine.Interfaces;
using System;
using System.Collections.Generic;

namespace AGS.Slots.PeacockBeauty.Logic
{
    public class MachineEngine
    {
        private readonly ReelsScanner<ItemOnReel> _scanner;

        public MachineEngine(ReelsScanner<ItemOnReel> scanner)
        {
            _scanner = scanner;
        }


        public void Scan(string force)
        {
            _scanner.Scan(force);
        }



        public Result GetResult()
        {
            return _scanner.Result;
        }

        public Result CalculateResult(Bet bet,List<List<int>> resultedReels,IRandom random)
        {
            _scanner.Result.Reels = resultedReels;
            _scanner.EvaluateResult(bet,random);
          
            return _scanner.Result;
        }

       

        public void ClearResult()
        {
            _scanner.Result = new Result();
        }

    }
}
