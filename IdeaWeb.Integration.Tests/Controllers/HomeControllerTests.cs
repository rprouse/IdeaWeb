using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace IdeaWeb.Integration.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        TestServer _server;
        HttpClient _client;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());

            _client = _server.CreateClient();
        }

        [Test]
        public async Task HomePageContainsIdeas()
        {
            var response = await _client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response, Does.Contain("<span class=\"name\">"));
        }
    }
}
