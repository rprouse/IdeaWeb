using IdeaWeb.Data;
using IdeaWeb.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace IdeaWeb.Test.Data
{
    public static class TestIdeaContextFactory
    {
        /// <summary>
        /// Creates an <see cref="IdeaContext"/> using an in-memory
        /// database and populates it with test data.
        /// </summary>
        /// <param name="numIdeas">The number of ideas you want to be created in the database</param>
        /// <returns>A test context backed by an in-memory database</returns>
        public static IdeaContext Create(int numIdeas)
        {
            // ================================================================
            // Use the in-memory database to make the tests lightning fast.
            // Create unique database names based on the test id
            // ================================================================
            var options = new DbContextOptionsBuilder<IdeaContext>()
                .UseInMemoryDatabase(TestContext.CurrentContext.Test.ID)
                .Options;

            // ================================================================
            // Seed the in-memory database with known data
            // ================================================================
            using (var context = new IdeaContext(options))
            {
                for (int i = 1; i <= numIdeas; i++)
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

            // ================================================================
            // Use a clean copy of the context within the tests
            // ================================================================
            return new IdeaContext(options);
        }
    }
}
