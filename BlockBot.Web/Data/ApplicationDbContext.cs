using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

namespace BlockBot.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private readonly IConfiguration configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                optionsBuilder
                       .UseLazyLoadingProxies()
                       .UseSqlite(configuration.GetConnectionString("MacConnection"));
            }
            else
            {
                optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<ProjectStep> ProjectSteps { get; set; }
    }
}
