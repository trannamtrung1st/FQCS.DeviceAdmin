using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FQCS.DeviceAdmin.WebAdmin.Models;
using TNT.Core.Http.DI;

namespace FQCS.DeviceAdmin.WebAdmin.Pages.DeviceConfig
{
    [InjectionFilter]
    public class CreateModel : BasePageModel<CreateModel>
    {
        public void OnGet()
        {
            SetPageInfo();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Create device config",
                Menu = Constants.Menu.DEVICE_CONFIG,
                BackUrl = BackUrl ?? Constants.Routing.DEVICE_CONFIG
            };
        }
    }
}
