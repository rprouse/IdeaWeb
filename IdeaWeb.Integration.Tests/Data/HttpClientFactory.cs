using System.IO;
using System.Net.Http;
using IdeaWeb.Data;
using IdeaWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

namespace IdeaWeb.Integration.Tests.Data
{
    /// <summary>
    /// Used to create an <see cref="HttpClient"/> for testing and
    /// seed the database with known values
    /// </summary>
    public static class HttpClientFactory
    {
        public static HttpClient Create()
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
            SeedDatabase(server);

            return server.CreateClient();
        }

        static void SeedDatabase(TestServer server)
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
        static string CalculateRelativeContentRootPath() =>
            Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
            @"..\..\..\..\IdeaWeb");
    }
}
