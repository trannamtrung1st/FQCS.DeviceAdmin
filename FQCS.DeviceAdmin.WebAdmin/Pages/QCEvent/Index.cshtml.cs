using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NLog.LayoutRenderers;
using FQCS.DeviceAdmin.WebAdmin.Models;
using TNT.Core.Http.DI;

namespace FQCS.DeviceAdmin.WebAdmin.Pages.QCEvent
{
    [InjectionFilter]
    public class IndexModel : BasePageModel<IndexModel>
    {
        public void OnGet()
        {
            SetPageInfo();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Menu = Constants.Menu.QC_EVENT,
                Title = "QC event",
                BackUrl = BackUrl ?? Constants.Routing.DASHBOARD
            };
        }
    }
}
