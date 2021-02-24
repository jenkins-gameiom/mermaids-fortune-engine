using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using System.Collections.Generic;

namespace AGS.Slots.MermaidsFortune.Logic
{
    public class Paylines
    {
        public List<List<ItemOnReel>> PayLines { get; set; }

        public void AddLine(List<ItemOnReel> line)
        {
            if (PayLines == null)
            {
                PayLines = new List<List<ItemOnReel>>();
            }
            PayLines.Add(line);

        }

        List<ItemOnReel> ReverseLine(List<ItemOnReel> line)
        {
            List<ItemOnReel> newline = new List<ItemOnReel>();
            for (int i = line.Count - 1; i > -1; i--)
            {
                newline.Add(line[i]);
            }
            return newline;
        }


    }
}
