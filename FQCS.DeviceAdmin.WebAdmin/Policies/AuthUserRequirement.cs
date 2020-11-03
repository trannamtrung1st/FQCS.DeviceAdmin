using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.WebAdmin.Policies
{
    public class AuthUserRequirement : LogicRequirement
    {
        public string Role { get; }
        public AuthUserRequirement(string role = null, bool isOR = false) : base(isOR)
        {
            Role = role;
        }
    }
}
