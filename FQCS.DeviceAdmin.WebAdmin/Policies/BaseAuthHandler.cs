using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace FQCS.DeviceAdmin.WebAdmin.Policies
{
    public abstract class BaseAuthHandler<T> : AuthorizationHandler<T> where T : IAuthorizationRequirement
    {
        public BaseAuthHandler(ServiceInjection inj)
        {
            inj.Inject(this);
        }

        protected void SucceedORRequirement(AuthorizationHandlerContext context)
        {
            foreach (var req in context.PendingRequirements.ToList())
            {
                if (req is LogicRequirement)
                {
                    var logReq = req as LogicRequirement;
                    if (logReq.IsOR)
                        context.Succeed(logReq);
                }
            }
        }
    }
}
