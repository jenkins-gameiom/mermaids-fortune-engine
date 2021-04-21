using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;

namespace AGS.Slots.MermaidsFortune.WebAPI.Tests
{
    public class SpinTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ILogger<RequestManager> _logger;

        public SpinTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _logger = new Logger<RequestManager>(new LoggerFactory());
        }

        [Theory]
        [InlineData("freespins")]
        public async Task Test_Force_bn(string forceName)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await PostRequest(client, forceName, false);
            var res = response.ToString();
            PlatformResponse resultObject = JsonConvert.DeserializeObject<PlatformResponse>(res);
            Assert.True(resultObject.PublicState.spin.wins.Any(x => x.featureType == "freeSpinWin"));
        }

        [Theory]
        [InlineData("freespins")]
        public async Task Test_Force_retrigger(string forceName)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await PostRequest(client, forceName, true);
            var res = response.ToString();
            PlatformResponse resultObject = JsonConvert.DeserializeObject<PlatformResponse>(res);
            Assert.True(resultObject.PrivateState.freeSpinsLeft == 15);
        }

        [Theory]
        [InlineData("bigwin")]
        public async Task RequestManager_RtpValid(string force)
        {
            var client = _factory.CreateClient();
            var response = await PostRequest(client, force, false);
            Assert.True(true);
        }

        [Theory]
        [InlineData("bigwin")]
        public async Task RequestManager_RtpInvalid(string force)
        {
            try
            {
                var client = _factory.CreateClient();
                var response = await PostRequest(client, force, false, "spininvalidrtp");
            }
            catch (Exception e)
            {
                Assert.True(e.ToString().Contains("is not valid"));
            }
        }

        [Theory]
        [InlineData("bigwin")]
        public async Task RequestManager_RtpEmpty(string force)
        {
            try
            {
                var client = _factory.CreateClient();
                var response = await PostRequest(client, force, false, "spinemptyrtp");
            }
            catch (Exception e)
            {
                Assert.True(e.ToString().Contains("empty"));
            }
        }

        private async Task<dynamic> PostRequest(HttpClient client, [CallerMemberName] string callerMethodName = null, bool isFreeSpin = false, string fileName = null)
        {
            //var json = await File.ReadAllTextAsync(System.IO.Directory.GetCurrentDirectory() + "\\Engine\\MermaidsFortune\\Forces\\" + callerMethodName + ".json");
            string json = null;
            if (fileName == null)
            {
                if (isFreeSpin)
                {
                    json = await File.ReadAllTextAsync($"Requests/freespin.json");
                }
                else
                {
                    json = await File.ReadAllTextAsync($"Requests/spin.json");
                }
            }
            else
            {
                json = await File.ReadAllTextAsync($"Requests/" + fileName + ".json");
            }


            var resultObject = JsonConvert.DeserializeObject<PlatformRequest>(json);
            resultObject.PublicState.force = callerMethodName;
            json = JsonConvert.SerializeObject(resultObject);
            var request = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/request", request);
            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseJson);

            if (result.error != null)
            {
                throw new Exception("Error received: " + result.error.message.Value);
            }

            return result;
        }
    }
}
