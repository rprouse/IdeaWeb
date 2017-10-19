using System.Collections.Generic;
using System.Linq;
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
    public class IdeasControllerTests
    {
        const int NUM_IDEAS = 10;

        IdeasController _controller;
        IdeaContext _context;

        // ======================================================================================
        // An IEqualityComparer<T> allows you to test equality of items in collections
        // ======================================================================================
        class IdeaEqualityComparer : IEqualityComparer<Idea>
        {
            public bool Equals(Idea x, Idea y) =>
                x?.Name == y?.Name &&
                x?.Description == y?.Description &&
                x?.Rating == y?.Rating;

            public int GetHashCode(Idea obj) =>
                obj?.GetHashCode() ?? 0;
        }

        // ======================================================================================
        // SetUp happens once before every test and is useful for setting up data all your
        // tests use
        // ======================================================================================
        [SetUp]
        public void SetUp()
        {
            _context = TestIdeaContextFactory.Create(NUM_IDEAS);
            _controller = new IdeasController(_context);
        }

        // ======================================================================================
        // The Test attribute indicates a unit test. We are using async code, so the method
        // must be async Task
        // ======================================================================================
        [Test]
        public async Task DetailsReturnsIdea()
        {
            // =========
            //  ARRANGE
            // =========
            var expected = await _context.Ideas.FirstOrDefaultAsync();
            Assume.That(expected, Is.Not.Null);

            // =========
            //    ACT
            // =========
            ViewResult result = await _controller.Details(expected.Id) as ViewResult;

            // =========
            //  ASSERT
            // =========
            AssertViewResultContainsExpectedId(expected, result);
        }

        // ======================================================================================
        // Data driven tests allow you to test easily with multiple values. See also the
        // TestCaseSource attribute for more complex data driven tests.
        // ======================================================================================
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
        public async Task CreateWithInvalidModelDoesNotPersist()
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
        public async Task CreateWithInvalidModelUnpersistedModel()
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


        [TestCase(null)]
        [TestCase(-1)]
        [TestCase(10000000)]
        public async Task EditGetWithNullOrMissingIdReturnsNotFound(int? id)
        {
            IActionResult result = await _controller.Edit(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task EditGetReturnsIdea()
        {
            // Get a valid idea out of the db
            var expected = await _context.Ideas.FirstOrDefaultAsync();
            Assume.That(expected, Is.Not.Null);

            ViewResult result = await _controller.Edit(expected.Id) as ViewResult;

            AssertViewResultContainsExpectedId(expected, result);
        }


        [TestCase(-1)]
        [TestCase(10000000)]
        public async Task EditPostWithNullOrMissingIdReturnsNotFound(int id)
        {
            var idea = new Idea
            {
                Id = id,
                Name = "Invalid"
            };
            IActionResult result = await _controller.Edit(id, idea);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task EditPersistsChanges()
        {
            // Get a valid idea out of the db and update it
            var idea = await _context.Ideas.FirstOrDefaultAsync();
            Assume.That(idea, Is.Not.Null);
            idea.Description += "Updated";

            await _controller.Edit(idea.Id, idea);

            // Fetch the idea back out of the database
            var updated = await _context.Ideas.SingleOrDefaultAsync(m => m.Id == idea.Id);
            Assert.That(updated, Is.Not.Null);
            Assert.That(updated.Description, Is.EqualTo(idea.Description));
        }

        [Test]
        public async Task EditRedirectsToDetails()
        {
            // Get a valid idea out of the db and update it
            var idea = await _context.Ideas.FirstOrDefaultAsync();
            Assume.That(idea, Is.Not.Null);
            idea.Description = "Updated";

            RedirectToActionResult result = await _controller.Edit(idea.Id, idea) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues["Id"], Is.EqualTo(idea.Id));
        }

        [Test]
        public async Task EditWithInvalidModelDoesNotSave()
        {
            // Get a valid idea out of the db and update it
            var idea = await _context.Ideas.FirstOrDefaultAsync();
            Assume.That(idea, Is.Not.Null);

            // In-memory database, work on a copy
            var copy = new Idea
            {
                Id = idea.Id,
                Description = idea.Description + "Updated"
            };

            _controller.ModelState.AddModelError("TestError", "TestError");
            await _controller.Edit(copy.Id, copy);

            // Fetch the idea back out of the database
            var updated = await _context.Ideas.SingleOrDefaultAsync(m => m.Id == idea.Id);
            Assert.That(updated, Is.Not.Null);
            Assert.That(updated.Description, Is.Not.EqualTo(copy.Description));
        }

        [Test]
        public async Task EditWithDifferentIdReturnsNotFound()
        {
            // Get a valid idea out of the db and update it
            var idea = await _context.Ideas.FirstOrDefaultAsync();
            Assume.That(idea, Is.Not.Null);

            IActionResult result = await _controller.Edit(10000, idea);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task EditWithInvalidIdReturnsNotFound()
        {
            // Get a valid idea out of the db and update it
            var idea = new Idea
            {
                Id = 10000,
                Name = "Invalid",
                Description = "Doesn't exist"
            };

            IActionResult result = await _controller.Edit(idea.Id, idea);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }


        [TestCase(null)]
        [TestCase(-1)]
        [TestCase(10000000)]
        public async Task DeleteWithNullOrMissingIdReturnsNotFound(int? id)
        {
            IActionResult result = await _controller.Delete(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteReturnsIdea()
        {
            // Get a valid idea out of the db
            var expected = await _context.Ideas.FirstOrDefaultAsync();
            Assume.That(expected, Is.Not.Null);

            ViewResult result = await _controller.Delete(expected.Id) as ViewResult;

            AssertViewResultContainsExpectedId(expected, result);
        }

        [Test]
        public async Task DeleteConfirmedDeletesIdea()
        {
            // Get a valid idea out of the db
            var idea = await _context.Ideas.FirstOrDefaultAsync();
            Assume.That(idea, Is.Not.Null);

            await _controller.DeleteConfirmed(idea.Id);

            // Fetch the idea back out of the database
            var result = await _context.Ideas.SingleOrDefaultAsync(i => i.Id == idea.Id);
            Assert.That(result, Is.Null);
        }

        private static void AssertViewResultContainsExpectedId(Idea expected, ViewResult result)
        {
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
    }
}
