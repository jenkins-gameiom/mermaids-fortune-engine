﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic;
using AGS.Slots.MermaidsFortune.Logic.Engine;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using Autofac.Features.Indexed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Random = System.Random;

namespace AGS.Slots.MermaidsFortune.Platform
{
    public class Init
    {
        private readonly GameEngine _engineLogic;
        private readonly IRequestContext _context;


        public Init(IRequestContext context, GameEngine engineLogic)
        {
            _context = context;
            _engineLogic = engineLogic;
        }
        static Random rnd = new Random();

        #region Starting symbols

        private static List<String> StartingReels = new List<string>()
        {
            "[[9,1,1,10],[11,9,3,3],[1,1,11,1],[6,3,7,0],[4,10,5,4]]","[[6,3,4,7],[0,9,11,1],[11,2,7,10],[3,2,6,7],[10,5,4,6]]",
            "[[6,4,6,10],[8,1,1,5],[1,8,1,1],[6,4,6,9],[8,4,10,6]]","[[1,7,1,1],[3,9,8,11],[0,11,2,7],[13,13,13,13],[5,4,6,9]]",
            "[[13,13,10,2],[2,10,4,4],[7,12,11,4],[7,2,4,3],[7,4,5,9]]","[[5,5,10,2],[5,8,5,6],[3,8,5,11],[3,11,2,4],[4,6,4,9]]",
            "[[2,9,10,6],[9,1,1,8],[1,1,10,1],[8,5,6,4],[4,7,5,6]]","[[10,2,2,9],[8,3,3,6],[10,4,8,4],[3,4,2,6],[6,4,7,5]]",
            "[[9,2,8,2],[8,2,1,9],[7,1,1,11],[2,9,3,7],[7,12,8,5]]","[[9,8,3,10],[8,1,1,11],[8,2,7,4],[2,7,2,6],[6,4,10,7]]",
            "[[12,8,7,1],[7,3,11,8],[4,4,7,2],[6,4,6,3],[4,7,5,6]]","[[9,10,2,2],[13,7,3,11],[8,12,10,1],[4,8,2,6],[4,9,13,13]]",
            "[[3,4,11,4],[8,7,12,11],[4,10,5,5],[10,9,12,11],[12,8,5,7]]","[[4,4,9,8],[8,0,9,11],[8,12,10,1],[6,3,11,3],[6,5,8,9]]",
            "[[1,9,1,1],[11,12,9,6],[1,11,1,8],[11,2,6,5],[8,5,6,5]]","[[9,2,7,2],[8,2,1,9],[8,2,7,4],[4,6,3,2],[3,4,10,9]]",
            "[[1,7,1,1],[3,11,5,8],[4,6,1,1],[6,2,5,6],[6,5,8,9]]","[[5,10,2,2],[5,8,3,3],[1,8,4,4],[4,11,8,2],[9,1,1,6]]",
            "[[7,2,2,9],[11,8,0,9],[5,11,4,8],[3,4,2,7],[10,3,4,6]]","[[2,9,6,4],[13,8,5,5],[9,5,1,8],[3,9,2,6],[6,7,5,8]]",
            "[[7,9,2,2],[5,8,5,6],[8,5,11,0],[2,3,5,6],[4,6,9,5]]","[[10,4,4,9],[6,2,8,11],[12,11,4,4],[7,0,10,3],[5,6,7,4]]",
            "[[13,13,13,13],[1,8,1,9],[8,5,11,5],[3,7,0,11],[7,5,6,8]]","[[2,7,6,9],[3,6,8,0],[8,7,12,11],[3,7,2,4],[6,6,3,3]]",
            "[[1,1,10,1],[8,6,5,5],[4,4,8,5],[7,5,6,10],[7,4,6,4]]","[[4,9,10,6],[11,13,13,13],[4,4,10,4],[6,3,7,2],[5,6,3,3]]",
            "[[2,2,7,9],[4,9,12,8],[10,6,2,9],[7,6,2,6],[5,6,8,3]]","[[10,2,7,6],[13,13,13,13],[7,0,11,4],[3,3,6,7],[3,5,4,5]]",
            "[[2,2,7,9],[9,12,8,1],[10,4,8,12],[2,6,7,4],[7,5,6,8]]","[[4,10,8,2],[5,5,11,2],[8,5,5,9],[4,3,7,6],[4,6,8,1]]",
            "[[4,4,10,8],[3,10,5,5],[6,11,0,8],[3,2,3,6],[8,5,6,5]]","[[11,2,7,2],[6,8,0,9],[11,4,4,10],[7,2,6,3],[3,6,6,4]]",
            "[[9,5,5,10],[8,5,5,11],[11,1,8,10],[4,11,2,6],[6,9,4,5]]","[[7,9,2,10],[0,9,11,1],[11,1,1,10],[4,8,2,6],[6,4,7,4]]",
            "[[2,2,7,9],[5,11,5,8],[0,11,4,4],[6,4,6,3],[1,6,6,3]]","[[7,9,5,5],[8,11,3,9],[7,4,4,10],[2,9,12,7],[6,7,5,8]]",
            "[[10,2,2,9],[1,8,1,9],[0,11,4,4],[3,5,7,2],[8,3,3,6]]","[[7,4,4,6],[11,12,9,8],[4,4,8,3],[8,5,6,4],[4,4,7,12]]",
            "[[2,7,9,2],[4,8,4,11],[8,4,4,3],[11,2,7,5],[9,3,3,6]]","[[6,4,6,10],[8,2,11,13],[2,7,10,5],[3,4,2,6],[4,7,4,6]]",
            "[[13,13,7,11],[8,5,11,6],[1,11,1,8],[1,1,9,1],[9,1,1,6]]","[[7,1,9,1],[11,1,1,8],[1,1,10,5],[6,2,3,3],[1,8,1,5]]",
            "[[9,1,10,6],[6,8,0,9],[1,7,1,1],[9,4,3,6],[7,4,6,9]]","[[7,2,2,6],[3,11,5,8],[5,5,7,1],[7,2,6,3],[6,5,5,10]]",
            "[[9,8,3,10],[8,1,1,7],[11,5,10,6],[3,2,6,7],[4,10,6,4]]","[[10,6,2,2],[8,1,11,5],[7,5,10,4],[7,4,10,12],[10,6,5,6]]",
            "[[10,2,2,9],[0,9,11,1],[3,7,10,3],[4,10,12,7],[9,6,4,7]]","[[7,9,5,5],[3,8,1,11],[4,4,8,5],[6,3,3,2],[9,5,4,10]]",
            "[[6,2,2,8],[5,11,6,2],[4,10,5,5],[7,4,2,3],[5,8,4,7]]","[[4,11,4,6],[8,5,6,5],[1,10,1,8],[2,3,3,6],[9,5,4,10]]"
        };
        #endregion

        public static string GetStartingSymbols()
        {
            return StartingReels[rnd.Next(StartingReels.Count)];
        }

        static object locker = new object();

        private static string _initData = null;

        public static string GetInitDataString()
        {

            if (_initData == null)
            {
                lock (locker)
                {
                    if (_initData == null)
                    {

                        string currentDirectory = Directory.GetCurrentDirectory();
                        string filePath = System.IO.Path.Combine(currentDirectory, "Inits", "init.json");
                        _initData = File.ReadAllText(filePath);
                    }
                }
            }
            return _initData;
        }

        //TODO - use GetInitDataNew instead when gamium fix issues
        //public static dynamic GetInitDataOld(dynamic config)
        //{
        //    dynamic initdata = Commons.Json.Decode(GetInitDataString());
        //    ISlotConfig configRequest = Game.CurrentGame.GetConfig(config.rtp.ToString());
        //    initdata.stakes.availableStakes = Commons.Json.ObjectToDynamic(Game.CurrentGame.Config.BetSteps);
        //    initdata.stakes.defaultVal = Game.CurrentGame.GetConfig(config.rtp.ToString()).BetSteps.First(x => x == Configs.DefaultBet);
        //    initdata.denoms.values = Commons.Json.ObjectToDynamic(Configs.Denoms);
        //    initdata.denoms.defaultVal = Configs.Denoms.First(x => x == Configs.DefaultDenom);
        //    //var jObj2 = JsonConvert.DeserializeObject<List<int>>(config.stakes.ToString());
        //    initdata.jackpots = Commons.Json.ObjectToDynamic(Game.CurrentGame.Config.JackpotTableValues);
        //    
        //    return initdata;
        //}
        public dynamic GetInitData(dynamic config)
        {
            ;
            dynamic initdata = Json.Decode(GetInitDataString());
            if (config.stakes != null && ValidateStacks(config.stakes, _context.MathFile.BetStepsDevider))
            {
                initdata.stakes.availableStakes = config.stakes;
                initdata.stakes.defaultVal = config.defaultStake;
            }
            else
            {
                //take from math
                initdata.stakes.availableStakes = config.stakes;
                initdata.stakes.defaultVal = config.defaultStake;
            }

            if (config.denominations != null && ValidateDenoms(config.denominations))
            {
                initdata.denoms.values = config.denominations;
                initdata.denoms.defaultVal = config.defaultDenomination;
            }
            else
            {
                initdata.denoms.values = config.denominations;
                initdata.denoms.defaultVal = config.defaultDenomination;
            }

            initdata.jackpots = Json.ObjectToDynamic(_context.MathFile.JackpotTableValues);

            return initdata;
        }

        public static bool ValidateStacks(dynamic stakes, int betStepsDevider)
        {
            if (stakes != null)
            {
                string stakesString = stakes.ToString();
                var stakesFromPlatform = JsonConvert.DeserializeObject<List<int>>(stakesString);
                var orderedStakesFromPlatform = stakesFromPlatform.OrderBy(x => x).ToList();
                for (int i = 0; i < stakesFromPlatform.Count; i++)
                {
                    if (stakesFromPlatform[i] != orderedStakesFromPlatform[i])
                    {
                        throw new ValidationException("Bet amounts are not sorted, stakes from platform is " + stakesFromPlatform[i] + "ordered is " + orderedStakesFromPlatform[i]);
                    }
                    if (stakesFromPlatform[i] % betStepsDevider != 0)
                    {
                        throw new ValidationException("Bet amounts are not devided by bet devider");
                    }
                }
            }

            return true;
        }

        public static bool ValidateDenoms(dynamic denominations)
        {

            if (denominations != null)
            {
                string newDenominations = denominations.ToString();
                if (newDenominations.ToString().Contains('.'))
                {
                    throw new ValidationException("Denoms should contain only numbers and commas");
                }
                var denominationsFromPlatform = JsonConvert.DeserializeObject<List<int>>(newDenominations);
                var orderedDenominationsFromPlatform = denominationsFromPlatform.OrderBy(x => x).ToList();
                for (int i = 0; i < denominationsFromPlatform.Count; i++)
                {
                    if (denominationsFromPlatform[i] != orderedDenominationsFromPlatform[i])
                    {
                        throw new ValidationException("Denoms are not sorted");
                    }
                }
            }

            return true;
        }

        public dynamic InitSlot(dynamic obj)
        {
            try
            {
                int fwState = 0;
                bool cleanPrivateState = true;
                string userName = obj.publicState.userName ?? "";
                var treasureChestState = _context.State != null ? _context.State.TreasureChestTurnState : 0;
                var treasureChestTurnOver = _context.State != null ? _context.State.TreasureChestTurnOver : 0;
                string mode = obj.publicState.mode ?? "";
                string juri = Json.GetValueOrDefault(obj.publicState, "jurisdiction", "").Replace(" ", "_").ToLower();
                obj.publicState = GetInitData(obj.config);
                string val = GetStartingSymbols();
                obj.publicState.startingSymbols = Json.DecodeNS(val);
                obj.publicState.minStopDuration = 0;

                if (!string.IsNullOrEmpty(juri))
                {
                    //Set min stop in ms by jurisdiction
                    int ms;
                    if (Configs.StopDurationByJur.TryGetValue(juri, out ms))
                        obj.publicState.minStopDuration = ms;

                    //Disable autoplay by jurisdiction
                    if (Configs.NoAutoPlay.Contains(juri))
                        obj.publicState.autoplay = null;
                }

                //SpinPrivateState spinPrivateStateRequest = null;
                if (Json.HasProperty(obj, "privateState"))
                {

                    //spinPrivateStateRequest = Json.ConvertDynamic<SpinPrivateState>(obj.privateState);


                    if (_context.State != null && _context.State.lastState != null)
                    {
                        //Set Last state if wasnt completed
                        if (_context.State.completed == false && _context.State.lastState.spin.complete == false)
                        {
                            cleanPrivateState = false;
                            //Set recovered game spin
                            obj.publicState.recoveredGame = Json.ObjectToDynamic(_context.State.lastState);
                        }
                    }
                }

                
                //Clean privae state if not needed
                if (cleanPrivateState)
                    _context.State = new SpinPrivateState();
                if (treasureChestState == 0)
                {
                    treasureChestState = 1;
                }
                _context.State.TreasureChestTurnState = treasureChestState;
                _context.State.TreasureChestTurnOver = treasureChestTurnOver;
                _context.State.completed = false;
                _context.State.animationState = fwState;
                _context.State.userName = userName;
                _context.State.sessionId = Guid.NewGuid().ToString();
                _context.State.mode = mode;
                obj.publicState.treasureChestLevel = _context.State.TreasureChestTurnState;
            }
            catch (Exception ex)
            {
                obj = Json.ObjectToDynamic(new ErrorObject() { error = new Error() { message = ex.Message, stackTrace = ex.StackTrace } });
            }

            obj.privateState = Json.ObjectToDynamic(_context.State);
            return obj;
        }
    }
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {

        }
    }
}
