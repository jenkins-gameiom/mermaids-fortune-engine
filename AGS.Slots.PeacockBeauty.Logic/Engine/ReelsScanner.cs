using AGS.Slots.PeacockBeauty.Logic.Engine.Interfaces;
using System.Collections.Generic;

namespace AGS.Slots.PeacockBeauty.Logic
{
    public class LineReelsScanner : ReelsScanner<ItemOnReel>
    {
       
        Paylines _lines = new Paylines();
        private bool _scanBackWards;
        public LineReelsScanner(IPayoutResolver resolver, List<List<int>> winningLines, Dictionary<int, ItemOnReel> indices,bool scanBackWards) : base(resolver, winningLines[0].Count)
        {
            _scanBackWards = scanBackWards;
            for (int row = 0; row < winningLines.Count; row++)
            {
                List<ItemOnReel> line = new List<ItemOnReel>();
                for (int reelnumber = 0; reelnumber < winningLines[row].Count - 1; reelnumber++)
                {
                    line.Add(indices[winningLines[row][reelnumber]]);
                }
                line.Add(indices[winningLines[row][winningLines[row].Count - 1]]);
                _lines.AddLine(line);
            }
        }

       


        void Reset()
        {
            _result.ResetSequences();
        }

        public override void Scan(string force)
        {
            foreach (var line in _lines.PayLines)
            {
                for (int i = 0; i < line.Count; i++)
                {
                    if (!HandleItem(line[i],  false))
                    {
                        break;
                    }
                }
                _resolver.EvaluateSequence(_result);
                Reset();
            }

            if (ScanBackWards)
            {
                foreach (var line in _lines.PayLines)
                {
                    for (int i = line.Count - 1; i > -1 ; i--)
                    {
                        if (!HandleItem(line[i], true))
                        {
                            break;
                        }
                    }
                    _resolver.EvaluateSequence(_result);
                    Reset();
                }
            }
        }

        public override bool ScanBackWards
        {
            get { return _scanBackWards; }

        }
    }
}
