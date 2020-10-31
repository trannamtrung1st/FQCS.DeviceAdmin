using FQCS.DeviceAdmin.Business.Queries;
using FQCS.DeviceAdmin.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace FQCS.DeviceAdmin.WebApi.Policies
{
    public class AuthUserAuthHandler : BaseAuthHandler<AuthUserRequirement>
    {

        public AuthUserAuthHandler(ServiceInjection inj) : base(inj)
        {
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       AuthUserRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                context.Succeed(requirement);
                SucceedORRequirement(context);
            }
            return Task.CompletedTask;
        }
    }
}
