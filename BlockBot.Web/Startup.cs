using System;
using System.Runtime.InteropServices;
using BlockBot.Module.Aws.Extensions;
using BlockBot.Module.BlockBot.Extensions;
using BlockBot.Module.Google.Extensions;
using BlockBot.Module.SendGrid.Extensions;
using BlockBot.Module.Twilio.Extensions;
using BlockBot.Web.Data;
using BlockBot.Web.Extensions;
using BlockBot.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlockBot.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = 
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            if (Environment.IsDevelopment())
            {
                //services.AddHttpsRedirection(options =>
                //{
                //    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                //    options.HttpsPort = 44305;
                //});
            }
            else
            {
                //services.AddHsts(options =>
                //{
                //    options.Preload = true;
                //    options.IncludeSubDomains = true;
                //    options.MaxAge = TimeSpan.FromDays(1); // TODO gradually increase
                //});

                //services.AddHttpsRedirection(options =>
                //{
                //    //options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                //    options.HttpsPort = 443;
                //});
            }

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                //options.HttpOnly = HttpOnlyPolicy.Always;
            });

            // Add DB context
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options
                        .UseLazyLoadingProxies()
                        .UseSqlite(Configuration.GetConnectionString("MacConnection")));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options
                        .UseLazyLoadingProxies()
                        .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            }

            services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
                {
                    o.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            // build authorization policy
            AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            services.AddMvc(options =>
                {
                    // require authorization by default
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // TODO convert to extension methods
            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration.GetGoogleClientId();
                googleOptions.ClientSecret = Configuration.GetGoogleClientSecret();
            });

            //
            // Dependency injection registration
            //

            // register configuration object
            services.AddSingleton(Configuration);

            // register AWS services
            services.AddAwsCredentials(Configuration);
            services.AddAwsRegion();
            services.AddAwsServices();

            // register BlockBot services
            services.AddBlockBotIntegrationServices();

            // register Google services
            services.AddGoogleServices();

            // register SendGrid services
            services.AddSendGridServices();

            // register Twilio services
            services.AddTwilioServices();
            services.AddTwilioIntegrationServices();

            // TODO move to extensions
            services.AddTransient<IntegrationCreationService,IntegrationCreationService>();

            // register Identity services
            services.AddUserStore();
            services.AddRoleStore();
            services.AddUserManager();
            services.AddRoleManager();
            services.AddSignInManager();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                //app.UseExceptionHandler("/Home/Error");
                //app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}