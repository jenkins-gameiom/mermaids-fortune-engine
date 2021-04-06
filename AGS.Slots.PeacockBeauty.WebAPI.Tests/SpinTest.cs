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
        [InlineData("3bnandnowin")]
        [InlineData("4bnandnowin")]
        [InlineData("5bnandnowin")]
        public async Task Test_Force_bn_regular(string forceName)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            try
            {
                var response = await PostRequest(client, forceName);
                var res = response.ToString();
                PlatformResponse resultObject = JsonConvert.DeserializeObject<PlatformResponse>(res);
                var zz = resultObject.PublicState.spin.reels.SelectMany(x => x).ToList().Count(x => x == 12);
                if (zz.ToString() == forceName[0].ToString())
                {

                }
                //if (resultObject.PublicState.spin.reels.ToList())
            }
            catch (Exception e)
            {
                Assert.False(true);
            }
            // Assert
        }

        [Fact]
        public async Task Test_Force_3bnandwin()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            try
            {
                var response = await PostRequest(client, "3bnandwin");
                var res = response.ToString();
                PlatformResponse resultObject = JsonConvert.DeserializeObject<PlatformResponse>(res);
                var zz = resultObject.PublicState.spin.reels.SelectMany(x => x).ToList().Count(x => x == 12);

            }
            catch (Exception e)
            {
                Assert.False(true);
            }
            // Assert
        }


        [Theory]
        [InlineData("3bnretrigger")]
        [InlineData("4bnretrigger")]
        [InlineData("5bnretrigger")]
        public async Task Test_Force_bn_retrigger(string forceName)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            try
            {
                var response = await PostRequest(client, forceName, true);
                var res = response.ToString();
                PlatformResponse resultObject = JsonConvert.DeserializeObject<PlatformResponse>(res);
                var zz = resultObject.PublicState.spin.childFeature.First().reels.ToList().SelectMany(x => x).ToList().Count(x => x == 12);
                Assert.True(zz.ToString() == forceName[0].ToString() && resultObject.PublicState.action == "freespin");

            }
            catch (Exception e)
            {
                Assert.False(true);
            }
            // Assert
        }

        [Fact]
        public async Task Test_Force_holdandwininbase6symbols()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            try
            {
                var response = await PostRequest(client, "holdandwininbase6symbols", false);
                var res = response.ToString();
                PlatformResponse resultObject = JsonConvert.DeserializeObject<PlatformResponse>(res);
                Assert.True(resultObject.PublicState.spin.MCSymbols.Count == 6);
            }
            catch (Exception e)
            {
                Assert.False(true);
            }
            // Assert
        }

        private async Task<dynamic> PostRequest(HttpClient client, [CallerMemberName] string callerMethodName = null, bool isFreeSpin = false)
        {
            //var json = await File.ReadAllTextAsync(System.IO.Directory.GetCurrentDirectory() + "\\Engine\\MermaidsFortune\\Forces\\" + callerMethodName + ".json");
            string json = null;
            if (isFreeSpin)
            {
                json = await File.ReadAllTextAsync($"Requests/freespin.json");
            }
            else
            {
                json = await File.ReadAllTextAsync($"Requests/spin.json");
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
