using System.Threading.Tasks;
using IdeaWeb.Controllers;
using IdeaWeb.Data;
using IdeaWeb.Models;
using IdeaWeb.Test.Data;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace IdeaWeb.Test.Controllers
{
    [TestFixture]
    public class IdeaControllerTests
    {
        const int NUM_IDEAS = 10;

        IdeasController _controller;

        [SetUp]
        public void SetUp()
        {
            IdeaContext context = TestIdeaContextFactory.Create(TestContext.CurrentContext, NUM_IDEAS);
            _controller = new IdeasController(context);
        }

        [TestCase(null)]
        [TestCase(100)]
        public async Task DetailsWithNullOrMissingIdReturnsNotFound(int? id)
        {
            IActionResult result = await _controller.Details(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task DetailsReturnsIdea()
        {
            ViewResult result = await _controller.Details(1) as ViewResult;

            Assert.That(result?.Model, Is.Not.Null);

            Idea idea = result.Model as Idea;
            Assert.That(idea, Is.Not.Null);
            Assert.That(idea.Name, Is.EqualTo("Idea name 1"));
            Assert.That(idea.Description, Does.Contain("1"));
            Assert.That(idea.Rating, Is.EqualTo(2));
        }
    }
}
