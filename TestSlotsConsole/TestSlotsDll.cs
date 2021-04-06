using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic;
using AGS.Slots.MermaidsFortune.Logic.Engine;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;
using AGS.Slots.MermaidsFortune.Logic.Engine.Providers;
using AGS.Slots.MermaidsFortune.WebAPI;
using Autofac.Features.Indexed;
using Microsoft.AspNetCore.Http;
using Moq;

namespace TestSlotsConsole
{
    class TestSlotsDll
    {

        public async void RunSpins(int denom, int bet, string math, int spinsAmount)
        {
            //Run(bet, denom, math, spinsAmount);
            await Task.Factory.StartNew(() => Run(bet, denom, math, spinsAmount));
        }

        private void Run(int betAmount, int denomAmount, string math, int totalSpinsRequest)
        {
            IRandom random = new RandomGeneratorCrypro();
            var stats = new Statistics();
            SimulatorLogic simLogic = new SimulatorLogic();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long spins = 0, totalBetAmount = 0;
            int betSum = betAmount * denomAmount;
            var context = InitContext(betAmount, denomAmount, math);
            var iindexMock = new Mock<IIndex<RandomizerType, IRandom>>();
            iindexMock.Setup(i => i[It.IsAny<RandomizerType>()]).Returns(random);
            var configs = new Configs();
            for (int i = 0; i < totalSpinsRequest; i++)
            {
                if (i == totalSpinsRequest - 1)
                {

                }
                simLogic.AddSpin(stats);
                spins++;
                simLogic.AddTotalBetAmount(stats, betSum);

                context.RequestItems.isFreeSpin = false;


                GameEngine ge = new GameEngine(context, new MermaidsFortuneResolver(context, configs, random),
                    new MermaidsFortuneScanner(context, random, configs), configs, random);
                if (context.State.holdAndSpin != HoldAndSpin.None)
                {
                }
                Result res = ge.Spin(null);
                if (context.State.holdAndSpin != HoldAndSpin.None)
                {
                }
                CalculateWin(context, stats, res);
                var initGe = true;
                while (context.State.freeSpinsLeft > 0)
                {
                    context.RequestItems.isFreeSpin = true;
                    ge = new GameEngine(context, new MermaidsFortuneResolver(context, configs, random),
                        new MermaidsFortuneScanner(context, random, configs), configs, random);
                    var before = context.State.holdAndSpin;
                    
                    Result resFreeSpin = ge.Spin(null);
                    var after = context.State.holdAndSpin;
                    var beenInPlus = false;
                    if (before == HoldAndSpin.None)
                    {
                        context.State.freeSpinsLeft--;
                    }
                    if (after != HoldAndSpin.None)
                    {
                        context.State.freeSpinsLeft++;
                        beenInPlus = true;
                    }
                    CalculateWin(context, stats, resFreeSpin);
                    if (context.State.holdAndSpin != HoldAndSpin.None && context.State.freeSpinsLeft == 0)
                    {
                    }
                    
                    if (before != HoldAndSpin.None && after != HoldAndSpin.None)
                    {
                    }
                }
                if (spins % 10000 == 0)
                {
                    int percentage = Convert.ToInt32(((decimal)spins / totalSpinsRequest) * 100);
                    Console.WriteLine("Finished " + percentage + "%");
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                }
            }
            sw.Stop();
            Console.WriteLine("Test ran for " + sw.Elapsed.Hours + ":" + sw.Elapsed.Minutes + ":" + sw.Elapsed.Seconds + "\n\n");
            Console.WriteLine(simLogic.CalculateRTP(stats));
        }

        private RequestExecutionContext InitContext(int betAmount, int denomAmount, string math)
        {
            var httpContext = new HttpContextAccessor();
            httpContext.HttpContext = new DefaultHttpContext();
            RequestExecutionContext context = new RequestExecutionContext(httpContext);
            context.RequestItems = new RequestItems
            {
                betAmount = betAmount,
                denom = denomAmount
            };
            context.State = new State();
            context.MathFile = new AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune.Config("Math" + math);
            context.Config = new AGS.Slots.MermaidsFortune.Common.Entities.Config
            {
                stakes = new List<int>
                {
                    50,100, 150, 250, 500
                },
                denominations = new List<int>
                {
                    1
                }
            };
            return context;
        }

        private static void CalculateWin(RequestExecutionContext context, Statistics stats, Result res)
        {
            foreach (var win in res.Wins)
            {
                if (context.RequestItems.isFreeSpin)
                {
                    stats.Collector.TotalFreeSpinsMoneyWonAmount += win.WinAmount;
                    stats.Collector.AmountOfFreeSpinsWon++;
                }
                else
                {
                    stats.Collector.TotalRegularSpinsMoneyWonAmount += win.WinAmount;
                    stats.Collector.TotalWinTimesInBase++;
                }
                //
                //
                //
                //
                //
                //
                //
                //
                //
                //
                //if ((win.WinType == WinType.FreeSpin || win.WinType == WinType.Regular) &&
                //    context.RequestItems.isFreeSpin)
                //{
                //    stats.Collector.TotalFreeSpinsMoneyWonAmount += win.WinAmount;
                //    stats.Collector.AmountOfFreeSpinsWon++;
                //}
                //
                //if ((win.WinType == WinType.FreeSpin || win.WinType == WinType.Regular) &&
                //    !context.RequestItems.isFreeSpin)
                //{
                //    stats.Collector.TotalRegularSpinsMoneyWonAmount += win.WinAmount;
                //    stats.Collector.TotalWinTimesInBase++;
                //}
                //
                //if (win.WinType == WinType.FiveOfAKind)
                //{
                //    foreach (var mcSymbol in context.State.BonusGame.MCSymbols)
                //    {
                //        if (mcSymbol.symbol == 10)
                //        {
                //            stats.Collector.TotalMajorMoneyWonAmount += mcSymbol.winAmount;
                //        }
                //        if (mcSymbol.symbol == 11)
                //        {
                //            stats.Collector.TotalMinorMoneyWonAmount += mcSymbol.winAmount;
                //        }
                //        if (mcSymbol.symbol == 12)
                //        {
                //            stats.Collector.TotalNumberMoneyWonAmount += mcSymbol.winAmount;
                //        }
                //        if (mcSymbol.symbol == 13)
                //        {
                //            stats.Collector.TotalGrandMoneyWonAmount += mcSymbol.winAmount;
                //        }
                //
                //        if (context.RequestItems.isFreeSpin)
                //        {
                //            if (mcSymbol.winAmount != 100)
                //            {
                //
                //            }
                //            stats.Collector.TotalFreeSpinsMoneyWonAmount += mcSymbol.winAmount;
                //        }
                //        else
                //        {
                //            stats.Collector.TotalRegularSpinsMoneyWonAmount += mcSymbol.winAmount;
                //        }
                //        //stats.Collector.TotalBonusMoneyWonAmount += mcSymbol.winAmount;
                //    }
                //}
                //if (win.WinType == WinType.FiveOfAKind && !context.RequestItems.isFreeSpin)
                //{
                //
                //}
            }
        }

    }
}
