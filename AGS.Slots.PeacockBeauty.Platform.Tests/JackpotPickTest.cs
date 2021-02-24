using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Helpers;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;
using AGS.Slots.MermaidsFortune.Logic.Tests;
using Autofac.Features.Indexed;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AGS.Slots.MermaidsFortune.Platform.Tests
{
    public class JackpotPickTest
    {
        private readonly string prefix = "JackpotPick/";
        private JackpotPick _jackpotPickInstance;
        private Configs _configsInstance;
        private IJackpotService _jackpotService;
        private MermaidsFortuneScanner _scanner;
        private MermaidsFortuneResolver _resolver;
        private Mock<IIndex<RandomizerType, IRandom>> _randomIndex;
        private Mock<IRandom> _randomService;
        private Mock<IRequestContext> _contextInstance;
        private IMathFile _mathFile;

        public JackpotPickTest()
        {
            _configsInstance = new Configs() { IsTest = true };
            _randomService = new Mock<IRandom>();
            _randomIndex = new Mock<IIndex<RandomizerType, IRandom>>();
            _randomIndex.Setup(i => i[It.IsAny<RandomizerType>()]).Returns(_randomService.Object);

            _contextInstance = new Mock<IRequestContext>();
            //_contextInstance.State = new BonusGameServiceTests.State();
            //_contextInstance.State.JackpotGame = new JackpotGame();
            //_contextInstance.State.JackpotGame.selectedItems = new List<JackpotItem>();
            _contextInstance.Setup(x => x.MathFile).Returns(new Logic.Engine.MermaidsFortune.Config("Math96"));
            _contextInstance.Setup(x => x.GetDenom()).Returns(1);
            _jackpotService = new JackpotService(_contextInstance.Object);
            _jackpotPickInstance = new JackpotPick(_contextInstance.Object, _jackpotService);
            
           
        }

        [Theory]//1000, 2500, 80000, 1000000
        [InlineData("322004", 81000, "02")]//02 - 81000
        [InlineData("3201014", 3500, "01")]//01 - 3500
        [InlineData("431021", 2500, "1")]//1 - 2500
        [InlineData("332120104", 1083500, "0123")]//0123 - 1083500
        [InlineData("33221004", 1081000, "023")]//023 - 1081000
        [InlineData("430213", 1000000, "3")]//3 - 1000000
        [InlineData("421300", 1000, "0")]//0 - 1000
        [InlineData("32120014", 83500, "012")]//012 - 83500
        [InlineData("33212014", 1082500, "123")]//123 - 1082500
        public void Pick_TestWholeJackpotScenario(string combination, int expectedTotalWin, string outcome)
        {
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq(prefix + combination));
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            _contextInstance.Setup(x => x.RequestItems).Returns(new RequestItems
            {
                denom = 1,
                isFreeSpin = false,
                betAmount = 88
            });
            _contextInstance.Object.RequestItems.denom = 1;

            var charNumbes = combination.ToCharArray();
            _contextInstance.Object.State.JackpotGame.leftItems = charNumbes.Length;
            _contextInstance.Object.State.JackpotGame.outcome = outcome;
            _contextInstance.Object.State.JackpotGame.selectedItems = new List<JackpotItem>();
            for (int i = 0; i < charNumbes.Length; i++)
            {
                _contextInstance.Object.State.JackpotGame.selectedItems.Add(new JackpotItem
                {
                    Symbol = Convert.ToString(charNumbes[i])
                });
            }
            dynamic ret = null;
            for (int i = 0; i < charNumbes.Length; i++)
            {
                var dynamicRequest = Json.ObjectToDynamic(req);
                ret = _jackpotPickInstance.PickJackpot(dynamicRequest);

            }
            var actualTotalWin = int.Parse((ret.transactions.credits[0].ToString()));
            Assert.Equal(actualTotalWin, expectedTotalWin);
        }

        [Theory]//1000, 2500, 80000, 1000000
        [InlineData("322004", 0)]//02 - 81000
        [InlineData("3201014", 0)]//01 - 3500
        [InlineData("431021", 0)]//1 - 2500
        [InlineData("332120104", 0)]//0123 - 1083500
        [InlineData("33221004", 0)]//023 - 1081000
        [InlineData("430213", 0)]//3 - 1000000
        [InlineData("421300", 0)]//0 - 1000
        [InlineData("32120014", 0)]//012 - 83500
        [InlineData("33212014", 0)]//123 - 1082500
        public void Pick_TestFirstPick(string combination, int expectedTotalWin)
        {
            //3220104
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq(prefix + combination));
            _contextInstance.Setup(x => x.RequestItems).Returns(new RequestItems());
            _contextInstance.Object.RequestItems.denom = 1;
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            var charNumbes = combination.ToCharArray();
            _contextInstance.Object.State.JackpotGame.leftItems = charNumbes.Length;
            for (int i = 0; i < charNumbes.Length; i++)
            {
                _contextInstance.Object.State.JackpotGame.selectedItems.Add(new JackpotItem
                {
                    Symbol = Convert.ToString(charNumbes[i])
                });
            }
            dynamic ret = null;
            var dynamicRequest = Json.ObjectToDynamic(req);
            ret = _jackpotPickInstance.PickJackpot(dynamicRequest);
            var spin = ((List<Spin>) ret.publicState.spin.childFeature.ToObject < List<Spin>>()).Last();
            Assert.Equal(charNumbes[0].ToString(), spin.value);
            Assert.Equal(charNumbes.Length-1, _contextInstance.Object.State.JackpotGame.leftItems);
            var actualTotalWin = 0;
            Assert.Equal(actualTotalWin, expectedTotalWin);
        }

        [Theory]
        [InlineData("Null")]
        public void Pick_JackpotGameNull(string jsonFile)
        {
            //3220104
            var req = JsonConvert.DeserializeObject<PlatformRequest>(JsonsClass.GetReq(prefix + jsonFile));
            _contextInstance.Setup(x => x.RequestItems).Returns(new RequestItems());
            _contextInstance.Object.RequestItems.denom = 1;
            _contextInstance.Setup(a => a.State).Returns(req.PrivateState);
            dynamic ret = null;
            var dynamicRequest = Json.ObjectToDynamic(req);
            ret = _jackpotPickInstance.PickJackpot(dynamicRequest);
            string retInString = ret.ToString();
            Assert.True(retInString.Contains("Error in validating jackpot game"));
        }
    }
}
