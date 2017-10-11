using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdeaWeb.Controllers;
using IdeaWeb.Data;
using IdeaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace IdeaWeb.Test
{
    [TestFixture]
    public class HomeControllerTests
    {
        const int NUM_IDEAS = 10;

        HomeController _controller;

        [SetUp]
        public void SetUp()
        {
            // Create unique database names based on the test id
            var options = new DbContextOptionsBuilder<IdeaContext>()
                .UseInMemoryDatabase(TestContext.CurrentContext.Test.ID)
                .Options;

            // Seed the in-memory database
            using (var context = new IdeaContext(options))
            {
                for(int i = 1; i <= NUM_IDEAS; i++)
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

            // Use a clean copy of the context within the tests
            _controller = new HomeController(new IdeaContext(options));
        }

        [Test]
        public async Task IndexReturnsListOfIdeas()
        {
            ViewResult result = await _controller.Index() as ViewResult;

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
