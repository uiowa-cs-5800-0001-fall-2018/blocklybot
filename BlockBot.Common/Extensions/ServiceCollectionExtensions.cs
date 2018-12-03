using BlockBot.Common.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BlockBot.Common.Extensions
{
    /// <summary>
    ///     Class for extensions to the <see cref="IServiceCollection"/>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUserStore(this IServiceCollection services)
        {
            services.AddTransient<IUserStore<ApplicationUser>, ApplicationUserStore>();
            services.AddTransient<ApplicationUserStore, ApplicationUserStore>();
            return services;
        }

        public static IServiceCollection AddRoleStore(this IServiceCollection services)
        {
            services.AddTransient<IRoleStore<ApplicationRole>, ApplicationRoleStore>();
            services.AddTransient<ApplicationRoleStore, ApplicationRoleStore>();
            return services;
        }

        public static IServiceCollection AddUserManager(this IServiceCollection services)
        {
            services.AddTransient<UserManager<ApplicationUser>, ApplicationUserManager>();
            services.AddTransient<ApplicationUserManager, ApplicationUserManager>();
            return services;
        }

        public static IServiceCollection AddRoleManager(this IServiceCollection services)
        {
            services.AddTransient<RoleManager<ApplicationRole>, ApplicationRoleManager>();
            services.AddTransient<ApplicationRoleManager, ApplicationRoleManager>();
            return services;
        }

        public static IServiceCollection AddSignInManager(this IServiceCollection services)
        {
            services.AddTransient<SignInManager<ApplicationUser>, ApplicationSignInManager>();
            services.AddTransient<ApplicationSignInManager, ApplicationSignInManager>();
            return services;
        }
    }
}
