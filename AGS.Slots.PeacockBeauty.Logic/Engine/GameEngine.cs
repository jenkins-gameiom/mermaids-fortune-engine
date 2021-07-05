using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using Autofac.Features.Indexed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.Math;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;

namespace AGS.Slots.MermaidsFortune.Logic.Engine
{
    public class GameEngine
    {
        private readonly IPayoutResolver _resolver;
        private MermaidsFortuneScanner _scanner;
        private readonly IRandom _random;
        private readonly IRequestContext _context;

        public GameEngine(IRequestContext context, IPayoutResolver resolver, MermaidsFortuneScanner scanner, Configs applicationConfig, IIndex<RandomizerType, IRandom> random = null)
        {
            _context = context;
            _resolver = resolver;
            _scanner = scanner;
            if (applicationConfig.IsTest)
                _random = random[RandomizerType.Local];
            else
                _random = random[RandomizerType.Remote];
        }

        public GameEngine(IRequestContext context, IPayoutResolver resolver, MermaidsFortuneScanner scanner, Configs applicationConfig, IRandom random)
        {
            _context = context;
            _resolver = resolver;
            _scanner = scanner;
            _random = random;
        }
        public void ValidateSpins()
        {
            if (_context.RequestItems.isFreeSpin)
            {

                if (_context.State.freeSpinsLeft < 0 || (_context.State.freeSpinsLeft == 0 && !_context.State.isReSpin))
                    throw new Exception("Error in validating freeSpins (freeSpinsLeft<=0)");
                else if (!_context.State.isReSpin)
                    _context.State.freeSpinsLeft--;
            }
            else
            {
                if (_context.State.freeSpinsLeft > 0)
                    throw new Exception("Should be in freeSpins " + _context.State.freeSpinsLeft + " Left and regular spin requested");
            }
        }

        public static bool enteredInRespin = false; 
        public Result Spin(List<List<int>> spinResult = null)
        {
            //ValidateSpins();
            if (!_context.State.isReSpin)
            {
                _context.MathFile.AssignReelSet(_context, _random);
                enteredInRespin = false;
            }
            else
            {
                if (enteredInRespin)
                {
                    if (_context.State.holdAndSpin != HoldAndSpin.Both)
                    {

                    }
                }
                enteredInRespin = true;
            }
            _context.State.BonusGame = null;
            if (spinResult == null)
                spinResult = _context.MathFile.GetReels(_context, _random).reels;
            if (!_context.RequestItems.isFreeSpin)
            {
                //var count1 = _context.MathFile.GetFullReels(1);
                //var count2 = _context.MathFile.GetFullReels()[1].Count;
                //var count3 = _context.MathFile.GetFullReels()[2].Count;
                //var count4 = _context.MathFile.GetFullReels()[3].Count;
                //var count5 = _context.MathFile.GetFullReels()[4].Count;

            }
            List<int> reesStops = new List<int>();
            Result result = new Result();
            ApplyResultion(spinResult, result);
            Scan(result);
            CalculateResult(spinResult, result);


            //FROM NOW THOSE CANT HAPPEN, no combination of JP1,JP2,JP4 exists
            //if (_context.RequestItems.isFreeSpin && _context.State.holdAndSpin == HoldAndSpin.First && result.Wins.Any(x => x.WinType == WinType.FiveOfAKind))
            //{
            ////    //3 scatters yes regular win "3 BN SYMBOLS + win"
            //    SerializeObjectAndWriteToFile(result, "fiveoakandrespininsidefreespin");
            //}
            //if (!_context.RequestItems.isFreeSpin && result.Wins.Count == 1 && result.Wins.Any(x => x.WinType == WinType.FiveOfAKind)
            //    && _context.State.BonusGame.MCSymbols.Any(x => x.JPSymbolIfString == "grand")
            //    && _context.State.BonusGame.MCSymbols.Any(x => x.JPSymbolIfString == "major")
            //    && !_context.State.BonusGame.MCSymbols.Any(x => x.JPSymbolIfString == "minor"))
            //{
            //    //3 scatters yes regular win "3 BN SYMBOLS + win"
            //    SerializeObjectAndWriteToFile(result, "fiveofakindmajorgrand");
            //}
            //if (!_context.RequestItems.isFreeSpin && result.Wins.Count == 1 && result.Wins.Any(x => x.WinType == WinType.FiveOfAKind)
            //    && _context.State.BonusGame.MCSymbols.Any(x => x.JPSymbolIfString == "grand")
            //    && !_context.State.BonusGame.MCSymbols.Any(x => x.JPSymbolIfString == "major")
            //    && _context.State.BonusGame.MCSymbols.Any(x => x.JPSymbolIfString == "minor"))
            //{
            //    //3 scatters yes regular win "3 BN SYMBOLS + win"
            //    SerializeObjectAndWriteToFile(result, "fiveofakindminorgrand");
            //}
            //if (!_context.RequestItems.isFreeSpin && result.Wins.Count == 1 && result.Wins.Any(x => x.WinType == WinType.FiveOfAKind)
            //    && _context.State.BonusGame.MCSymbols.Any(x => x.JPSymbolIfString == "grand")
            //    && _context.State.BonusGame.MCSymbols.Any(x => x.JPSymbolIfString == "major")
            //    && _context.State.BonusGame.MCSymbols.Any(x => x.JPSymbolIfString == "minor"))
            //{
            //    //3 scatters yes regular win "3 BN SYMBOLS + win"
            //    SerializeObjectAndWriteToFile(result, "fiveofakindminormajorgrand");
            //}
            return result;

        }


        private void ApplyResultion(List<List<int>> reels, Result result)
        {
            _scanner.ApplyResultion(reels, result);
        }

        public void Scan(Result result)
        {
            _scanner.Scan(result);
        }

        private void CalculateResult(List<List<int>> resultedReels, Result result)
        {
            result.Reels = resultedReels;
            _resolver.EvaluateResult(result);
        }

        public static bool FileExists(string fileName)
        {
            //for getting json, should be used from the app
            //var fullPath3 = System.IO.Directory.GetCurrentDirectory() + "\\Engine\\MermaidsFortune\\Forces\\" + fileName + ".json";
            var fullPath = System.AppDomain.CurrentDomain.BaseDirectory + "Engine\\MermaidsFortune\\Forces\\" + fileName + ".json"; ;
            //for creating json, should be used only local.
            //var fullPath2 = @"C:\AGSI\AGSI\Branches\RMG\2020\MermaidsFortune\Slots\Engine\MermaidsFortune\Forces\" + fileName + ".json";
            return File.Exists(fullPath);
        }
        public static Result DeserializeAndReturnResult(string fileName)
        {
            Result resultObject = null;
            try
            {
                var fullPath = System.AppDomain.CurrentDomain.BaseDirectory + "Engine\\MermaidsFortune\\Forces\\" + fileName + ".json"; ;
                var fullPath3 = System.IO.Directory.GetCurrentDirectory() + "\\Engine\\MermaidsFortune\\Forces\\" + fileName + ".json";
                using (StreamReader r = new StreamReader(fullPath))
                {
                    string json = r.ReadToEnd();
                    resultObject = JsonConvert.DeserializeObject<Result>(json);
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return resultObject;
        }

        private static bool SerializeObjectAndWriteToFile(Result res, string fileName)
        {
            try
            {
                var objectString = JsonConvert.SerializeObject(res, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                });
                var fullPath2 = @"C:\Projects\mermaids-fortune-engine\AGS.Slots.PeacockBeauty.Logic\Engine\MermaidsFortune\Forces\" + fileName + ".json";
                System.IO.File.WriteAllText(fullPath2, objectString);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

    }
}
