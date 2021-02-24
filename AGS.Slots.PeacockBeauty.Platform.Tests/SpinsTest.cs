using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic;
using AGS.Slots.MermaidsFortune.Logic.Engine;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using Autofac.Features.Indexed;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;
using Xunit;

namespace AGS.Slots.MermaidsFortune.Platform.Tests
{
    public class SpinsTest
    {

        private Spins _spinsInstance;
        private Configs _configsInstance;
        private GameEngine _gameEngine;
        private MermaidsFortuneScanner _scanner;
        private MermaidsFortuneResolver _resolver;
        private Mock<IIndex<RandomizerType, IRandom>> _randomIndex;
        private Mock<IRandom> _randomService;
        private Mock<IRequestContext> _contextInstance;
        private IMathFile _mathFile;


        public SpinsTest() 
        {
            _configsInstance = new Configs() { IsTest=true};

            _randomService = new Mock<IRandom>();
            _randomIndex = new Mock<IIndex<RandomizerType, IRandom>>();
            _randomIndex.Setup(i => i[It.IsAny<RandomizerType>()]).Returns(_randomService.Object);
            _contextInstance = new Mock<IRequestContext>();
            _resolver = new MermaidsFortuneResolver(_contextInstance.Object,_configsInstance, _randomIndex.Object);
            _scanner = new MermaidsFortuneScanner(_contextInstance.Object, _randomIndex.Object, _configsInstance);
            _gameEngine = new GameEngine(_contextInstance.Object, _resolver, _scanner,_configsInstance, _randomIndex.Object);
            _spinsInstance = new Spins(_contextInstance.Object, _configsInstance, _gameEngine);
            _mathFile = new Logic.Engine.MermaidsFortune.Config("Math96");
            _contextInstance.Setup(a => a.MathFile).Returns(_mathFile);
            _contextInstance.Setup(a => a.GetBetAmount()).Returns(88);
            _contextInstance.Setup(a => a.GetDenom()).Returns(1);
        }

        [Fact]
        public void Spin_Basic_TransactionsValid()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Spin1"));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems
            {
                denom = 1,
                betAmount = 88,
                isFreeSpin = false
            });
            var dynamicRequest = Json.ObjectToDynamic(req);
            _randomService.SetupSequence(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
              .Returns(new List<RandomNumber>(new RandomNumber[] {
                new RandomNumber() {Min=0,Max=57,Quantity=1,Values=new List<int>(new int[]{56 })}, //5,3,2,7
                new RandomNumber() {Min=0,Max=61,Quantity=1,Values=new List<int>(new int[]{59 })},
                new RandomNumber() {Min=0,Max=71,Quantity=1,Values=new List<int>(new int[]{65 })},
                new RandomNumber() {Min=0,Max=111,Quantity=1,Values=new List<int>(new int[]{60 })},
                new RandomNumber() {Min=0,Max=104,Quantity=1,Values=new List<int>(new int[]{78 })} }))
              .Returns(new List<RandomNumber>(new RandomNumber[] {
                new RandomNumber() {Min=0,Max=57,Quantity=1,Values=new List<int>(new int[]{0 })},
                new RandomNumber() {Min=0,Max=61,Quantity=1,Values=new List<int>(new int[]{0 })},
                new RandomNumber() {Min=0,Max=71,Quantity=1,Values=new List<int>(new int[]{0})},
                new RandomNumber() {Min=0,Max=111,Quantity=1,Values=new List<int>(new int[]{60 })},
                new RandomNumber() {Min=0,Max=104,Quantity=1,Values=new List<int>(new int[]{12 })} })); ;
            var result  = _spinsInstance.Spin(dynamicRequest);

            Assert.Equal(result.transactions.credits[0].Value, 28);
            Assert.Equal(result.transactions.debits[0].Value, 88);
        }

        [Fact]
        public void Spin_Basic_TransactionsCreditsInvalid()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Spin1"));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems
            {
                denom = 1,
                betAmount = 88,
                isFreeSpin = false
            });
            var dynamicRequest = Json.ObjectToDynamic(req);
            _randomService.SetupSequence(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(new List<RandomNumber>(new RandomNumber[]
                {
                    new RandomNumber()
                        {Min = 0, Max = 57, Quantity = 1, Values = new List<int>(new int[] {56})}, //5,3,2,7
                    new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                    new RandomNumber() {Min = 0, Max = 71, Quantity = 1, Values = new List<int>(new int[] {65})},
                    new RandomNumber() {Min = 0, Max = 111, Quantity = 1, Values = new List<int>(new int[] {60})},
                    new RandomNumber() {Min = 0, Max = 104, Quantity = 1, Values = new List<int>(new int[] {78})}
                }));
            var result = _spinsInstance.Spin(dynamicRequest);

            Assert.NotEqual(result.transactions.credits[0].Value, 48);
            Assert.Equal(result.transactions.debits[0].Value, 88);
        }
        [Fact]
        public void Spin_Basic_TransactionsDebitsInvalid()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Spin1"));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems
            {
                denom = 1,
                betAmount = 88,
                isFreeSpin = false
            });
            var dynamicRequest = Json.ObjectToDynamic(req);
            _randomService.SetupSequence(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(new List<RandomNumber>(new RandomNumber[]
                {
                    new RandomNumber()
                        {Min = 0, Max = 57, Quantity = 1, Values = new List<int>(new int[] {56})}, //5,3,2,7
                    new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                    new RandomNumber() {Min = 0, Max = 71, Quantity = 1, Values = new List<int>(new int[] {65})},
                    new RandomNumber() {Min = 0, Max = 111, Quantity = 1, Values = new List<int>(new int[] {60})},
                    new RandomNumber() {Min = 0, Max = 104, Quantity = 1, Values = new List<int>(new int[] {78})}
                }));
            var result = _spinsInstance.Spin(dynamicRequest);

            Assert.Equal(result.transactions.credits[0].Value, 28);
            Assert.NotEqual(result.transactions.debits[0].Value, 176);
        }
        [Fact]
        public void Spin_Basic_BetAmountInvalid()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Spin1"));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems
            {
                denom = 1,
                betAmount = 92,
                isFreeSpin = false
            });
            _contextInstance.Setup(a => a.GetBetAmount()).Returns(92);
            var dynamicRequest = Json.ObjectToDynamic(req);
            _randomService.SetupSequence(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(new List<RandomNumber>(new RandomNumber[]
                {
                    new RandomNumber()
                        {Min = 0, Max = 57, Quantity = 1, Values = new List<int>(new int[] {56})}, //5,3,2,7
                    new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                    new RandomNumber() {Min = 0, Max = 71, Quantity = 1, Values = new List<int>(new int[] {65})},
                    new RandomNumber() {Min = 0, Max = 111, Quantity = 1, Values = new List<int>(new int[] {60})},
                    new RandomNumber() {Min = 0, Max = 104, Quantity = 1, Values = new List<int>(new int[] {78})}
                }));
            var result = _spinsInstance.Spin(dynamicRequest);
            string resultInString = result.ToString();
            Assert.True(resultInString.Contains("Error betAmount not valid"));
        }
        [Fact]
        public void Spin_Basic_DenomInvalid()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Spin1"));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems
            {
                denom = 33,
                betAmount = 88,
                isFreeSpin = false
            });
            _contextInstance.Setup(a => a.GetDenom()).Returns(3);
            var dynamicRequest = Json.ObjectToDynamic(req);
            _randomService.SetupSequence(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(new List<RandomNumber>(new RandomNumber[]
                {
                    new RandomNumber()
                        {Min = 0, Max = 57, Quantity = 1, Values = new List<int>(new int[] {56})}, //5,3,2,7
                    new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                    new RandomNumber() {Min = 0, Max = 71, Quantity = 1, Values = new List<int>(new int[] {65})},
                    new RandomNumber() {Min = 0, Max = 111, Quantity = 1, Values = new List<int>(new int[] {60})},
                    new RandomNumber() {Min = 0, Max = 104, Quantity = 1, Values = new List<int>(new int[] {78})}
                }));
            var result = _spinsInstance.Spin(dynamicRequest);
            string resultInString = result.ToString();
            Assert.True(resultInString.Contains("Error chipsPerPlay not valid"));
        }

        [Fact]
        public void Spin_FreeSpin_FreespinsLeft0()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Spin1"));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Object.State.freeSpinsLeft = 0;
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems
            {
                denom = 1,
                betAmount = 88,
                isFreeSpin = true
            });
            var dynamicRequest = Json.ObjectToDynamic(req);
            _randomService.SetupSequence(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(new List<RandomNumber>(new RandomNumber[]
                {
                    new RandomNumber()
                        {Min = 0, Max = 57, Quantity = 1, Values = new List<int>(new int[] {56})}, //5,3,2,7
                    new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                    new RandomNumber() {Min = 0, Max = 71, Quantity = 1, Values = new List<int>(new int[] {65})},
                    new RandomNumber() {Min = 0, Max = 111, Quantity = 1, Values = new List<int>(new int[] {60})},
                    new RandomNumber() {Min = 0, Max = 104, Quantity = 1, Values = new List<int>(new int[] {78})}
                }));
            var result = _spinsInstance.Spin(dynamicRequest);
            string resultInString = result.ToString();
            Assert.True(resultInString.Contains("Error in validating freeSpins"));
        }

        [Fact]
        public void Spin_ShouldBeInFreeSpins()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Spin1"));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Object.State.freeSpinsLeft = 6;
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems
            {
                denom = 1,
                betAmount = 88,
                isFreeSpin = false
            });
            var dynamicRequest = Json.ObjectToDynamic(req);
            _randomService.SetupSequence(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(new List<RandomNumber>(new RandomNumber[]
                {
                    new RandomNumber()
                        {Min = 0, Max = 57, Quantity = 1, Values = new List<int>(new int[] {56})}, //5,3,2,7
                    new RandomNumber() {Min = 0, Max = 61, Quantity = 1, Values = new List<int>(new int[] {59})},
                    new RandomNumber() {Min = 0, Max = 71, Quantity = 1, Values = new List<int>(new int[] {65})},
                    new RandomNumber() {Min = 0, Max = 111, Quantity = 1, Values = new List<int>(new int[] {60})},
                    new RandomNumber() {Min = 0, Max = 104, Quantity = 1, Values = new List<int>(new int[] {78})}
                }));
            var result = _spinsInstance.Spin(dynamicRequest);
            string resultInString = result.ToString();
            Assert.True(resultInString.Contains("Should be in freeSpins"));
        }

        [Fact]
        public void Spin_Force_3BN()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Spin1"));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems
            {
                denom = 1,
                betAmount = 88,
                isFreeSpin = false,
                force = "3bnandnowin"
            });
            var dynamicRequest = Json.ObjectToDynamic(req);
            _randomService.SetupSequence(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
              .Returns(new List<RandomNumber>(new RandomNumber[] {
                new RandomNumber() {Min=0,Max=57,Quantity=1,Values=new List<int>(new int[]{56 })}, //5,3,2,7
                new RandomNumber() {Min=0,Max=61,Quantity=1,Values=new List<int>(new int[]{59 })},
                new RandomNumber() {Min=0,Max=71,Quantity=1,Values=new List<int>(new int[]{65 })},
                new RandomNumber() {Min=0,Max=111,Quantity=1,Values=new List<int>(new int[]{60 })},
                new RandomNumber() {Min=0,Max=104,Quantity=1,Values=new List<int>(new int[]{78 })} }))
              .Returns(new List<RandomNumber>(new RandomNumber[] {
                new RandomNumber() {Min=0,Max=57,Quantity=1,Values=new List<int>(new int[]{0 })},
                new RandomNumber() {Min=0,Max=61,Quantity=1,Values=new List<int>(new int[]{0 })},
                new RandomNumber() {Min=0,Max=71,Quantity=1,Values=new List<int>(new int[]{0})},
                new RandomNumber() {Min=0,Max=111,Quantity=1,Values=new List<int>(new int[]{60 })},
                new RandomNumber() {Min=0,Max=104,Quantity=1,Values=new List<int>(new int[]{12 })} })); ;
            var result = _spinsInstance.Spin(dynamicRequest);

            var featureType = (string)result.privateState.lastState.spin.wins[0].featureType;
            Assert.Equal(featureType, "freeSpinWin");
            Assert.Equal((int)result.privateState.lastState.spin.wins[0].winAmount, 88);
        }

        [Fact]
        public void Spin_Force_4BN()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Spin1"));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems
            {
                denom = 1,
                betAmount = 88,
                isFreeSpin = false,
                force = "4bnandnowin"
            });
            var dynamicRequest = Json.ObjectToDynamic(req);
            _randomService.SetupSequence(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
              .Returns(new List<RandomNumber>(new RandomNumber[] {
                new RandomNumber() {Min=0,Max=57,Quantity=1,Values=new List<int>(new int[]{56 })}, //5,3,2,7
                new RandomNumber() {Min=0,Max=61,Quantity=1,Values=new List<int>(new int[]{59 })},
                new RandomNumber() {Min=0,Max=71,Quantity=1,Values=new List<int>(new int[]{65 })},
                new RandomNumber() {Min=0,Max=111,Quantity=1,Values=new List<int>(new int[]{60 })},
                new RandomNumber() {Min=0,Max=104,Quantity=1,Values=new List<int>(new int[]{78 })} }))
              .Returns(new List<RandomNumber>(new RandomNumber[] {
                new RandomNumber() {Min=0,Max=57,Quantity=1,Values=new List<int>(new int[]{0 })},
                new RandomNumber() {Min=0,Max=61,Quantity=1,Values=new List<int>(new int[]{0 })},
                new RandomNumber() {Min=0,Max=71,Quantity=1,Values=new List<int>(new int[]{0})},
                new RandomNumber() {Min=0,Max=111,Quantity=1,Values=new List<int>(new int[]{60 })},
                new RandomNumber() {Min=0,Max=104,Quantity=1,Values=new List<int>(new int[]{12 })} })); ;
            var result = _spinsInstance.Spin(dynamicRequest);

            var featureType = (string)result.privateState.lastState.spin.wins[0].featureType;
            Assert.Equal(featureType, "freeSpinWin");
            Assert.Equal((int)result.privateState.lastState.spin.wins[0].winAmount, 176);
        }

        [Fact]
        public void Spin_Force_5BN()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Spin1"));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems
            {
                denom = 1,
                betAmount = 88,
                isFreeSpin = false,
                force = "5bnandnowin"
            });
            var dynamicRequest = Json.ObjectToDynamic(req);
            _randomService.SetupSequence(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
              .Returns(new List<RandomNumber>(new RandomNumber[] {
                new RandomNumber() {Min=0,Max=57,Quantity=1,Values=new List<int>(new int[]{56 })}, //5,3,2,7
                new RandomNumber() {Min=0,Max=61,Quantity=1,Values=new List<int>(new int[]{59 })},
                new RandomNumber() {Min=0,Max=71,Quantity=1,Values=new List<int>(new int[]{65 })},
                new RandomNumber() {Min=0,Max=111,Quantity=1,Values=new List<int>(new int[]{60 })},
                new RandomNumber() {Min=0,Max=104,Quantity=1,Values=new List<int>(new int[]{78 })} }))
              .Returns(new List<RandomNumber>(new RandomNumber[] {
                new RandomNumber() {Min=0,Max=57,Quantity=1,Values=new List<int>(new int[]{0 })},
                new RandomNumber() {Min=0,Max=61,Quantity=1,Values=new List<int>(new int[]{0 })},
                new RandomNumber() {Min=0,Max=71,Quantity=1,Values=new List<int>(new int[]{0})},
                new RandomNumber() {Min=0,Max=111,Quantity=1,Values=new List<int>(new int[]{60 })},
                new RandomNumber() {Min=0,Max=104,Quantity=1,Values=new List<int>(new int[]{12 })} })); ;
            var result = _spinsInstance.Spin(dynamicRequest);

            var featureType = (string)result.privateState.lastState.spin.wins[0].featureType;
            Assert.Equal(featureType, "freeSpinWin");
            Assert.Equal((int)result.privateState.lastState.spin.wins[0].winAmount, 440);
        }
    }
}
