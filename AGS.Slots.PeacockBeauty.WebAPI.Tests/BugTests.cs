using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AGS.Slots.MermaidsFortune.WebAPI.Tests
{
    public class BugTests : IClassFixture<WebApplicationFactory<Startup>>
    {

        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ILogger<RequestManager> _logger;

        public BugTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _logger = new Logger<RequestManager>(new LoggerFactory());
        }


        [Fact(DisplayName = "Deal -> Object reference not set to an instance of an object")]
        public async Task InitRtpValidation()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            try
            {
                var response = await PostRequest(client);

            }
            catch (Exception e)
            {
                Assert.True(e.Message.Contains("Rtp shouldn't be empty"));
            }
            // Assert
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
