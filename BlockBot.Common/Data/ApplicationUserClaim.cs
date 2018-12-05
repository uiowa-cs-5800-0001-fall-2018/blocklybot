using System;
using Microsoft.AspNetCore.Identity;

namespace BlockBot.Common.Data
{
    public class ApplicationUserClaim : IdentityUserClaim<Guid>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
