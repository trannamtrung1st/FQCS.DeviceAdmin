using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.WebApi.Policies
{
    public class AuthUserRequirement : LogicRequirement
    {
        public string Role { get; }
        public AuthUserRequirement(bool isOR = false, string role = null) : base(isOR)
        {
            Role = role;
        }
    }
}
