using System.Collections.Generic;
using System.Threading.Tasks;
using IdeaWeb.Controllers;
using IdeaWeb.Data;
using IdeaWeb.Models;
using IdeaWeb.Test.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace IdeaWeb.Test.Controllers
{
    [TestFixture]
    public class IdeaControllerTests
    {
        const int NUM_IDEAS = 10;

        IdeasController _controller;
        IdeaContext _context;

        class IdeaEqualityComparer : IEqualityComparer<Idea>
        {
            public bool Equals(Idea x, Idea y) =>
                x?.Name == y?.Name &&
                x?.Description == y?.Description &&
                x?.Rating == y?.Rating;

            public int GetHashCode(Idea obj) =>
                obj?.GetHashCode() ?? 0;
        }

        [SetUp]
        public void SetUp()
        {
            _context = TestIdeaContextFactory.Create(TestContext.CurrentContext, NUM_IDEAS);
            _controller = new IdeasController(_context);
        }

        [TestCase(null)]
        [TestCase(-1)]
        [TestCase(10000000)]
        public async Task DetailsWithNullOrMissingIdReturnsNotFound(int? id)
        {
            IActionResult result = await _controller.Details(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task DetailsReturnsIdea()
        {
            // Get a valid idea out of the db
            var expected = await _context.Ideas.FirstOrDefaultAsync();
            Assume.That(expected, Is.Not.Null);

            ViewResult result = await _controller.Details(expected.Id) as ViewResult;

            Assert.That(result?.Model, Is.Not.Null);

            Idea idea = result.Model as Idea;
            Assert.That(idea, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(idea.Name, Is.EqualTo(expected.Name));
                Assert.That(idea.Description, Is.EqualTo(expected.Description));
                Assert.That(idea.Rating, Is.EqualTo(expected.Rating));
            });
        }

        [Test]
        public async Task CreatePersistsNewIdea()
        {
            var idea = new Idea
            {
                Name = "New idea",
                Description = "Persist an idea",
                Rating = 1
            };
            await _controller.Create(idea);

            // Fetch the ideas back out of the database
            var ideas = await _context.Ideas.ToListAsync();
            Assert.That(ideas, Has.Count.EqualTo(NUM_IDEAS + 1));
            Assert.That(ideas, Has.One.EqualTo(idea).Using(new IdeaEqualityComparer()));
        }

        [Test]
        public async Task CreateRedirectsToHome()
        {
            var idea = new Idea
            {
                Name = "New idea",
                Description = "Persist an idea",
                Rating = 1
            };
            RedirectToActionResult result = await _controller.Create(idea) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
        }

        [Test]
        public async Task CreateMissingNameDoesNotPersist()
        {
            var idea = new Idea
            {
                Description = "Persist an idea",
                Rating = 1
            };
            _controller.ModelState.AddModelError("TestError", "TestError");
            await _controller.Create(idea);

            // Ensure the idea is not in the database
            var ideas = await _context.Ideas.ToListAsync();
            Assert.That(ideas, Has.Count.EqualTo(NUM_IDEAS));
            Assert.That(ideas, Has.None.EqualTo(idea).Using(new IdeaEqualityComparer()));
        }

        [Test]
        public async Task CreateMissingNameReturnsUnpersistedModel()
        {
            var idea = new Idea
            {
                Description = "Persist an idea",
                Rating = 1
            };
            _controller.ModelState.AddModelError("TestError", "TestError");
            var result = await _controller.Create(idea) as ViewResult;

            Assert.That(result?.Model, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(idea).Using(new IdeaEqualityComparer()));
        }
    }
}
