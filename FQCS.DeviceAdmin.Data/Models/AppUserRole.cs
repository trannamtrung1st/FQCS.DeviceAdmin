using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.DeviceAdmin.Data.Models
{
    public class AppUserRole : IdentityUserRole<string>
    {
        public virtual AppUser User { get; set; }
        public virtual AppRole Role { get; set; }
    }
}
