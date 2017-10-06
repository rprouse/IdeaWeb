using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdeaWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace IdeaWeb.Data
{
    public class IdeaContext : DbContext
    {
        public IdeaContext(DbContextOptions<IdeaContext> options) : base(options)
        {
        }

        public DbSet<Idea> Ideas { get; set; }
    }
}
