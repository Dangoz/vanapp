using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Server.Models
{
    public class UserRole : IdentityUserRole<int>
    {
        public SiteUser User { get; set; }
        public Role Role { get; set; }
    }
}