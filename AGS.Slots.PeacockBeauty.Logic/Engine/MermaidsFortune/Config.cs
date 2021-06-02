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
        private List<List<BaseTable>> base_reel_set = null;
        private List<List<BaseTable>> fg_reel_set = null;
        private List<List<BaseTable>> fg_binary_reel_set = null;
        private List<ReelItemJackpot> base_bonus_weights = null;
        private List<ReelItemJackpot> fg_bonus_weights = null;
        private List<ReelItemJackpot> fg_binary_bonus_weights = null;
        private int[] base_reelset_weights = null;
        private int[] fg_reelset_weights = null;

        private dynamic config;
        private List<List<int>> progressive_information = null;
        private List<ReelItemJackpot> base_prize = null;
        private List<List<int>> lookup_paytable = null;
        private List<int> betsteps = null;
        private List<int> denoms = null;

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
            base_reel_set = config.base_reel_set.ToObject<List<List<BaseTable>>>();
            foreach (var base_bonus in base_reel_set)
            {
                foreach (var x in base_bonus)
                {
                    foreach (var y in x.weights)
                    {
                        AggregateArray(y);
                    }
                }
            }
            fg_reel_set = config.fg_reel_set.ToObject<List<List<BaseTable>>>();
            foreach (var base_bonus in fg_reel_set)
            {
                foreach (var x in base_bonus)
                {
                    foreach (var y in x.weights)
                    {
                        AggregateArray(y);
                    }
                }
            }
            fg_binary_reel_set = config.fg_binary_reel_set.ToObject<List<List<BaseTable>>>();
            foreach (var base_bonus in fg_binary_reel_set)
            {
                foreach (var x in base_bonus)
                {
                    foreach (var y in x.weights)
                    {
                        AggregateArray(y);
                    }
                }
            }
            lookup_paytable = config.lookup_paytable.ToObject<List<List<int>>>();
            base_bonus_weights = config.base_bonus_weights.ToObject<List<ReelItemJackpot>>();
            foreach (var base_bonus in base_bonus_weights)
            {
                AggregateArray(base_bonus.weights);
            }
            fg_bonus_weights = config.fg_bonus_weights.ToObject<List<ReelItemJackpot>>();
            foreach (var fg_bonus in fg_bonus_weights)
            {
                AggregateArray(fg_bonus.weights);
            }
            fg_binary_bonus_weights = config.fg_binary_bonus_weights.ToObject<List<ReelItemJackpot>>();
            foreach (var fg_binary_bonus in fg_binary_bonus_weights)
            {
                AggregateArray(fg_binary_bonus.weights);
            }
            base_reelset_weights = config.base_reelset_weights.ToObject<int[]>();
            fg_reelset_weights = config.fg_reelset_weights.ToObject<int[]>();
            AggregateArray(base_reelset_weights);
            AggregateArray(fg_reelset_weights);
            progressive_information = config.progressive_information.ToObject<List<List<int>>>();
            betsteps = config.betsteps.ToObject<List<int>>();
            denoms = config.denoms.ToObject<List<int>>();
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
                        var dir = AppDomain.CurrentDomain.BaseDirectory;
                        var path = dir + mathName + ".json";
                        config = JObject.Parse(File.ReadAllText(path));
                    }
                }
            }
        }

        public List<int> GetProgressiveInformation()
        {
            return progressive_information[0];
        }

        public int BetStepsDevider
        {
            get
            {
                return 50;
            }
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

        //public int GetBasePrize(int betLevel, IRandom random)
        //{
        //    int valueToFind = random.Next(0, base_prize[betLevel].weights[base_prize[betLevel].weights.Length - 1]);
        //    int selected = RandomWeightedIndex(base_prize[betLevel].weights, valueToFind);
        //    return base_prize[betLevel].outcome[selected];
        //    //return selected;
        //}

        //public ReelItemJackpot GetAllBasePrizes(int betLevel)
        //{
        //    return base_prize[betLevel];
        //}

        //public int GetPrizeSpinPrize(int betLevel, IRandom random)
        //{
        //    int valueToFind = random.Next(0, prize_spin_prize[betLevel].weights[prize_spin_prize[betLevel].weights.Length - 1]);
        //    int selected = RandomWeightedIndex(prize_spin_prize[betLevel].weights, valueToFind);
        //    return prize_spin_prize[betLevel].outcome[selected];
        //}

        //public string GetJackpotBonusOutcome(IRandom random)
        //{
        //    int valueToFind = random.Next(0, jackpot_bonus_outcome[0].weights[jackpot_bonus_outcome[0].weights.Length - 1]);
        //    int selected = RandomWeightedIndex(jackpot_bonus_outcome[0].weights, valueToFind);
        //    return jackpot_bonus_outcome[0].outcome[selected];
        //}

        public void AssignReelSet(IRequestContext _context, IRandom random)
        {
            _context.State.reelSet = GetReelSet(_context.RequestItems.isFreeSpin, random);
        }

        public SpinBagResult GetFullReels(IRequestContext _context, List<int> chosenIndexes)
        {
            //int reelSet = _context.State.reelSet;
            SpinBagResult res = new SpinBagResult();
            List<List<int>> reels = null;
            //List<int> weightedIndexes = null;
            BaseTable reelsWithWeights = null;
            reelsWithWeights = base_reel_set[0][_context.MathFile.BetSteps.IndexOf(_context.GetBetAmount())];
            reels = reelsWithWeights.outcome;
            //res.indexRes = weightedIndexes;
            res.reels = reels.RandomizeReels(new List<int>(new int[] { 3, 3, 4, 3, 3 }), chosenIndexes);
            return res;
        }

        public SpinBagResult GetReels(IRequestContext _context, IRandom random)
        {
            int reelSet = _context.State.reelSet;
            SpinBagResult res = new SpinBagResult();
            List<List<int>> reels = null;
            List<int> weightedIndexes = null;
            BaseTable reelsWithWeights = null;
            if (_context.RequestItems.isFreeSpin)
            {
                if (_context.State.holdAndSpin != HoldAndSpin.None)
                {
                    
                    reelsWithWeights = fg_binary_reel_set[(int) (_context.State.holdAndSpin - 1)][0];
                }
                else
                {
                    reelsWithWeights = fg_reel_set[reelSet][0];
                    if (reelSet == 3)
                    {

                    }
                }
            }
            else
            {
                reelsWithWeights = base_reel_set[reelSet][_context.MathFile.BetSteps.IndexOf(_context.GetBetAmount())];
            }

            List<int> chosenIndexes = new List<int>();
            for (int i = 0; i < reelsWithWeights.outcome.Count; i++)
            {
                var xxx = random.Next(0, reelsWithWeights.weights[i][reelsWithWeights.weights[i].Length - 1]);
                var selected = RandomWeightedIndex(reelsWithWeights.weights[i], xxx);
                if (selected == 28 && i == 1)
                {

                }
                chosenIndexes.Add(selected);
            }
            //var valueToFind = random.Next(0, reelsWithWeights.weights[reelsWithWeights.weights.Count - 1]);
            //
            
            //return base_bonus_weights[reelSet].outcome[selected];
            //
            //
            //weightedIndexes = GetWeightedSymbol(reelsWithWeights, random);
            reels = reelsWithWeights.outcome;
            //res.indexRes = weightedIndexes;
            res.reels = reels.RandomizeReels(new List<int>(new int[] {3, 3, 4, 3, 3}), chosenIndexes);
            return res;
        }

        private int GetReelSet(bool isFreeSpins, IRandom random)
        {
            int[] arr = null;
            if (isFreeSpins)
            {
                arr = fg_reelset_weights;
            }
            else
            {
                arr = base_reelset_weights;
            }
            int valueToFind = random.Next(0, arr[arr.Length - 1]);
            int selected = RandomWeightedIndex(arr, valueToFind);
            return selected;
        }

        //private List<int> GetWeightedSymbol(BaseTable table, IRandom random)
        //{
        //    List<int> ret = new List<int>();
        //
        //    var numOfReels = table.lookup_table_symbols.Count();
        //    List<RandomNumber> randomNumbers = new List<RandomNumber>();
        //    for (int i = 0; i < numOfReels; i++)
        //    {
        //        RandomNumber r = new RandomNumber();
        //        r.Min = 0;
        //        r.Max = table.bucket_sort_table_symbols[i].Count;
        //        r.Quantity = 1;
        //        randomNumbers.Add(r);
        //    }
        //    var rnumberres = random.GetRandomNumbers(randomNumbers);
        //
        //    for (var rn = 0; rn < rnumberres.Count(); rn++)
        //    {
        //        ret.Add(table.bucket_sort_table_symbols[rn][randomNumbers[rn].Values[0]]);
        //    }
        //    return ret;
        //
        //}

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
        public int MoneyChargeSymbol(bool isFreeSpin, int reelSet, IRandom random, HoldAndSpin holdAndSpinType = HoldAndSpin.None)
        {
            int valueToFind = 0;
            if (isFreeSpin)
            {
                if (holdAndSpinType == HoldAndSpin.None)
                {
                    valueToFind = random.Next(0,
                        fg_bonus_weights[reelSet].weights[fg_bonus_weights[reelSet].weights.Length - 1]);
                    var selected = RandomWeightedIndex(fg_bonus_weights[reelSet].weights, valueToFind);
                    return fg_bonus_weights[reelSet].outcome[selected];
                }
                else
                {
                    var holdAndSpinInt = (int) (holdAndSpinType - 1);
                    valueToFind = random.Next(0,
                        fg_binary_bonus_weights[holdAndSpinInt].weights[fg_binary_bonus_weights[holdAndSpinInt].weights.Length - 1]);
                    var selected = RandomWeightedIndex(fg_binary_bonus_weights[holdAndSpinInt].weights, valueToFind);
                    return fg_binary_bonus_weights[holdAndSpinInt].outcome[selected];
                }
            }
            else
            {
                valueToFind = random.Next(0, base_bonus_weights[reelSet].weights[base_bonus_weights[reelSet].weights.Length - 1]);
                var selected = RandomWeightedIndex(base_bonus_weights[reelSet].weights, valueToFind);
                return base_bonus_weights[reelSet].outcome[selected];
            }
            
        }
        public List<int> JackpotTableValues
        {
            get
            {
                return progressive_information[0];
            }
        }
    }
}
