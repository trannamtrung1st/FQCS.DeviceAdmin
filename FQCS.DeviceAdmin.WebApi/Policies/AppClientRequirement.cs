using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.WebApi.Policies
{
    public class AppClientRequirement : IAuthorizationRequirement
    {
        public AppClientRequirement()
        {
        }
    }
}
