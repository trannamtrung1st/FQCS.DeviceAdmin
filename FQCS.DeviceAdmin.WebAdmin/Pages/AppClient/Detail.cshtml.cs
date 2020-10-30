using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FQCS.DeviceAdmin.WebAdmin.Models;
using TNT.Core.Http.DI;

namespace FQCS.DeviceAdmin.WebAdmin.Pages.AppClient
{
    [InjectionFilter]
    public class DetailModel : BasePageModel<DetailModel>
    {
        public string Id { get; set; }
        public void OnGet(string id)
        {
            SetPageInfo();
            Id = id;
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "App client detail",
                Menu = Constants.Menu.APP_CLIENT,
                BackUrl = BackUrl ?? Constants.Routing.APP_CLIENT
            };
        }
    }
}
