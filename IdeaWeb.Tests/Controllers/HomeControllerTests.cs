using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdeaWeb.Controllers;
using IdeaWeb.Data;
using IdeaWeb.Models;
using IdeaWeb.Test.Data;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace IdeaWeb.Test
{
    [TestFixture]
    public class HomeControllerTests
    {
        const int NUM_IDEAS = 10;

        HomeController _controller;

        // ======================================================================================
        // SetUp happens once before every test and is useful for setting up data all your
        // tests use
        // ======================================================================================
        [SetUp]
        public void SetUp()
        {
            // Arrange
            IdeaContext context = TestIdeaContextFactory.Create(NUM_IDEAS);
            _controller = new HomeController(context);
        }

        // ======================================================================================
        // The Test attribute indicates a unit test. We are using async code, so the method
        // must be async Task
        // ======================================================================================
        [Test]
        public async Task IndexReturnsListOfIdeas()
        {
            // Act
            ViewResult result = await _controller.Index() as ViewResult;

            // Assert
            Assert.That(result?.Model, Is.Not.Null);
            Assert.That(result.Model, Has.Count.EqualTo(NUM_IDEAS));

            IEnumerable<Idea> ideas = result.Model as IEnumerable<Idea>;
            Idea idea = ideas?.FirstOrDefault();
            Assert.That(idea, Is.Not.Null);
            Assert.That(idea.Name, Is.EqualTo("Idea name 1"));
            Assert.That(idea.Description, Does.Contain("1"));
            Assert.That(idea.Rating, Is.EqualTo(2));
        }
    }
}
