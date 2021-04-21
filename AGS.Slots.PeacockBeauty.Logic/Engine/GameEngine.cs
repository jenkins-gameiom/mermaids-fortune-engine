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
        private  MermaidsFortuneScanner _scanner;
        private readonly IRandom _random;
        private readonly IRequestContext _context;

        public GameEngine(IRequestContext context, IPayoutResolver resolver, MermaidsFortuneScanner scanner,Configs applicationConfig, IIndex<RandomizerType, IRandom> random = null)
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

                if (_context.State.freeSpinsLeft <= 0)
                    throw new Exception("Error in validating freeSpins (freeSpinsLeft<=0)");
                else if(_context.State.isReSpin == null || !_context.State.isReSpin.Value)
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
            //this line is straight forward - we have in capital gains 5 different combinations for the reels.
            //4 for FS, and 1 for regular spin. why we have 5? cause we want different amount of wilds for different spins.
            //so here we just get the reels from the config.
            //**notice we dont get the reels result (3 number for each reel), but the WHOLE reels ( about 65 numbers for each reel)
            //List<List<int>> reels = _context.MathFile.GetReels();
            if (_context.State.isReSpin == null || !_context.State.isReSpin.Value)
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
            //here, from the 5 whole reels, we get the actual matrix that will say if we won and what we won (5*3)
            //this is where the randomize functions kicks (Gamiume, C# Random etc.).
            if (spinResult == null)
                spinResult = _context.MathFile.GetReels(_context, _random).reels;
            //here is actually the analysis of our result (3*5), we iterate the reels and see if we won something. options to win:
            //1 - scatter, we got 3 scatters (index 12) so we go to gateway-FS OR TO Gateway-MC
            //2 - regular line wins, 3 or more same symbols including wilds etc.
            //3 - we got 6+ MC (index 13) so we got into MC
            List<int> reesStops = new List<int>();
            Result result = new Result();
            
            ApplyResultion(spinResult, result);
            Scan(result);
            CalculateResult(spinResult, result);//freespinandregular - 
            //if (result.Wins.Count == 1 && result.Wins.Any(x => x.WinType == WinType.FiveOfAKind)
            //    && _context.State.holdAndSpin != HoldAndSpin.First)
            //{
            //    //3 scatters yes regular win "3 BN SYMBOLS + win"
            //    SerializeObjectAndWriteToFile(result, "fiveofakindwithfirstreelrespin");
            //}
            //if (result.Wins.Count == 1 && result.Wins.Any(x => x.WinType == WinType.FiveOfAKind)
            //                           && _context.State.holdAndSpin != HoldAndSpin.Both)
            //{
            //    //3 scatters yes regular win "3 BN SYMBOLS + win"
            //    SerializeObjectAndWriteToFile(result, "fiveofakindwithbothreelrespin");
            //}


            //FROM NOW THOSE CANT HAPPEN, no combination of JP1,JP2,JP4 exists
            //if (!_context.RequestItems.isFreeSpin && result.Wins.Count == 1 && result.Wins.Any(x => x.WinType == WinType.FiveOfAKind)
            //    && !_context.State.BonusGame.MCSymbols.Any(x => x.JPSymbolIfString == "grand")
            //    && _context.State.BonusGame.MCSymbols.Any(x => x.JPSymbolIfString == "major")
            //    && _context.State.BonusGame.MCSymbols.Any(x => x.JPSymbolIfString == "minor"))
            //{
            //    //3 scatters yes regular win "3 BN SYMBOLS + win"
            //    SerializeObjectAndWriteToFile(result, "fiveofakindminormajor");
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


        private void ApplyResultion(List<List<int>> reels,Result result)
        {
            _scanner.ApplyResultion(reels, result);
        }

        public void Scan(Result result)
        {
            _scanner.Scan(result);
        }

        private void  CalculateResult(List<List<int>> resultedReels, Result result)
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
