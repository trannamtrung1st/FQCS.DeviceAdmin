using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.WebAdmin.Policies
{
    public abstract class LogicRequirement : IAuthorizationRequirement
    {
        public bool IsOR { get; set; }
        public LogicRequirement(bool isOR = false)
        {
            IsOR = isOR;
        }
    }
}
