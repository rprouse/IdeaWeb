using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using IdeaWeb.Integration.Tests.Data;
using NUnit.Framework;

namespace IdeaWeb.Integration.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _client = HttpClientFactory.Create();
        }

        [Test]
        public async Task HomePageContainsIdeas()
        {
            // =======================================================================
            // Using the client created from the TestHost, you can make calls that
            // use all of the routing and configuration without going through a
            // browser or the network
            // =======================================================================
            var response = await _client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            // =======================================================================
            // You can use the HtmlAgilityPack to parse the HTML for testing
            // =======================================================================
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var ideas = doc.DocumentNode
                           .Descendants("span")
                           .Where(s => s.Attributes["class"].Value == "name");

            Assert.That(ideas.Any(), Is.True);
        }
    }
}
