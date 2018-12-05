using System;
using Microsoft.AspNetCore.Identity;

namespace BlockBot.Common.Data
{
    public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
    {
        public virtual ApplicationRole Role { get; set; }
    }
}
