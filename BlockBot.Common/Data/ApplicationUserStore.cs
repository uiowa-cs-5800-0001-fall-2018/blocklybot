using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BlockBot.Common.Data
{
    public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationUserToken, ApplicationRoleClaim>
    {
        public ApplicationUserStore(ApplicationDbContext context): base(context)
        {
        }
    }
}
