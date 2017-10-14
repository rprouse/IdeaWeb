using IdeaWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdeaWeb.Integration.Tests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<IdeaContext>(options => options.UseInMemoryDatabase("Integration"));
        }
    }
}
