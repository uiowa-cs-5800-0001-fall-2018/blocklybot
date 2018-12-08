using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BlockBot.Common.Data;
using BlockBot.Common.Extensions;
using BlockBot.Module.Aws.Extensions;
using BlockBot.Module.BlockBot.Extensions;
using BlockBot.Module.Google.Extensions;
using BlockBot.Module.SendGrid.Extensions;
using BlockBot.Module.Twilio.Extensions;
using Microsoft.AspNetCore.Authentication;
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
using BlockBot.module.Integrations.Extensions;

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
                        .UseSqlite(Configuration.GetConnectionString("MacConnection"),
                            builder => builder.MigrationsAssembly(typeof(Startup).Assembly.FullName)));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options
                        .UseLazyLoadingProxies()
                        .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                            builder => builder.MigrationsAssembly(typeof(Startup).Assembly.FullName))
                );
            }

            services.AddIdentity<ApplicationUser, ApplicationRole>(o => { o.SignIn.RequireConfirmedEmail = false; })
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
                googleOptions.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
                googleOptions.Scope.Add("https://www.googleapis.com/auth/calendar.readonly");
                googleOptions.Scope.Add("https://www.googleapis.com/auth/calendar.events");
                googleOptions.SaveTokens = true;
                googleOptions.AccessType = "offline";
                googleOptions.Events.OnCreatingTicket = ctx =>
                {
                    List<AuthenticationToken> tokens = ctx.Properties.GetTokens()
                        as List<AuthenticationToken>;
                    tokens.Add(new AuthenticationToken
                    {
                        Name = "TicketCreated",
                        Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
                    });
                    ctx.Properties.StoreTokens(tokens);
                    return Task.CompletedTask;
                };
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

            // register Integrations services
            services.AddIntegrationsServices();

            // register SendGrid services
            services.AddSendGridServices();

            // register Twilio services
            services.AddTwilioServices();
            services.AddTwilioIntegrationServices();


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
                //app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();

                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
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