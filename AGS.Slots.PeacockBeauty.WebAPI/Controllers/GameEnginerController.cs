using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGS.Slots.MermaidsFortune.Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AGS.Slots.MermaidsFortune.WebAPI.Controllers
{
    [ApiController]
    [Route("")]
    public class GameEnginerController : ControllerBase
    {


        private readonly ILogger<GameEnginerController> _logger;
        private readonly RequestManager _requestManager;
        
        public GameEnginerController(RequestManager requestManager, ILogger<GameEnginerController> logger)
        {
            _logger = logger;
            _requestManager = requestManager;
        }


        [HttpPost]
        [Route("request")]
        public string Post(PlatformRequest content)
        {

            if (content == null)
            {
                _logger.LogWarning("Request could not be deserialized. Request body: {RequestBody}", GetRequestBodyAsString());
                throw new ArgumentNullException(nameof(content), "Request is null. Check the format of the request body.");
            }

            return _requestManager.HandleRequest(content);
        }
        [HttpGet]
        [Route("hello")]
        public string Hello()
        {
            return "hello from game engine";
        }


        private string GetRequestBodyAsString()
        {
            var body = new StreamReader(Request.Body);
            body.BaseStream.Seek(0, SeekOrigin.Begin);
            var requestBody = body.ReadToEnd();
            return requestBody;
        }
    }
}
