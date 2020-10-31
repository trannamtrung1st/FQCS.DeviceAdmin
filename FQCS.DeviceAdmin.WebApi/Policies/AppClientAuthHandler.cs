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
    public class AppClientAuthHandler : BaseAuthHandler<AppClientRequirement>
    {
        [Inject]
        protected readonly IdentityService identityService;
        [Inject]
        protected readonly AppClientService appClientService;
        [Inject]
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppClientAuthHandler(ServiceInjection inj) : base(inj)
        {
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       AppClientRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext.Request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                try
                {
                    var authHeader = httpContext.Request.Headers[HeaderNames.Authorization];
                    var validScheme = identityService.IsValidAppClientScheme(authHeader);
                    if (validScheme)
                    {
                        var clientInfo = identityService.ExtractInfoFromClientAuthHeader(authHeader);
                        var appClient = appClientService.AppClients.Id(clientInfo[0]).FirstOrDefault();
                        var validationResult = identityService.ValidateClientRequest(
                            clientInfo[1], clientInfo[2], clientInfo[3], appClient);
                        if (validationResult.IsValid)
                        {
                            httpContext.Items[Constants.RequestItemKey.CLIENT_ID] = appClient.Id;
                            context.Succeed(requirement);
                            SucceedORRequirement(context);
                            return Task.CompletedTask;
                        }
                    }
                }
                catch (Exception) { }
            }
            return Task.CompletedTask;
        }
    }
}
