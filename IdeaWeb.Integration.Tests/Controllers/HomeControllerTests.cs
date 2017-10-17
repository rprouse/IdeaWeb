using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using IdeaWeb.Data;
using IdeaWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using NUnit.Framework;

namespace IdeaWeb.Integration.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        HttpClient _client;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // ==================================================================
            // The configuration sets the content root so the Razor views will
            // be found, sets the environment and specifies a TestStartup to use
            // ==================================================================
            var webHostBuilder = new WebHostBuilder()
                .UseContentRoot(CalculateRelativeContentRootPath())
                .UseEnvironment("Development")
                .UseStartup<TestStartup>();

            // ================================================================
            // ================================================================
            var server = new TestServer(webHostBuilder);

            _client = server.CreateClient();

            SeedDatabase(server);
        }

        private static void SeedDatabase(TestServer server)
        {
            var context = server.Host.Services.GetService<IdeaContext>();

            for (int i = 1; i <= 10; i++)
            {
                context.Add(new Idea
                {
                    Name = $"Idea name {i}",
                    Description = $"Description {i}",
                    Rating = i % 3 + 1
                });
            }
            context.SaveChanges();
        }

        // =======================================================================
        // With MVC, we need to set the content root to the project being tested
        // so that the Razor views can be found
        // =======================================================================
        string CalculateRelativeContentRootPath() =>
            Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
            @"..\..\..\..\IdeaWeb");

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
