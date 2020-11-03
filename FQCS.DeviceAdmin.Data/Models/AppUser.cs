using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.DeviceAdmin.Data.Models
{
    public class AppUser : IdentityUser<string>
    {
        public AppUser()
        {
        }

        public string FullName { get; set; }
        public bool Disabled { get; set; }

        public virtual IList<AppUserRole> UserRoles { get; set; }
    }
}
