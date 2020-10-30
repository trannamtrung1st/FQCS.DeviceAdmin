using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FQCS.DeviceAdmin.WebAdmin.Helpers;
using FQCS.DeviceAdmin.WebAdmin.Models;
using TNT.Core.Http.DI;

namespace FQCS.DeviceAdmin.WebAdmin.Pages
{
    [InjectionFilter]
    public class AccessDeniedModel : BasePageModel<AccessDeniedModel>, IStatusModel
    {
        public string Message { get; set; }
        public int Code { get; set; } = (int)HttpStatusCode.Forbidden;
        public string Layout { get; set; } = null;
        public string MessageTitle { get; set; }
        public string StatusCodeStyle { get; set; } = "danger";
        public string OriginalUrl { get; set; }

        public IActionResult OnGet(string return_url = null)
        {
            if (return_url == null) return LocalRedirect(Constants.Routing.DASHBOARD);
            SetPageInfo();
            OriginalUrl = return_url;
            return this.StatusView();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Access denied",
                BackUrl = BackUrl ?? Constants.Routing.DASHBOARD
            };
            MessageTitle = "Access denied";
            Message = "You don't have the right to access this resource";
        }
    }
}
