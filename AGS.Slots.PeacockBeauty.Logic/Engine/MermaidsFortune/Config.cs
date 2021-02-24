using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.Math;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;
using Autofac.Features.Indexed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune
{
    public class Config : IMathFile
    {
        private dynamic config;

        private List<List<int>> lookup_table_symbols = null;
        private List<List<int>> prize_reel_strip = null;
        private List<List<int>> special_reel_strip = null;
        private List<List<int>> progressive_information = null;
        private List<ReelItemJackpot> base_prize = null;
        private List<ReelItemJackpotString> jackpot_bonus_outcome = null;
        private List<Dictionary<string, List<string>>> jackpot_bonus_reveal = null;
        private List<ReelItemJackpot> prize_spin_prize = null;
        private List<List<int>> lookup_paytable = null;
        private List<int[]> base_jackpot_bonus_trigger = null;
        private List<int[]> fg_jackpot_bonus_trigger = null;
        private List<int> betsteps = null;
        private List<int> denoms = null;
        private List<int> long_term_persistence = null;

        public Config()
        {

        }

        public Config(string mathName)
        {
            
            LoadConfigFromRM(mathName);
            Populate();
        }

        private void Populate()
        {
            lookup_table_symbols = config.lookup_table_symbols.ToObject<List<List<int>>>();
            prize_reel_strip = config.prize_reel_strip.ToObject<List<List<int>>>();
            special_reel_strip = config.special_reel_strip.ToObject<List<List<int>>>();
            base_prize = config.base_prize.ToObject<List<ReelItemJackpot>>();
            foreach (var reelItems in base_prize)
            {
                AggregateArray(reelItems.weights);
            }
            prize_spin_prize = config.prize_spin_prize.ToObject<List<ReelItemJackpot>>();
            foreach (var reelItems in prize_spin_prize)
            {
                AggregateArray(reelItems.weights);
            }
            progressive_information = config.progressive_information.ToObject<List<List<int>>>();
            base_jackpot_bonus_trigger = config.base_jackpot_bonus_trigger.ToObject<List<int[]>>();
            foreach (var reelItems in base_jackpot_bonus_trigger)
            {
                AggregateArray(reelItems);
            }
            jackpot_bonus_outcome = config.jackpot_bonus_outcome.ToObject<List<ReelItemJackpotString>>();
            foreach (var reelItems in jackpot_bonus_outcome)
            {
                AggregateArray(reelItems.weights);
            }
            jackpot_bonus_reveal = config.jackpot_bonus_reveal.ToObject<List<Dictionary<string, List<string>>>>();
            fg_jackpot_bonus_trigger = config.fg_jackpot_bonus_trigger.ToObject<List<int[]>>();
            foreach (var reelItems in fg_jackpot_bonus_trigger)
            {
                AggregateArray(reelItems);
            }
            lookup_paytable = config.lookup_paytable.ToObject<List<List<int>>>();
            betsteps = config.betsteps.ToObject<List<int>>();
            denoms = config.denoms.ToObject<List<int>>();
            long_term_persistence = config.long_term_persistence.ToObject<List<int>>();

        }

        public bool WildTriggerJackpot(bool isFreeSpin, int betLevel, IRandom random = null)
        {

            if (random != null)
            {
                int[] val = null;
                int valueToFind = 0;
                if (isFreeSpin)
                {
                    val = fg_jackpot_bonus_trigger[betLevel];
                }
                else
                {
                    val = base_jackpot_bonus_trigger[betLevel];
                }
                valueToFind = random.Next(0, val[val.Length - 1]);
                var selected = RandomWeightedIndex(val.ToArray(), valueToFind);
                if (selected == 0)
                {
                    return true;
                }
            }
            return false;
        }


        private static void AggregateArray(int[] arr)
        {
            for (int i = 1; i < arr.Count(); i++)
            {
                arr[i] = arr[i - 1] + arr[i];
            }
        }

        static object locker = new object();
        void LoadConfigFromRM(string mathName)
        {
            if (config == null)
            {
                lock (locker)
                {
                    if (config == null)
                    {

                        string currentDirectory = Directory.GetCurrentDirectory();
                        string filePath = currentDirectory + "\\" + mathName + ".json";
                        config = JObject.Parse(File.ReadAllText(filePath));

                    }
                }
            }
        }

        public List<List<int>> GetReels()
        {
            return lookup_table_symbols;
        }

        public List<int> GetPrizeReelStrip()
        {
            return prize_reel_strip[0];

        }

        public List<int> GetSpecialReelStrip(RemoveX2Enum removeX2, bool removeX8)
        {
            if (removeX2 == RemoveX2Enum.Both)
            {

            }
            var newList = special_reel_strip[0].ToList();
            if (removeX2 != RemoveX2Enum.None)
            {

                if (removeX2 == RemoveX2Enum.First)
                {
                    newList.Remove(15);
                }
                if (removeX2 == RemoveX2Enum.Second)
                {
                    newList.RemoveAt(newList.FindLastIndex(x => x == 15));
                }
                if (removeX2 == RemoveX2Enum.Both)
                {
                    newList.RemoveAll(r => r == 15);
                }

            }
            if (removeX8)
            {
                newList.RemoveAll(r => r == 16);
            }
            return newList;
        }


        public int GetHoldSpinSymbolValue(int betAmount, TableTypeEnum type, int symbol, out string jackpotSymbol, IRandom random)
        {
            jackpotSymbol = symbol == 13 ? "prize" : symbol == 14 ? "paid" : symbol == 15 ? "x2" : symbol == 16 ? "x8" : null;
            int betIndex = 0;
            if (type == TableTypeEnum.Regular)
            {
                betIndex = BetSteps.IndexOf(betAmount);
                return GetPrizeSpinPrize(betIndex, random);
            }
            else //(type == TableTypeEnum.Special)
            {
                //TODO I think that in special if you win paid, mx2, mx8 you dont get money but a prize (multiply 2 or 8 or pay now). so I dont return money
                return 0;
            }
        }

        public List<string> GetRandomJackpotCombination(string outcome, IRandom random)
        {
            var allOptionsForKey = jackpot_bonus_reveal[0].First(x => x.Key == outcome).Value;
            var temp = random.Next(0, 2);
            var valueToFind = random.Next(0, allOptionsForKey.Count);
            var zz = allOptionsForKey[valueToFind];
            var listToReturn = new List<string>();
            for (int i = 0; i < zz.Length; i++)
            {
                if (zz[i] == '0')
                {
                    listToReturn.Add("mini");
                }
                if (zz[i] == '1')
                {
                    listToReturn.Add("minor");
                }
                if (zz[i] == '2')
                {
                    listToReturn.Add("major");
                }
                if (zz[i] == '3')
                {
                    listToReturn.Add("grand");
                }
                if (zz[i] == '4')
                {
                    listToReturn.Add("wild");
                }
            }

            return listToReturn;
        }

        public List<int> GetProgressiveInformation()
        {
            return progressive_information[0];
        }

        public int BetStepsDevider
        {
            get
            {
                return 88;
            }
        }

        public int GetJackpotItemWithoutRandomizing(TableTypeEnum type, int valueToFind, RemoveX2Enum removeX2, bool removeX8, out bool isJackpot, string force = null)
        {
            isJackpot = false;
            List<int> reelItems;
            if (type == TableTypeEnum.Regular)
            {
                reelItems = GetPrizeReelStrip();
                var val = reelItems[valueToFind];
                if (val == 13)
                {
                    isJackpot = true;
                }
                return reelItems[valueToFind];
            }
            else if (type == TableTypeEnum.Special)
            {
                reelItems = GetSpecialReelStrip(removeX2, removeX8);
                var val = reelItems[valueToFind];
                if (new int[] { 14, 15, 16 }.Contains(val))
                {
                    isJackpot = true;
                }
                if (force != null)
                {

                    if (force == "paid")
                    {
                        isJackpot = true;
                        return 14;
                        
                    }
                    if (force == "x2")
                    {
                        isJackpot = true;
                        return 15;
                    }
                    if (force == "x8")
                    {
                        isJackpot = true;
                        return 16;
                    }
                }
                return reelItems[valueToFind];
            }
            return -1;
        }

        public string GetProgressiveValueFromNumber(int number)
        {
            switch (number)
            {
                case 1:
                    return "Wild";
                    break;
                case 2:
                    return "Mini";
                    break;
                case 3:
                    return "Minor";
                    break;
                case 4:
                    return "Major";
                    break;
                case 5:
                    return "Grand";
                    break;
                default:
                    return "";
                    break;
            }
        }

        public int GetBasePrize(int betLevel, IRandom random)
        {
            int valueToFind = random.Next(0, base_prize[betLevel].weights[base_prize[betLevel].weights.Length - 1]);
            int selected = RandomWeightedIndex(base_prize[betLevel].weights, valueToFind);
            return base_prize[betLevel].outcome[selected];
            //return selected;
        }

        public ReelItemJackpot GetAllBasePrizes(int betLevel)
        {
            return base_prize[betLevel];
        }

        public int GetPrizeSpinPrize(int betLevel, IRandom random)
        {
            int valueToFind = random.Next(0, prize_spin_prize[betLevel].weights[prize_spin_prize[betLevel].weights.Length - 1]);
            int selected = RandomWeightedIndex(prize_spin_prize[betLevel].weights, valueToFind);
            return prize_spin_prize[betLevel].outcome[selected];
        }

        public string GetJackpotBonusOutcome(IRandom random)
        {
            int valueToFind = random.Next(0, jackpot_bonus_outcome[0].weights[jackpot_bonus_outcome[0].weights.Length - 1]);
            int selected = RandomWeightedIndex(jackpot_bonus_outcome[0].weights, valueToFind);
            return jackpot_bonus_outcome[0].outcome[selected];
        }

        public ReelItemJackpot GetAllGetPrizeSpinPrizes(int betLevel)
        {
            return prize_spin_prize[betLevel];
        }

        public List<List<int>> GetLookupPaytable()
        {
            return lookup_paytable;
        }


        public List<int> BetSteps
        {
            get { return betsteps; }
        }

        public List<int> Denoms
        {
            get { return denoms; }
        }


        public void GetCurrentLongTermPersistence(IStateItems stateItems)
        {
            for (int i = 1; i < long_term_persistence.Count; i++)
            {
                //TODO - I devide by 100 because it should be by dollar and not cents.
                if ((stateItems.TreasureChestTurnOver / 100) < long_term_persistence[i])
                {
                    if (i > stateItems.TreasureChestTurnState)
                    {
                        stateItems.TreasureChestTurnState++;
                        //stateItems.TreasureChestTurnState = i;
                    }
                    return;
                }
            }
            stateItems.TreasureChestTurnState++;
            if (stateItems.TreasureChestTurnState > 4)
            {
                stateItems.TreasureChestTurnState = 4;
            }
            //stateItems.TreasureChestTurnState = long_term_persistence.Count;
        }
        private static int RandomWeightedIndex(Array array, object valuetoFind)
        {
            int selectedIndex;
            int i = Array.BinarySearch(array, valuetoFind);
            int indexOfNearest = ~i;
            if (i >= 0)
            {
                selectedIndex = i + 1;
            }
            else
            {
                if (indexOfNearest == array.Length)
                {
                    // number is greater that last item (Should never happen)
                    throw new Exception("Cant Happen");
                }
                else if (indexOfNearest == 0)
                {
                    selectedIndex = 0;
                }
                else
                {
                    // number is between (indexOfNearest - 1) and indexOfNearest
                    selectedIndex = indexOfNearest;
                }
            }
            return selectedIndex;
        }

        //this method assign mcsymbol value to one item of the matrix
        public string MoneyChargeSymbol(int betLevel, int valueToFind, IRandom random)
        {
            var vec = base_prize[betLevel].weights;
            if (random != null)
            {
                valueToFind = random.Next(0, vec[vec.Length - 1]);
            }
            var selected = RandomWeightedIndex(vec.ToArray(), valueToFind);
            return base_prize[betLevel].outcome[selected].ToString();
        }

        public List<int> JackpotTableValues
        {
            get
            {
                return progressive_information[0];
            }

        }
        public class ReelItemJackpot
        {
            public List<int> outcome { get; set; }
            public int[] weights { get; set; }
        }

        public class ReelItemJackpotString
        {
            public List<string> outcome { get; set; }
            public int[] weights { get; set; }
        }
    }
}
