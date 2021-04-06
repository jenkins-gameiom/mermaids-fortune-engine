using System;
using System.Collections.Generic;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Helpers;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine;
using AGS.Slots.MermaidsFortune.Platform;
using AGS.Slots.MermaidsFortune.WebAPI.Controllers;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AGS.Slots.MermaidsFortune.WebAPI.Tests
{
    public class UnitTest1
    {
        private readonly ILogger<RequestManager> _logger;
        private readonly RequestManager _requestManager;
        private readonly Game _game;
        private readonly Mock<IRequestContext> _context;
        private readonly Mock<IMathFileService> _mathFileService;
        public UnitTest1()
        {
            _context = new Mock<IRequestContext>();
            _context.SetupSet(x => x.State = new SpinPrivateState()).Verifiable();
            _context.SetupSet(x => x.RequestItems = new RequestItems()).Verifiable();
            _context.SetupSet(x => x.Config = new Config()).Verifiable();
            
            _mathFileService = new Mock<IMathFileService>();
            _mathFileService.Setup(x => x.GetMathFile(It.IsAny<MathFileType>())).Returns(new Logic.Engine.MermaidsFortune.Config());
            _logger = new Logger<RequestManager>(new LoggerFactory());
            _requestManager = null;
            //new RequestManager(_mathFileService.Object, _game, _logger, _context.Object);
        }

        [Theory]
        [InlineData(96)]
        [InlineData(94)]
        public void RequestManager_RtpValid(double rtp)
        {
            _context.SetupGet(x => x.Config).Returns(new Config { rtp = rtp });
            
            PlatformRequest platformRequest = new PlatformRequest();
            platformRequest.PrivateState = new SpinPrivateState();
            platformRequest.PublicState = new SpinPublicStateRequest();
            platformRequest.Config = new Config();
            var x = _requestManager.HandleRequest(platformRequest);
            //var ex = Assert.Throws<Exception>(() => _requestManager.HandleRequest(platformRequest));
            Assert.False(x.ToLower().Contains("error"));
        }

        [Theory]
        [InlineData(96.12)]
        [InlineData(96.08)]
        [InlineData(94.55)]
        [InlineData(72)]
        public void RequestManager_RtpInvalid(double rtp)
        {
            _context.SetupGet(x => x.Config).Returns(new Config { rtp = rtp });
            PlatformRequest platformRequest = new PlatformRequest();
            platformRequest.PrivateState = new SpinPrivateState();
            platformRequest.PublicState = new SpinPublicStateRequest();
            platformRequest.Config = new Config();
            var x = _requestManager.HandleRequest(platformRequest);
            //var ex = Assert.Throws<Exception>(() => _requestManager.HandleRequest(platformRequest));
            Assert.True(x.ToLower().Contains("rtp " + rtp + " is not valid"));
        }

        [Fact]
        public void RequestManager_RtpEmpty()
        {
            _context.SetupGet(x => x.Config).Returns(new Config { rtp = 0 });
            PlatformRequest platformRequest = new PlatformRequest();
            platformRequest.PrivateState = new SpinPrivateState();
            platformRequest.PublicState = new SpinPublicStateRequest();
            platformRequest.Config = new Config();
            var x = _requestManager.HandleRequest(platformRequest);
            //var ex = Assert.Throws<Exception>(() => _requestManager.HandleRequest(platformRequest));
            Assert.True(x.ToLower().Contains("rtp"));
            Assert.True(x.ToLower().Contains("error"));
            Assert.True(x.ToLower().Contains("empty"));
        }
    }
}
