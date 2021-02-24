using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic;
using AGS.Slots.MermaidsFortune.Logic.Engine;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;
using AGS.Slots.MermaidsFortune.Logic.Engine.Providers;
using AGS.Slots.MermaidsFortune.WebAPI;
using Autofac.Features.Indexed;
using Microsoft.AspNetCore.Http;
using Moq;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace TestSlots
{
    public partial class TestSlotsDll : Form
    {
        const string EOL = "\r\n";
        public TestSlotsDll()
        {
            InitializeComponent();
            Bet_DropBox.SelectedIndex = 0;
            Denom_DropBox.SelectedIndex = 0;
            scatter3SymbolsPerReel = new List<List<int>>() { new List<int>() { 7, 9, 2 }, new List<int>() { 6, 12, 0 }, new List<int>() { 4, 7, 2 }, new List<int>() { 3, 12, 9 }, new List<int>() { 5, 5, 12 } };
            scatter4SymbolsPerReel = new List<List<int>>() { new List<int>() { 7, 12, 8, 1 }, new List<int>() { 6, 12, 4, 7 }, new List<int>() { 4, 12, 3, 2 }, new List<int>() { 3, 11, 9, 12 }, new List<int>() { 5, 5, 7, 4 } };
            scatter5SymbolsPerReel = new List<List<int>>() { new List<int>() { 1, 12, 3, 2, 4, 9 }, new List<int>() { 1, 1, 8, 12, 0, 2 }, new List<int>() { 4, 12, 0, 0, 9, 2 }, new List<int>() { 3, 0, 0, 12, 11, 6 }, new List<int>() { 12, 0, 0, 4, 10, 2 } };
        }


        private readonly List<List<int>> scatter3SymbolsPerReel;
        private readonly List<List<int>> scatter4SymbolsPerReel;
        private readonly List<List<int>> scatter5SymbolsPerReel;
        //this method will run spin() once and print the scatter to the richTextBox
        private void FNFNSpin_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
        private void PrintScatter(List<List<int>> scatter)
        {
            var strToPrint = string.Empty;
            int j = 0;
            int itemsOnReal = scatter.Select(x => x.Count).Max();
            while (j < itemsOnReal)
            {
                for (int i = 0; i < scatter.Count; i++)
                {
                    if (j + 1 > scatter[i].Count)
                        strToPrint += "-                ";
                    else
                        strToPrint += scatter[i][j] + "                ";
                }
                strToPrint += "\n\n";
                j++;
            }


            richTextBox1.Text = strToPrint;
        }
        private void AddItemsToLists(List<List<int>> scatter, int numbersToAdd)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < numbersToAdd; i++)
            {
                foreach (var list in scatter)
                {

                    list.Add(random.Next(0, 13));
                }
            }
        }
        private List<List<int>> CreateRandomMatrix(int reelsAmount, int symbolOnReel)
        {
            List<List<int>> list = new List<List<int>>();
            Random random = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < reelsAmount; i++)
            {
                List<int> l = new List<int>();
                for (int j = 0; j < symbolOnReel; j++)
                {

                    l.Add(random.Next(0, 13));
                }

                list.Add(l);
            }
            return list;
        }

        //this method will run the spin async for multiple times
        private async void RunButton_Click(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                Status_Label.Text = richTextBox1.Text = "";
            });

            AGS.Slots.MermaidsFortune.Common.Interfaces.IRandom random;

            if (CSharpRandom_RadioButton.Checked)
                random = new RandomGeneratorCrypro();
            else
                random = new IgamingRandomize(new Configs { WalletUrl = "https://test-rgs.gameiom.com/rgs/rest/rng/rngValues" });

            int bet = Convert.ToInt32(Bet_DropBox.SelectedItem);
            int denom = Convert.ToInt32(Denom_DropBox.SelectedItem);
            int spins = Convert.ToInt32(Spins_TextBox.Text);
            int denomIndex = Convert.ToInt32(Denom_DropBox.SelectedIndex);
            int betIndex = Convert.ToInt32(Bet_DropBox.SelectedIndex);
            bool withFreeSpins = true, withRegularSpins = true;

            if (RegularSpins_RadioButton.Checked)
            {
                withRegularSpins = true;
                withFreeSpins = false;
            }
            else if (FreeSpins_RadioButton.Checked)
            {
                withFreeSpins = true;
                withRegularSpins = false;
            }
            int freeSpinsTotalAmount = Convert.ToInt32(FSTotalAmount_DropBox.SelectedItem);
            //create new thread so GUI will be responsive
            //Run(bet, denom, spins, random, withFreeSpins, withRegularSpins, "BonanzaBlast", freeSpinsTotalAmount, denomIndex, betIndex);

            this.Text = "sup man";
            await Task.Factory.StartNew(() => Run(bet, denom, spins, random, withFreeSpins, withRegularSpins, "BonanzaBlast", freeSpinsTotalAmount, denomIndex, betIndex));
        }

        static List<int> bets = new List<int>();
        static Dictionary<string, int> combinations = new Dictionary<string, int>();
        private void Run(int betAmount, int denomAmount, int totalSpinsRequest, AGS.Slots.MermaidsFortune.Common.Interfaces.IRandom random, bool withFreeSpins, bool withRegSpins, string gameName, int freeSpinsTotalAmount, int denomIndex, int betIndex)
        {
            var stats = new Statistics();
            SimulatorLogic simLogic = new SimulatorLogic();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long spins = 0, totalBetAmount = 0;
            int betSum = betAmount * denomAmount;
            var context = InitContext(betAmount, denomAmount);
            
            var iindexMock = new Mock<IIndex<RandomizerType, IRandom>>();
            iindexMock.Setup(i => i[It.IsAny<RandomizerType>()]).Returns(random);
            
            for (int i = 0; i < totalSpinsRequest; i++)
            {
                simLogic.AddSpin(stats);
                spins++;
                simLogic.AddTotalBetAmount(stats, betSum);
                var configs = new Configs();
                context.RequestItems.isFreeSpin = false;
                
                BonusGameService bonusGameService = new BonusGameService(context, new Configs(), iindexMock.Object);
                JackpotService jackpotService = new JackpotService(context);
                GameEngine ge = new GameEngine(context, new MermaidsFortuneResolver(context, configs, iindexMock.Object),
                    new MermaidsFortuneScanner(context, iindexMock.Object, configs), configs, iindexMock.Object);
                Result res = ge.Spin(null);
                CalculateWin(context, bonusGameService, jackpotService, stats, res);
                while (context.State.freeSpinsLeft > 0)
                {
                    context.RequestItems.isFreeSpin = true;
                    //ge = new GameEngine(context, new MermaidsFortuneResolver(context, configs, iindexMock.Object),
                    //    new MermaidsFortuneScanner(context, iindexMock.Object, configs), configs, iindexMock.Object);
                    Result resFreeSpin = ge.Spin(null);
                    CalculateWin(context, bonusGameService, jackpotService, stats, resFreeSpin);
                }
                if (spins % 10000 == 0)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        progressBar1.Value = Convert.ToInt32(((decimal)spins / totalSpinsRequest) * 100);
                        Status_Label.Text = spins + "/" + totalSpinsRequest;
                        Status_Label.Refresh();
                        progressBar1.Refresh();
                    });
                }
            }
            sw.Stop();
            this.Invoke((MethodInvoker)delegate ()
            {
                richTextBox1.Text += "Test ran for " + sw.Elapsed.Hours + ":" + sw.Elapsed.Minutes + ":" + sw.Elapsed.Seconds + EOL + EOL;
                //richTextBox1.Text += simLogic.CalculateRTP(stats);
            });
        }

        private RequestExecutionContext InitContext(int betAmount, int denomAmount)
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
            context.MathFile = new AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune.Config("Math94");
            context.Config = new AGS.Slots.MermaidsFortune.Common.Entities.Config
            {
                stakes = new List<int>
                {
                    88, 176, 264, 528, 880
                },
                denominations = new List<int>
                {
                    1
                }
            };
            return context;
        }

        private List<int> GetAdjustedVector(int selectedReel, List<List<int>> reels, int index)
        {
            var reelResult = new List<int>();
            for (int j = 0; j < 6; j++)
            {
                reelResult.Add(reels[selectedReel][(index + j) % reels[selectedReel].Count]);
            }
            return reelResult;
        }


        private static long TotalAmountGrand;
        private static long TotalAmountMajor;
        private static long TotalAmountMinor;

        private static void CalculateWin(RequestExecutionContext context, BonusGameService bonusGameService,
            JackpotService jackpotService, Statistics stats, Result res)
        {
            if (context.State.BonusGame != null)
            {
                stats.Collector.TotalBonusGameWinTimes++;
                while (!context.State.BonusGame.complete)
                {
                    bonusGameService.SpinAll();
                }
                stats.Collector.TotalBonusMoneyWonAmount += bonusGameService.GetCashWon();
                bonusGameService.EndBonus();
            }
            if (context.State.JackpotGame != null)
            {
                stats.Collector.TotalJackpotWinTimes++;
                while (!context.State.JackpotGame.complete)
                {
                    jackpotService.HandleJackpot();
                }

                stats.Collector.TotalJackpotMoneyWonAmount += jackpotService.GetCashWonAndEndJackpot();
            }


            foreach (var win in res.Wins)
            {
                if ((win.WinType == WinType.FreeSpin || win.WinType == WinType.Regular) &&
                    context.RequestItems.isFreeSpin)
                {
                    stats.Collector.TotalFreeSpinsMoneyWonAmount += win.WinAmount;
                    stats.Collector.AmountOfFreeSpinsWon++;
                }

                if ((win.WinType == WinType.FreeSpin || win.WinType == WinType.Regular) &&
                    !context.RequestItems.isFreeSpin)
                {
                    stats.Collector.TotalRegularSpinsMoneyWonAmount += win.WinAmount;
                    stats.Collector.TotalWinTimesInBase++;
                }
            }
        }

        private static string Biggest { get; set; }
        private static List<List<int>> MaxWinOfAllRegularWinsReels { get; set; }
        private static long MaxWinOfAllRegularWins { get; set; }
        private static long MaxWinRegularWinContribution { get; set; }
        private static long MaxWinBonusWinContribution { get; set; }
        private static long MaxWin { get; set; }
        private static List<List<int>> MaxWinReels { get; set; }
        private void PrintTable_Button_Click(object sender, EventArgs e)
        {
            List<List<int>> scatter = new List<List<int>>();
            if (NonRandom_RadioButton.Checked)
            {
                int freeSpinsAmount = Convert.ToInt32(FSTotalAmount_DropBox.SelectedItem);

                scatter = scatter3SymbolsPerReel;
                switch (GetSymbolsPerReel())
                {
                    case 3:
                        scatter = scatter3SymbolsPerReel;
                        break;
                    case 4:
                        scatter = scatter4SymbolsPerReel;
                        break;
                    case 5:
                        scatter = scatter5SymbolsPerReel;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                scatter = CreateRandomMatrix(5, GetSymbolsPerReel());
            }

            PrintScatter(scatter);
        }

        private int GetSymbolsPerReel()
        {
            int freeSpinsAmount = Convert.ToInt32(FSTotalAmount_DropBox.SelectedItem);
            switch (freeSpinsAmount)
            {
                case 15:
                    return 3;
                    break;
                case 10:
                    return 4;
                case 5:
                    return 5;
                default:
                    return 5;
            }
        }

        private void Bet_DropBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
