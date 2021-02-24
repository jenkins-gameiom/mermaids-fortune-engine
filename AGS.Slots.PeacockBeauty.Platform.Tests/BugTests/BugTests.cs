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

namespace AGS.Slots.MermaidsFortune.Platform.Tests.BugTests
{
    public class BugTests
    {
        private Spins _spinsInstance;
        private Init _initsInstance;
        private Configs _configsInstance;
        private GameEngine _gameEngine;
        private MermaidsFortuneScanner _scanner;
        private MermaidsFortuneResolver _resolver;
        private Mock<IIndex<RandomizerType, IRandom>> _randomIndex;
        private Mock<IRandom> _randomService;
        private Mock<IRequestContext> _contextInstance;
        private IMathFile _mathFile;


        public BugTests()
        {
            _configsInstance = new Configs() { IsTest = true };

            _randomService = new Mock<IRandom>();
            _randomIndex = new Mock<IIndex<RandomizerType, IRandom>>();
            _randomIndex.Setup(i => i[It.IsAny<RandomizerType>()]).Returns(_randomService.Object);
            _contextInstance = new Mock<IRequestContext>();
            _resolver = new MermaidsFortuneResolver(_contextInstance.Object, _configsInstance, _randomIndex.Object);
            _scanner = new MermaidsFortuneScanner(_contextInstance.Object, _randomIndex.Object, _configsInstance);
            _gameEngine = new GameEngine(_contextInstance.Object, _resolver, _scanner, _configsInstance, _randomIndex.Object);
            _spinsInstance = new Spins(_contextInstance.Object, _configsInstance, _gameEngine);
            _initsInstance = new Init(_contextInstance.Object, _gameEngine);
            _mathFile = new Logic.Engine.MermaidsFortune.Config("Math96");
            _contextInstance.Setup(a => a.MathFile).Returns(_mathFile);
        }

        [Fact]
        public void NoRtp()
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq("NoRtp"));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(a => a.Config).Returns(req.Config);
            _contextInstance.Setup(a => a.RequestItems).Returns(new RequestItems { betAmount = 88 });
            _randomService.Setup(a => a.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(3);
            _randomService.Setup(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(BonusGameHelper.GenerateRandomNumbers(11));
            var dynamicRequest = Json.ObjectToDynamic(req);
            var result = _initsInstance.InitSlot(dynamicRequest);
        }
    }
}
