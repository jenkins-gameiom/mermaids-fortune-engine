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
            _contextInstance.Setup(x => x.MathFile).Returns(new Config("Math96"));
            _configsInstance = new Configs() { IsTest = true };
            _resolver = new MermaidsFortuneResolver(_contextInstance.Object, _configsInstance, _random.Object);

        }

        [Fact]
        public void EvaluateResult_Regular()
        {
            Result res = new Result();
            res.Wins = new List<Win>();
            var xx = new ItemOnReel
            {
                Symbol = 1,
                Reel = 0
            };
            var yy = new ItemOnReel
            {
                Symbol = 1,
                Reel = 1
            };
            var zz = new ItemOnReel
            {
                Symbol = 1,
                Reel = 2
            };
            res.Wins.Add(new Win
            {
                WinType = WinType.Regular,
                Symbol = 1,
                Ways = 1,
                WinningLines = new HashSet<ItemOnReel>()
            });
            res.Wins.First().WinningLines.Add(xx);
            res.Wins.First().WinningLines.Add(yy);
            res.Wins.First().WinningLines.Add(zz);
            _resolver.EvaluateResult(res);
            Assert.Equal(50000, res.WonAmount);
        }

        [Fact]
        public void EvaluateResult_FiveOfAKind()
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
                Symbol = 10
            });
            _resolver.EvaluateResult(res);
            Assert.Equal(50000, res.WonAmount);
        }
    }
}
