using System;
using System.Collections.Generic;
using System.Text;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Helpers;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic;
using AGS.Slots.MermaidsFortune.Logic.Engine;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;
using Autofac.Features.Indexed;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AGS.Slots.MermaidsFortune.Platform.Tests
{
    public class PickTest
    {
        private BonusPick _pickInstance;
        private Configs _configsInstance;
        private IBonusGameService _bonusGameService;
        private MermaidsFortuneScanner _scanner;
        private MermaidsFortuneResolver _resolver;
        private Mock<IIndex<RandomizerType, IRandom>> _randomIndex;
        private Mock<IRandom> _randomService;
        private Mock<IRequestContext> _contextInstance;
        private IMathFile _mathFile;

        public PickTest()
        {
            _configsInstance = new Configs() { IsTest = true };
            _randomService = new Mock<IRandom>();
            _randomIndex = new Mock<IIndex<RandomizerType, IRandom>>();
            _randomIndex.Setup(i => i[It.IsAny<RandomizerType>()]).Returns(_randomService.Object);
            _contextInstance = new Mock<IRequestContext>();
            //_resolver = new MermaidsFortuneResolver(_contextInstance.Object, _configsInstance, _randomIndex.Object);
            //_scanner = new MermaidsFortuneScanner(_contextInstance.Object, _randomIndex.Object, _configsInstance);
            _bonusGameService = new BonusGameService(_contextInstance.Object, _configsInstance, _randomIndex.Object);
            _pickInstance = new BonusPick(_contextInstance.Object, _bonusGameService);
            _mathFile = new Logic.Engine.MermaidsFortune.Config("Math96");
            _contextInstance.Setup(a => a.MathFile).Returns(_mathFile);
        }

        //In this test, if no exception occureed we passed the test.
        [Fact]
        public void Pick_BasicTestValid()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Pick1"));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems {betAmount = 88});
            _randomService.Setup(a => a.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(3);
            _randomService.Setup(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(BonusGameHelper.GenerateRandomNumbers(11));
            var dynamicRequest = Json.ObjectToDynamic(req);
            _pickInstance.Pick(dynamicRequest);
        }

        [Fact]
        public void Pick_Complete_False()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Pick1"));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems { betAmount = 88 });
            _randomService.Setup(a => a.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(3);
            _randomService.Setup(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(BonusGameHelper.GenerateRandomNumbers(11));
            var dynamicRequest = Json.ObjectToDynamic(req);
            _pickInstance.Pick(dynamicRequest);
        }

        [Fact]
        public void Pick_Complete_True()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Pick1_Complete_True"));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems { betAmount = 88 });
            _randomService.Setup(a => a.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(3);
            _randomService.Setup(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(BonusGameHelper.GenerateRandomNumbers(13));
            var dynamicRequest = Json.ObjectToDynamic(req);
            _pickInstance.Pick(dynamicRequest);
            Assert.True(_contextInstance.Object.State.completed);
        }

        [Fact]
        public void Pick_BonusGameNull()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Pick1"));
            req.PrivateState.BonusGame = null;
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems { betAmount = 88 });
            _randomService.Setup(a => a.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(3);
            _randomService.Setup(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(BonusGameHelper.GenerateRandomNumbers(11));
            var dynamicRequest = Json.ObjectToDynamic(req);

            string answer = _pickInstance.Pick(dynamicRequest).ToString();
            Assert.True(answer.ToLower().Contains("error in validating bonus"));
        }

        [Fact]
        public void Pick_BonusGameComplete_Or_NoBonusGameWon()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("Pick1"));
            req.PrivateState.completed = true;
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems { betAmount = 88 });
            _randomService.Setup(a => a.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(3);
            _randomService.Setup(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(BonusGameHelper.GenerateRandomNumbers(11));
            var dynamicRequest = Json.ObjectToDynamic(req);

            string answer = _pickInstance.Pick(dynamicRequest).ToString();
            Assert.True(answer.ToLower().Contains("error in validating bonus"));
        }
    }
}
