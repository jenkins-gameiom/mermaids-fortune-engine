using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;
using Moq;
using Xunit;
using Config = AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune.Config;

namespace AGS.Slots.MermaidsFortune.Logic.Tests
{
    public class ResolverTests
    {
        private readonly Mock<IRandom> _random;
        private readonly Config _config;
        private Mock<IRequestContext> _contextInstance;
        private MermaidsFortuneResolver _resolver;
        private Configs _configsInstance;

        public ResolverTests()
        {
            _random = new Mock<IRandom>();
            //_random.Setup(a =>  a.Next(0, 100)).Returns(6);
            //_random.Setup(a => a.Next(0, 50)).Returns(4);
            _random.Setup(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(new List<RandomNumber>(new RandomNumber[] {
                    new RandomNumber() {Min=0,Max=57,Quantity=1,Values=new List<int>(new int[]{56 })},
                    new RandomNumber() {Min=0,Max=61,Quantity=1,Values=new List<int>(new int[]{59 })},
                    new RandomNumber() {Min=0,Max=71,Quantity=1,Values=new List<int>(new int[]{65 })},
                    new RandomNumber() {Min=0,Max=111,Quantity=1,Values=new List<int>(new int[]{60 })},
                    new RandomNumber() {Min=0,Max=104,Quantity=1,Values=new List<int>(new int[]{78 })} }));
            _config = new Config("Math96");
            _contextInstance = new Mock<IRequestContext>();
            _contextInstance.Setup(x => x.RequestItems).Returns(new RequestItems
            {
                action = "spin",
                betAmount = 50,
                denom = 1,
                cleanState = false,
                force = null,
                isFreeSpin = false
            });
            _contextInstance.Setup(x => x.State).Returns(new State()
            {
                reelSet = 0
            });
            _contextInstance.Setup(x => x.Config).Returns(new Common.Entities.Config
            {
                stakes = new List<int> { 50,100,150,250,500},
                denominations = new List<int>{1}
            });
            _contextInstance.Setup(x => x.Config).Returns(new Common.Entities.Config
            {
                stakes = new List<int> { 50, 100, 150, 250, 500 },
                denominations = new List<int> { 1 }
            });
            _contextInstance.Setup(x => x.GetBetAmount()).Returns(50);
            _contextInstance.Setup(x => x.GetDenom()).Returns(1);
            _contextInstance.Setup(x => x.MathFile).Returns(new Config("Math96"));
            _configsInstance = new Configs() { IsTest = true };
            _resolver = new MermaidsFortuneResolver(_contextInstance.Object, _configsInstance, _random.Object);

        }

        [Theory]
        [InlineData(1, 50)]
        [InlineData(2, 10)]
        public void EvaluateResult_Regular(int symbol, int winAmount)
        {
            Result res = new Result();
            res.Wins = new List<Win>();
            var xx = new ItemOnReel
            {
                Symbol = symbol,
                Reel = 0,
                Index = 1
            };
            var yy = new ItemOnReel
            {
                Symbol = symbol,
                Reel = 1,
                Index = 2
            };
            var zz = new ItemOnReel
            {
                Symbol = 4,
                Reel = 2,
                Index = 3
            };
            res.Wins.Add(new Win
            {
                WinType = WinType.Regular,
                Symbol = symbol,
                Ways = 1,
                WinningLines = new HashSet<ItemOnReel>( new List<ItemOnReel>{xx, yy, zz})
            });
            var x = new HashSet<int>(new List<int> {1, 2, 3});
            _resolver.EvaluateResult(res);
            Assert.Equal(winAmount, res.WonAmount);
        }

        [Theory]
        [InlineData(10, 50000)]
        [InlineData(11, 10000)]
        [InlineData(13, 300000)]
        public void EvaluateResult_FiveOfAKind(int symbol, int winAmount)
        {
            Result res = new Result();
            res.Wins = new List<Win>();
            res.Wins.Add(new Win
            {
                WinType = WinType.FiveOfAKind,
                Symbol = 9,
                Ways = 1
            });
            res.McSymbols.Add(new ItemOnReel
            {
                Symbol = symbol
            });
            _resolver.EvaluateResult(res);
            Assert.Equal(winAmount, res.WonAmount);
        }

        [Fact]
        public void EvaluateResult_FreeSpin()
        {
            Result res = new Result();
            res.Wins = new List<Win>();
            res.Wins.Add(new Win
            {
                WinType = WinType.FreeSpin,
                Symbol = MermaidsFortuneResolver.SCATTER,
                Ways = 1
            });
            res.Scatter.Add(new ItemOnReel());
            res.Scatter.Add(new ItemOnReel());
            res.Scatter.Add(new ItemOnReel());
            _resolver.EvaluateResult(res);
            Assert.Equal(_contextInstance.Object.State.freeSpinsLeft, 8);
        }
    }
}
