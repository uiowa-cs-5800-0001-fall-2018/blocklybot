using System;
using Microsoft.AspNetCore.Identity;

namespace BlockBot.Common.Data
{
    public class ApplicationUserToken : IdentityUserToken<Guid>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
