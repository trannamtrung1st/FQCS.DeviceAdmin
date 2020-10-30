using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Business.Services;
using FQCS.DeviceAdmin.Data.Helpers;
using FQCS.DeviceAdmin.WebAdmin.Helpers;
using FQCS.DeviceAdmin.WebAdmin.Models;
using TNT.Core.Helpers.DI;
using TNT.Core.Http.DI;

namespace FQCS.DeviceAdmin.WebAdmin.Pages.Identity
{
    [InjectionFilter]
    public class LoginModel : BasePageModel<LoginModel>, IMessageModel
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        [Inject]
        protected readonly IdentityService identityService;
        public string Message { get; set; } = null;
        public string MessageTitle { get; set; } = null;
        public string Layout { get; set; } = null;

        public IActionResult OnGet(string return_url = Constants.Routing.DASHBOARD)
        {
            if (User.Identity.IsAuthenticated) return LocalRedirect(return_url);
            SetPageInfo();
            return Page();
        }

        public async Task<IActionResult> OnPost(Business.Models.LoginModel model,
            string return_url = Constants.Routing.DASHBOARD)
        {
            SetPageInfo();
            //this is auto handled by anti-forgery: return 400 bad request
            if (User.Identity.IsAuthenticated)
            {
                Message = "Already logged in";
                MessageTitle = "Already logged in";
                return this.MessageView();
            }
            var entity = await identityService.AuthenticateAsync(model.username, model.password);
            if (entity != null)
            {
                #region Custom Signin for extra claims store
                var principal = await identityService.GetApplicationPrincipalAsync(entity);
                var utcNow = DateTime.UtcNow;
                var cookieProps = new AuthenticationProperties()
                {
                    IssuedUtc = utcNow,
                };
                if (model.remember_me)
                {
                    cookieProps.IsPersistent = true;
                    cookieProps.ExpiresUtc = utcNow.AddHours(Settings.Instance.CookiePersistentHours);
                }
                await HttpContext.SignInAsync(principal.Identity.AuthenticationType,
                    principal, cookieProps);
                #endregion
                _logger.CustomProperties(entity).Info("Login user");
                #region Generate token
                var identity =
                    await identityService.GetIdentityAsync(entity, JwtBearerDefaults.AuthenticationScheme);
                principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties()
                {
                    IssuedUtc = utcNow,
                    ExpiresUtc = utcNow.AddHours(WebAdmin.Settings.Instance.TokenValidHours)
                };
                props.Parameters["refresh_expires"] = utcNow.AddHours(
                    WebAdmin.Settings.Instance.RefreshTokenValidHours);
                var resp = identityService.GenerateTokenResponse(principal, props);
                #endregion
                return LocalRedirect($"{Constants.Routing.INDEX}?access_token=" +
                            $"{resp.access_token}" +
                            $"&refresh_token={resp.refresh_token}" +
                            $"&expires_utc={resp.expires_utc}" +
                            $"&issued_utc={resp.issued_utc}" +
                            $"&token_type={resp.token_type}&" +
                            $"&return_url={return_url}");
            }
            Message = "Invalid username or password";
            return Page();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Log in"
            };
        }
    }
}
