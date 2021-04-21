using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AGS.Slots.MermaidsFortune.Common.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;

namespace AGS.Slots.MermaidsFortune.WebAPI.Tests
{
    public class InitTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ILogger<RequestManager> _logger;

        public InitTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _logger = new Logger<RequestManager>(new LoggerFactory());
        }

        [Fact]
        public async Task RequestManager_Valid()
        {
            var client = _factory.CreateClient();
            var response = await PostRequest(client, "initok");
            Assert.True(true);
        }

        [Fact]
        public async Task RequestManager_RtpInvalid()
        {
            try
            {
                var client = _factory.CreateClient();
                var response = await PostRequest(client, "initinvalidrtp");
            }
            catch (Exception e)
            {
                Assert.True(e.ToString().Contains("is not valid"));
            }
        }

        [Fact]
        public async Task RequestManager_RtpEmpty()
        {
            try
            {
                var client = _factory.CreateClient();
                var response = await PostRequest(client, "initemptyrtp");
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


        private async Task<dynamic> PostRequest(HttpClient client, [CallerMemberName] string callerMethodName = null)
        {
            var json = await File.ReadAllTextAsync($"Requests/{callerMethodName}.json");

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
