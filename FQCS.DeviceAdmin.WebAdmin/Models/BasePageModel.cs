using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace FQCS.DeviceAdmin.WebAdmin.Models
{

    public abstract class BasePageModel<T> : PageModel, IInfoPageModel
    {
        public PageInfo Info { get; set; }
        [BindProperty(SupportsGet = true, Name = "back_url")]
        public virtual string BackUrl { get; set; }

        protected abstract void SetPageInfo();
    }
}
