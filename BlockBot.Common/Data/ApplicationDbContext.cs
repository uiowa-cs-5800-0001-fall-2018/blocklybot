using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BlockBot.Common.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, ApplicationUserClaim,
        ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
    {
        private readonly IConfiguration configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            this.configuration = configuration;
        }

        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public virtual DbSet<ApplicationUserClaim> ApplicationUserClaims { get; set; }

        public virtual DbSet<Deployment> Deployments { get; set; }

        public virtual DbSet<Integration> Integrations { get; set; }

        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<ProjectSetting> ProjectSettings { get; set; }

        public virtual DbSet<ProjectSettingType> ProjectSettingType { get; set; }

        public virtual DbSet<Service> Services { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseSqlite(configuration.GetConnectionString("MacConnection"),
                        builder => builder.MigrationsAssembly("BlockBot.Web"));
            }
            else
            {
                optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        builder => builder.MigrationsAssembly("BlockBot.Web"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne(e => e.User)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne(e => e.User)
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<ApplicationRole>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                // Each Role can have many associated RoleClaims
                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();
            });

            // Project Setting Type seed values
            modelBuilder.Entity<ProjectSettingType>().HasData(
                //new ProjectSettingType
                //{
                //    Id = new Guid("2078335c-4dea-48e6-8248-1c9cee3f1a1b"),
                //    Name = "AwsAccessKey",
                //    AllowsMany = false
                //},
                //new ProjectSettingType
                //{
                //    Id = new Guid("2e10e0a3-f121-4b3b-a80d-2bad25e58064"),
                //    Name = "AwsSecretKey",
                //    AllowsMany = false
                //},
                new ProjectSettingType
                {
                    Id = new Guid("9ff16deb-455c-45b3-940f-c5908ac2ddc2"),
                    Name = "TwilioAccountSid",
                    AllowsMany = false
                },
                new ProjectSettingType
                {
                    Id = new Guid("cf1732d4-96c5-4c84-8689-6059d84ec6c7"),
                    Name = "TwilioAuthToken",
                    AllowsMany = false
                });

            // Service seed values
            modelBuilder.Entity<Service>().HasData(
                new Service
                {
                    Id = new Guid("28ce5d09-b126-44f2-80dd-13ec30e8b89b"),
                    Name = "BlockBot"
                },
                new Service
                {
                    Id = new Guid("eed6a6b3-33d1-4e25-bd0c-1fc726bbf2de"),
                    Name = "Twilio"
                }
                );
        }



        public void StoreAsync<T>(string key, T value)
        {
            ApplicationUser user = this.ApplicationUsers.FirstOrDefault(x => x.NormalizedUserName == key);
            if (user != null)
            {
                ApplicationUserClaim claim = this.ApplicationUserClaims.FirstOrDefault(x =>
                        x.UserId == user.Id && x.ClaimType == "GoogleAccessToken");
                if (claim != null)
                {
                    claim.ClaimValue = JsonConvert.SerializeObject(value);
                }
                else
                {
                    this.ApplicationUserClaims.Add(new ApplicationUserClaim
                    {
                        UserId = user.Id,
                        ClaimType = "GoogleAccessToken",
                        ClaimValue = JsonConvert.SerializeObject(value)
                    });
                }

                this.SaveChanges();
            }
            else
            {
                throw new Exception($"Cannot store Google Access Token for user '{key}', no such user found.");
            }
        }

        public void DeleteAsync<T>(string key)
        {
            ApplicationUser user = this.ApplicationUsers.FirstOrDefault(x => x.NormalizedUserName == key);
            if (user != null)
            {
                ApplicationUserClaim claim = this.ApplicationUserClaims
                    .FirstOrDefault(x => x.UserId == user.Id
                                              && x.ClaimType == "GoogleAccessToken");
                if (claim != null)
                {
                    this.ApplicationUserClaims.Remove(claim);
                    this.SaveChanges();
                }
            }

            // TODO consider logging if there is nothing to delete
        }

        public T GetAsync<T>(string key)
        {
            ApplicationUser user = this.ApplicationUsers
                .FirstOrDefault(x => x.NormalizedUserName == key);
            if (user != null)
            {
                ApplicationUserClaim claim = this.ApplicationUserClaims
                    .FirstOrDefault(x => x.UserId == user.Id
                                              && x.ClaimType == "GoogleAccessToken");
                if (claim != null)
                {
                    return JsonConvert.DeserializeObject<T>(claim.ClaimValue);
                }

                return default(T);
            }

            throw new Exception($"Cannot retrieve Google Access Token for user '{key}', no such user found.");
        }

        public void ClearAsync()
        {
            IQueryable<ApplicationUserClaim> claims =
                this.ApplicationUserClaims.Where(x => x.ClaimType == "GoogleAccessToken");
            this.ApplicationUserClaims.RemoveRange(claims);
        }

        public string GetRefreshToken(string key)
        {
            ApplicationUser user = this.ApplicationUsers
                .FirstOrDefault(x => x.NormalizedUserName == key);
            if (user != null)
            {
                ApplicationUserClaim claim = this.ApplicationUserClaims
                    .FirstOrDefault(x => x.UserId == user.Id
                                              && x.ClaimType == "GoogleRefreshToken");
                return claim?.ClaimValue;
            }

            throw new Exception($"Cannot retrieve Google Access Token for user '{key}', no such user found.");
        }
    }
}