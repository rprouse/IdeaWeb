using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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
        TestServer _server;
        HttpClient _client;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseContentRoot(CalculateRelativeContentRootPath())
                .UseEnvironment("Development")
                .UseStartup<TestStartup>());

            _client = _server.CreateClient();

            var context = _server.Host.Services.GetService<IdeaContext>();

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

        string CalculateRelativeContentRootPath() =>
            Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
            @"..\..\..\..\IdeaWeb");

        [Test]
        public async Task HomePageContainsIdeas()
        {
            var response = await _client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.That(content, Does.Contain("<span class=\"name\">"));
        }
    }
}
