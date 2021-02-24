using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;
using Autofac.Features.Indexed;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Helpers;
using Xunit;
using FluentAssertions;

namespace AGS.Slots.MermaidsFortune.Logic.Tests
{
    public class BonusGameServiceTests
    {
        private Mock<IIndex<RandomizerType, IRandom>> _randomIndex;
        private Mock<IRandom> _randomService;
        private readonly Engine.MermaidsFortune.Config _config;
        private readonly BonusGameService _bonusServiceInstance;
        private readonly IRequestContext _context;
        private Configs _configsInstance;



        public BonusGameServiceTests()
        {
            _configsInstance = new Configs() { IsTest = true };

            _randomService = new Mock<IRandom>();
            _randomIndex = new Mock<IIndex<RandomizerType, IRandom>>();
            _randomIndex.Setup(i => i[It.IsAny<RandomizerType>()]).Returns(_randomService.Object);
            _randomService.Setup(a => a.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(6);
            _config = new Engine.MermaidsFortune.Config("Math96");
            _context = new RequestConextImpl() { State = new State(), MathFile = _config, RequestItems = new RequestItems { betAmount = 88, denom = 1 } };
            _bonusServiceInstance = new BonusGameService(_context, _configsInstance, _randomIndex.Object);
        }


        

        private List<MCSymbol> GenerateMCSymbolsFromSpin()
        {
            List<MCSymbol> mcSymbolsList = new List<MCSymbol>();
            mcSymbolsList.Add(new MCSymbol(5, 4, 0, 13, true, TableTypeEnum.Regular, 68));
            mcSymbolsList.Add(new MCSymbol(2, 1, 0, 13, true, TableTypeEnum.Regular, 128));
            mcSymbolsList.Add(new MCSymbol(4, 3, 0, 13, true, TableTypeEnum.Regular, 128));
            mcSymbolsList.Add(new MCSymbol(9, 3, 1, 13, true, TableTypeEnum.Regular, 688));
            mcSymbolsList.Add(new MCSymbol(14, 3, 2, 13, true, TableTypeEnum.Regular, 28));
            mcSymbolsList.Add(new MCSymbol(19, 3, 3, 13, true, TableTypeEnum.Regular, 888));
            mcSymbolsList.Add(new MCSymbol(20, 4, 3, 13, true, TableTypeEnum.Regular, 28));
            return mcSymbolsList;
        }
        [Fact]
        public void SpinAll_LockedFromSpinStillLocked()
        {
            //ARRANGE
            _context.State.BonusGame = new BonusGame();
            _context.State.BonusGame.MCSymbols = GenerateMCSymbolsFromSpin();
            BonusGameHelper.GenerateRandomNumbers(1);
            List<int> expectedLockedIndexes = _context.State.BonusGame.MCSymbols.Select(s => s.index).ToList();
            _randomService.Setup(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(BonusGameHelper.GenerateRandomNumbers(57));
            _randomService.Setup(a => a.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(6);

            //ACT
            _bonusServiceInstance.SpinAll();

            //ASSERT
            for (int i = 0; i < expectedLockedIndexes.Count; i++)
            {
                var item = _context.State.BonusGame.MCSymbols.First(x => x.index == expectedLockedIndexes[i]);
                item.IsLocked.Should().Be(true);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void SpinAll_InstantWinPaysMoney(int amountOfTimes)
        {
            //ARRANGE
            _context.State.BonusGame = new BonusGame();
            _context.State.BonusGame.MCSymbols = GenerateMCSymbolsFromSpin();
            List<int> expectedLockedIndexes = _context.State.BonusGame.MCSymbols.Select(s => s.index).ToList();
            var counter = 0;
            _randomService.Setup(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(() =>
                {
                    if (counter < amountOfTimes)
                    {
                        counter++;
                        return BonusGameHelper.GenerateRandomNumbers(2);

                    }
                    else
                    {
                        return BonusGameHelper.GenerateRandomNumbers(5);
                    }
                });
            _randomService.Setup(a => a.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(6);

            //ACT
            int spinAllTimes = 0;
            long expectedMoneyToPay = 0;
            while (!_context.State.BonusGame.complete && spinAllTimes < amountOfTimes)
            {
                _bonusServiceInstance.SpinAll();
                expectedMoneyToPay += _context.State.BonusGame.MCSymbols.Sum(x => x.winAmount);
                spinAllTimes++;
            }
            //ASSERT
            _context.State.BonusGame.winAmount.Should().Be(expectedMoneyToPay);
        }

        [Fact]
        public void SpinAll_X2Valid_First()
        {
            //ARRANGE
            _context.State.BonusGame = new BonusGame();
            _context.State.BonusGame.MCSymbols = GenerateMCSymbolsFromSpin();
            var sumBefore = _context.State.BonusGame.MCSymbols.Sum(x => x.winAmount);
            List<int> expectedLockedIndexes = _context.State.BonusGame.MCSymbols.Select(s => s.index).ToList();
            _randomService.Setup(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(BonusGameHelper.GenerateRandomNumbers(11));
            _randomService.Setup(a => a.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(6);

            //ACT
            var happenedOnce = false;
            while (!_context.State.BonusGame.complete)
            {
                _bonusServiceInstance.SpinAll();
            }
            //ASSERT
            var actualMoneyToPay = _context.State.BonusGame.winAmount;
            _context.State.BonusGame.MCSymbols.Sum(x => x.winAmount).Should().Be(actualMoneyToPay);
            _context.State.BonusGame.MCSymbols.First(x => x.index == 20).winAmount.Should().Be(28 * 2);
            _context.State.BonusGame.X2IndexesToRemove.Should().Be(RemoveX2Enum.First);
        }

        [Fact]
        public void SpinAll_X2Valid_Second()
        {
            //ARRANGE
            _context.State.BonusGame = new BonusGame();
            _context.State.BonusGame.MCSymbols = GenerateMCSymbolsFromSpin();
            var sumBefore = _context.State.BonusGame.MCSymbols.Sum(x => x.winAmount);
            List<int> expectedLockedIndexes = _context.State.BonusGame.MCSymbols.Select(s => s.index).ToList();
            _randomService.Setup(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(BonusGameHelper.GenerateRandomNumbers(40));
            _randomService.Setup(a => a.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(6);

            //ACT
            var happenedOnce = false;
            while (!_context.State.BonusGame.complete)
            {
                _bonusServiceInstance.SpinAll();
            }
            //ASSERT
            _context.State.BonusGame.MCSymbols.Sum(x => x.winAmount).Should().Be(_context.State.BonusGame.winAmount);

            _context.State.BonusGame.MCSymbols.First(x => x.index == 20).winAmount.Should().Be( 28 * 2);
            _context.State.BonusGame.X2IndexesToRemove.Should().Be(RemoveX2Enum.Second);

        }

        [Fact]
        public void SpinAll_X2Valid_Both()
        {
            //ARRANGE
            _context.State.BonusGame = new BonusGame();
            _context.State.BonusGame.MCSymbols = GenerateMCSymbolsFromSpin();
            var sumBefore = _context.State.BonusGame.MCSymbols.Sum(x => x.winAmount);
            List<int> expectedLockedIndexes = _context.State.BonusGame.MCSymbols.Select(s => s.index).ToList();
            var firstTime = true;
            _randomService.Setup(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(() =>
                {
                    if (firstTime)
                    {
                        firstTime = false;
                        return BonusGameHelper.GenerateRandomNumbers(40);

                    }
                    else
                    {
                        return BonusGameHelper.GenerateRandomNumbers(11);
                    }
                });
            _randomService.Setup(a => a.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(6);

            //ACT
            while (!_context.State.BonusGame.complete)
            {
                _bonusServiceInstance.SpinAll();
            }
            //ASSERT
            _context.State.BonusGame.MCSymbols.Sum(x => x.winAmount).Should().Be(_context.State.BonusGame.winAmount);
            _context.State.BonusGame.MCSymbols.First(x => x.index == 20).winAmount.Should().Be( 28 * 4);
            _context.State.BonusGame.X2IndexesToRemove.Should().Be(RemoveX2Enum.Both);

        }

        [Fact]
        public void SpinAll_X8Valid()
        {
            //ARRANGE
            _context.State.BonusGame = new BonusGame();
            _context.State.BonusGame.MCSymbols = GenerateMCSymbolsFromSpin();
            var sumBefore = _context.State.BonusGame.MCSymbols.Sum(x => x.winAmount);
            List<int> expectedLockedIndexes = _context.State.BonusGame.MCSymbols.Select(s => s.index).ToList();
            _randomService.Setup(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(BonusGameHelper.GenerateRandomNumbers(49));
            _randomService.Setup(a => a.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(6);

            //ACT
            var happenedOnce = false;
            while (!_context.State.BonusGame.complete)
            {
                _bonusServiceInstance.SpinAll();
            }
            //ASSERT
            _context.State.BonusGame.MCSymbols.Sum(x => x.winAmount).Should().Be(_context.State.BonusGame.winAmount);
            _context.State.BonusGame.MCSymbols.First(x => x.index == 20).winAmount.Should().Be( 28 * 8);
            _context.State.BonusGame.removeX8.Should().Be(true);

        }
        public class RequestConextImpl : IRequestContext
        {
            public IStateItems State { get; set; }
            public Common.Entities.Config Config { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public IMathFile MathFile { get; set; }
            public RequestItems RequestItems { get; set; }
            public int GetDenom()
            {
                if (RequestItems.isFreeSpin)
                {
                    return State.lastState.denom.Value;
                }
                else
                {
                    return RequestItems.denom;
                }
            }

            public int GetBetAmount()
            {
                if (RequestItems.isFreeSpin)
                {
                    return State.lastState.betAmount.Value;
                }
                else
                {
                    return RequestItems.betAmount;
                }
            }
        }

        
    }
}
