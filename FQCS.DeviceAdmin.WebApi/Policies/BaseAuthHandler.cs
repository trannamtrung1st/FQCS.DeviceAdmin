using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace FQCS.DeviceAdmin.WebApi.Policies
{
    public abstract class BaseAuthHandler<T> : AuthorizationHandler<T> where T : IAuthorizationRequirement
    {
        public BaseAuthHandler(ServiceInjection inj)
        {
            inj.Inject(this);
        }
    }
}
