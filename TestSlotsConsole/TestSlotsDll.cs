using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using Microsoft.AspNetCore.Mvc.Formatters;
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
                context.State.totalFreeSpins = context.State.totalFreeSpins == null ? 0 : context.State.totalFreeSpins;
                simLogic.AddSpin(stats);
                spins++;
                simLogic.AddTotalBetAmount(stats, betSum);

                context.RequestItems.isFreeSpin = false;
                if (context.State.totalFreeSpins != null && context.State.totalFreeSpins != 0)
                {
                    context.State.totalFreeSpins = 0;
                }
                GameEngine ge = new GameEngine(context, new MermaidsFortuneResolver(context, configs, random),
                    new MermaidsFortuneScanner(context, random, configs), configs, random);
                Result res = ge.Spin(null);
                CalculateWin(context, stats, res);
                var initGe = true;
                while (context.State.freeSpinsLeft > 0)
                {
                    context.RequestItems.isFreeSpin = true;
                    ge = new GameEngine(context, new MermaidsFortuneResolver(context, configs, random),
                        new MermaidsFortuneScanner(context, random, configs), configs, random);
                    Result resFreeSpin = ge.Spin(null);
                    if (!context.State.isReSpin)
                    {
                        context.State.freeSpinsLeft--;
                    }
                    CalculateWin(context, stats, resFreeSpin);
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
            }
        }

    }
}
