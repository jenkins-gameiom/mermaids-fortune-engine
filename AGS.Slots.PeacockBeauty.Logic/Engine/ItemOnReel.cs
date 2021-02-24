using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AGS.Slots.MermaidsFortune.Logic
{
    public class ItemOnReel
    {
        public int Index { get; set; }
        public int Symbol { get; set; }
        public int Reel { get; set; }
        public Tuple<int, int> Coordinate { get; set; }
        public string MCSymbol { get; set; }

        public static int HitCount { get; set; }

        public bool CanBeSubForWinSymbol(int winSymbol)
        {
            var dd = Symbol;
            if (dd == 0)
            {
                //Wild can replace all except WILD and BN
                //TODO - check which symbols can wild replace, from the excel it looks like it can replace everything?
                //if (new int[]{1,2,3,4,5}.Contains(Symbol))
                return true;
            }

            if (dd == 11)//to check if BW is a wild in addition to BN
            {
                //Wild can replace all except WILD and BN
                //TODO - check which symbols can wild replace, from the excel it looks like it can replace everything?
                //if (new int[]{1,2,3,4,5}.Contains(Symbol))
                return true;
            }
            if (dd == winSymbol)
                return true;
            //if (winSymbol >= 5 && winSymbol <= 13)
            //    return (Symbol - 11) == winSymbol;
            //if (winSymbol >= 1 && winSymbol <= 4)
            //    return (Symbol >= 1 && Symbol <= 4);
            //if (winSymbol >= 16 && winSymbol <= 24)
            //    return (Symbol + 11) == winSymbol;

            return false;
        }

        public override bool Equals(object obj)
        {
            //return true;
            if (obj is Tuple<int, int>)
            {
                return this.Index == (obj as Tuple<int, int>).Item1 * 5 + 1 + (obj as Tuple<int, int>).Item2;
            }
            return this.Index == ((ItemOnReel)obj).Index;
        }

        public override int GetHashCode()
        {
            return Index;
        }



    }



    public class ReelItemComparer : IEqualityComparer<ItemOnReel>
    {
        public bool Equals(ItemOnReel x, ItemOnReel y)
        {
            return x.Symbol == y.Symbol;
        }

        public int GetHashCode(ItemOnReel obj)
        {
            return obj.GetHashCode();
        }
    }
}
