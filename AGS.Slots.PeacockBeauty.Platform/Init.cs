using System;
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
            "[[5,6,4],[7,2,2],[2,2,8,14],[4,8,1],[9,5,1]]","[[2,5,6],[8,7,3],[2,2,4,4],[3,3,3],[4,4,5]]",
            "[[6,9,5],[4,4,7],[8,14,5,3],[5,0,0],[5,8,1]]","[[1,8,14],[2,2,2],[3,5,4,4],[3,4,4],[14,5,1]]",
            "[[8,9,7],[7,1,6],[4,4,3,3],[3,3,4],[7,1,5]]","[[6,5,9],[3,3,3],[14,7,4,4],[0,5,2],[7,8,4]]",
            "[[5,9,7],[7,8,2],[1,1,6,14],[2,6,3],[6,14,5]]","[[2,8,9],[2,3,3],[8,14,5,3],[2,6,3],[4,4,5]]",
            "[[5,9,7],[8,4,4],[14,3,3,2],[9,8,6],[6,14,5]]","[[7,14,8],[3,3,3],[14,7,4,4],[8,9,5],[3,3,3]]",
            "[[2,2,7],[2,2,5],[3,3,5,4],[4,6,9],[4,4,5]]","[[14,8,2],[4,4,3],[3,3,2,2],[3,4,4],[2,14,5]]",
            "[[9,7,8],[4,4,4],[4,3,3,3],[5,2,2],[14,5,3]]","[[9,5,6],[0,8,4],[3,4,4,4],[9,8,9],[5,6,9]]",
            "[[7,14,8],[4,4,7],[3,2,2,4],[4,3,3],[8,1,6]]","[[1,8,1],[7,2,2],[4,3,3,3],[0,0,5],[14,5,3]]",
            "[[7,14,8],[4,4,4],[14,6,2,2],[9,7,4],[6,8,2]]","[[3,3,2],[4,7,9],[5,4,4,8],[3,3,3],[4,4,5]]",
            "[[5,6,9],[3,3,2],[2,2,4,4],[0,0,0],[9,4,4]]","[[3,3,5],[0,4,4],[6,14,7,4],[3,4,4],[4,6,9]]",
            "[[9,5,6],[4,4,4],[3,3,5,4],[4,3,3],[4,5,9]]","[[2,2,6],[2,5,4],[5,6,1,1],[6,1,5],[7,1,5]]",
            "[[6,9,5],[6,3,3],[3,5,4,4],[9,7,4],[4,4,5]]","[[8,2,2],[3,2,2],[14,7,4,4],[3,3,7],[8,1,6]]",
            "[[9,6,1],[3,3,3],[2,2,2,4],[6,0,0],[8,2,6]]","[[1,3,3],[1,7,2],[2,2,4,4],[0,0,5],[5,9,7]]",
            "[[6,4,4],[7,8,2],[3,3,5,4],[6,0,0],[2,14,5]]","[[5,14,8],[0,0,2],[3,2,2,4],[3,3,3],[9,7,5]]",
            "[[2,5,14],[4,4,7],[8,14,6,2],[5,3,3],[3,3,3]]","[[5,6,9],[2,8,4],[5,1,7,6],[3,8,7],[8,1,1]]",
            "[[6,9,5],[2,8,9],[3,4,4,4],[4,4,6],[9,5,1]]","[[2,5,14],[8,6,3],[2,2,4,4],[4,4,4],[9,4,4]]",
            "[[9,5,6],[7,9,6],[14,3,3,2],[9,8,6],[6,14,5]]","[[14,8,2],[4,7,9],[8,14,5,3],[3,8,7],[8,9,5]]",
            "[[8,7,3],[2,2,8],[14,3,3,2],[9,8,9],[3,3,5]]","[[2,5,6],[8,4,4],[8,14,5,3],[9,7,6],[1,7,6]]",
            "[[4,7,9],[2,7,8],[4,4,3,3],[4,4,7],[6,8,2]]","[[9,5,1],[8,4,4],[3,5,4,4],[6,9,4],[5,1,1]]",
            "[[9,7,6],[3,8,6],[3,3,8,14],[2,6,9],[5,3,3]]","[[9,5,6],[8,6,1],[1,2,2,8],[9,2,6],[8,1,6]]",
            "[[14,6,4],[2,2,7],[8,14,5,3],[6,0,0],[9,7,8]]","[[7,14,8],[8,9,5],[3,3,5,4],[3,4,4],[14,5,3]]",
            "[[9,7,6],[2,8,4],[8,14,5,3],[0,0,5],[6,2,9]]","[[3,3,5],[1,8,7],[4,4,8,14],[4,3,3],[7,8,4]]",
            "[[3,2,5],[2,2,8],[3,5,4,4],[7,6,9],[1,6,8]]","[[5,6,1],[2,2,2],[2,2,4,4],[4,4,3],[5,6,9]]",
            "[[2,2,6],[4,3,5],[5,1,1,6],[8,9,5],[9,5,1]]","[[9,6,1],[0,0,7],[4,8,14,5],[8,9,5],[5,3,3]]",
            "[[2,2,6],[8,2,2],[14,7,4,4],[4,6,9],[4,4,6]]","[[2,5,6],[7,8,2],[8,14,5,3],[4,6,9],[9,5,1]]"
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
                _context.State.completed = false;
                _context.State.animationState = fwState;
                _context.State.userName = userName;
                _context.State.sessionId = Guid.NewGuid().ToString();
                _context.State.mode = mode;
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
