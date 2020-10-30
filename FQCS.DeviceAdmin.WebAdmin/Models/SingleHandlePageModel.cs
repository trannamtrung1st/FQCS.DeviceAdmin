using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.WebAdmin.Models
{
    public abstract class SingleHandlePageModel<T> : BasePageModel<T>
    {
        protected abstract IActionResult OnAllMethod();

        public IActionResult OnGet()
        {
            return OnAllMethod();
        }

        public IActionResult OnPost()
        {
            return OnAllMethod();
        }
    }
}
