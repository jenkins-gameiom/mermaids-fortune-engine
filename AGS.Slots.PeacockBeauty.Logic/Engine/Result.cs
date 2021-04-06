using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Collections;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;
using Autofac.Features.Indexed;

namespace AGS.Slots.MermaidsFortune.Logic
{
    public class Bet
    {
        public int BetAmount { get; set; }
        public int Chipsperplay { get; set; }
        public bool IsFreeSpin { get; set; }
        public int BonusLevel { get; set; }
        public string Force { get; set; }

        public Bet()
        {
        }
    }
    public class Result
    {

        public List<List<ItemOnReel>> WinningLines { get; set; }
        private List<ItemOnReel> _sequence;
        private List<ItemOnReel> _scatter;
        private List<ItemOnReel> _wildCards;
        private List<ItemOnReel> _mcSymbols;
        private List<ItemOnReel> _nonMcSymbols;

        public long WonAmount { get; set; }

        //public JackpotGame JackpotGame { get; set; }

        public List<Win> Wins { get; set; }
        public List<List<int>> Reels { get; set; }

        public Result()
        {
            _sequence = new List<ItemOnReel>();
            _scatter = new List<ItemOnReel>();
            _wildCards = new List<ItemOnReel>();
            _mcSymbols = new List<ItemOnReel>();
            _nonMcSymbols = new List<ItemOnReel>();
            WinningLines = new List<List<ItemOnReel>>();
        }

        public List<ItemOnReel> Sequence
        {
            get
            {
                return _sequence;

            }
        }

        public void ResetSequences()
        {
            _sequence.Clear();
        }


        public void AddCurrentSequence()
        {
            ItemOnReel[] clone = new ItemOnReel[Sequence.Count];
            Sequence.CopyTo(clone);
            WinningLines.Add(new List<ItemOnReel>(clone));
        }

        public string PrintWinningLines()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var line in WinningLines)
            {
                int i = 0;
                foreach (var item in line)
                {
                    sb.Append(string.Format("[{0}]({1})", item.Index, item.Symbol));
                    if (i < line.Count - 1)
                        sb.Append("-");
                    i++;
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        public List<ItemOnReel> Scatter
        {
            get { return _scatter; }
        }
        public List<ItemOnReel> McSymbols
        {
            get { return _mcSymbols; }
        }
        public List<ItemOnReel> WildCards
        {
            get { return _wildCards; }
        }

        public List<ItemOnReel> NonMcSymbols
        {
            get { return _nonMcSymbols; }
        }
        public ItemOnReel LastSequenceItem
        {
            get
            {
                if (_sequence.Count == 0)
                    return null;
                return _sequence[_sequence.Count - 1];

            }
        }

    }

    public enum WinType
    {
        Regular,
        FreeSpin,
        FiveOfAKind
    }

    public class Win
    {
        public WinType WinType { get; set; }
        public long WinAmount { get; set; }
        public int Ways { get; set; }
        public HashSet<ItemOnReel> WinningLines { get; set; }
        public int GrantedFreeSpins { get; set; }
        public int Symbol { get; set; }

        [JsonIgnore]
        public int LongestSequence
        {
            get
            {
                return CalcLongestSequence(WinningLines);
            }

        }

        public static int CalcLongestSequence(HashSet<ItemOnReel> itemsOnReels)
        {

            int ret = 0;
            foreach (var item in itemsOnReels)
            {
                if (item.Symbol == MermaidsFortuneResolver.SCATTER)
                    return itemsOnReels.Count;

                ret = System.Math.Max(ret, item.Reel);
            }
            return ret + 1;
        }
    }
}

