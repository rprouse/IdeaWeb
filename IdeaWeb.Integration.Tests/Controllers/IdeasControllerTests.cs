using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdeaWeb.Integration.Tests.Extensions;
using NUnit.Framework;

namespace IdeaWeb.Integration.Tests.Controllers
{
    [TestFixture]
    public class IdeasControllerTests
    {
        HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _client = HttpClientFactory.Create();
        }

        [Test]
        public async Task CanCreateIdea()
        {
            // =======================================================================
            // First fetch the create form to get the anti-forgery token and
            // matching cookies
            // =======================================================================
            var formResponse = await _client.GetAsync("Ideas/Create");
            formResponse.EnsureSuccessStatusCode();

            string antiforgeryToken = await formResponse.ExtractAntiForgeryToken();

            // =======================================================================
            // Create the form data to POST back to the server
            // =======================================================================
            var request = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "__RequestVerificationToken", antiforgeryToken },
                { "Name", "New Idea" },
                { "Description", "This is a new idea" },
                { "Rating", "1" }
            });
            request.CopyCookiesFromResponse(formResponse);

            var response = await _client.PostAsync("Ideas/Create", request);

            // =======================================================================
            // If the idea was saved successfully, the page redirects to home
            // =======================================================================
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            Assert.That(response.Headers.Location?.ToString(), Is.Not.Null.And.EqualTo("/"));
        }

        [Test]
        public async Task CreateRequireseAntiForgeryToken()
        {
            var request = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "Name", "New Idea" },
                { "Description", "This is a new idea" },
                { "Rating", "1" }
            });

            var response = await _client.PostAsync("Ideas/Create", request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
