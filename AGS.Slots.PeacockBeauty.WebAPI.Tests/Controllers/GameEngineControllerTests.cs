using AGS.Slots.PeacockBeauty.Common.Interfaces;
using AGS.Slots.PeacockBeauty.Platform.Entities;
using AGS.Slots.PeacockBeauty.WebAPI.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace AGS.Slots.PeacockBeauty.WebAPI.Tests.Controllers
{
    public class GameEngineControllerTests
    {

        private GameEnginerController _instance;
        private Mock<ILogger<RequestManager>> _loggerMock;
        private Mock<ILogger<GameEnginerController>> _logger2Mock;
        private RequestManager _rmanager;
        private Mock<ISlotGame> _slotGame;

        public GameEngineControllerTests()
        {
            _loggerMock = new Mock<ILogger<RequestManager>>();
            _logger2Mock = new Mock<ILogger<GameEnginerController>>();
            _slotGame = new Mock<ISlotGame>();
            _rmanager = new RequestManager(_slotGame.Object, _loggerMock.Object);
            _instance = new GameEnginerController(_rmanager, _logger2Mock.Object);
        }

        [Fact]
        public async void  GameEngine_CheckAllObjectsSerializeCorrectly()
        {
           

            var req = JsonConvert.DeserializeObject<PlatformRequest>(GetReq("init1"));

            var rr = System.Text.Json.JsonSerializer.Deserialize<PlatformRequest>(GetReq("init1"));
            var res = await _instance.Post(req);
            Assert.NotNull(res);
            
           

        }


        private  string GetReq(string req)
        {
            var json =  File.ReadAllText($"Requests/{req}.json");

            return json;
        }
    }
}
