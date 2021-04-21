using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
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
    public class ScannerTests
    {

        private readonly Mock<IRandom> _random;
        private readonly Config _config;
        private Mock<IRequestContext> _contextInstance;
        private MermaidsFortuneScanner _scanner;
        private Configs _configsInstance;

        public ScannerTests()
        {
            _random = new Mock<IRandom>();
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
                stakes = new List<int> { 50, 100, 150, 250, 500 },
                denominations = new List<int> { 1 }
            });
            _contextInstance.Setup(x => x.GetBetAmount()).Returns(50);
            _contextInstance.Setup(x => x.GetDenom()).Returns(1);
            _contextInstance.Setup(x => x.MathFile).Returns(new Config("Math96"));
            _configsInstance = new Configs() { IsTest = true };
            _scanner = new MermaidsFortuneScanner(_contextInstance.Object, _random.Object, _configsInstance);
        }

        [Fact]
        public void EvaluateResult_FreeSpin()
        {
            Result res = new Result();
            var resultMatrix = GetResult();
            resultMatrix[0][0] = MermaidsFortuneResolver.SCATTER;
            resultMatrix[1][0] = MermaidsFortuneResolver.SCATTER;
            resultMatrix[2][0] = MermaidsFortuneResolver.SCATTER;
            _scanner.ApplyResultion(resultMatrix, res);
            _scanner.Scan(res);
            Assert.Contains(res.Wins, item => item.WinType == WinType.FreeSpin);
        }
        [Theory]
        [InlineData(3, WinType.Regular)]
        [InlineData(4, WinType.Regular)]
        [InlineData(5, WinType.FiveOfAKind)]
        public void EvaluateResult_Oak(int diamondsAmount, WinType winType)
        {
            Result res = new Result();
            var resultMatrix = GetResult();
            resultMatrix[0][0] = MermaidsFortuneResolver.MCSymbol;
            resultMatrix[1][0] = MermaidsFortuneResolver.MCSymbol;
            resultMatrix[2][0] = 11;
            if (diamondsAmount > 3)
            {
                resultMatrix[3][0] = MermaidsFortuneResolver.MCSymbol;
            }

            if (diamondsAmount > 4)
            {
                resultMatrix[4][0] = MermaidsFortuneResolver.MCSymbol;
            }
            _scanner.ApplyResultion(resultMatrix, res);
            _scanner.Scan(res);
            Assert.Contains(res.Wins, item => item.WinType == winType);
        }

        private List<List<int>> GetResult()
        {
            var listToReturn = new List<List<int>>();
            listToReturn.Add(new List<int>{1,2,3});
            listToReturn.Add(new List<int> { 1, 2, 3 });
            listToReturn.Add(new List<int> { 1, 2, 3, 4 });
            listToReturn.Add(new List<int> { 1, 2, 3 });
            listToReturn.Add(new List<int> { 1, 2, 3 });
            return listToReturn;
        }
    }
}
