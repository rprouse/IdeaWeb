using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdeaWeb.Models;

namespace IdeaWeb.Data
{
    public static class DbInitializer
    {
        public static void Seed(IdeaContext context)
        {
            context.Database.EnsureCreated();

            if(context.Ideas.Any())
            {
                return;
            }

            context.Add(new Idea {
                Name = "Write a Unit Test Framework",
                Description = "There aren't enough .NET test frameworks, we need yet another one!",
                Rating = 1
            });

            context.Add(new Idea
            {
                Name = "Prepare a Presentation on TDD",
                Description = "Need to convince .NET Developers that unit testing can make their lives easier.",
                Rating = 3
            });

            context.Add(new Idea
            {
                Name = "Write an idea app",
                Description = "Wouldn't it be cool if I had an app to jot down all my cool ideas in!",
                Rating = 2
            });

            context.SaveChanges();
        }
    }
}
