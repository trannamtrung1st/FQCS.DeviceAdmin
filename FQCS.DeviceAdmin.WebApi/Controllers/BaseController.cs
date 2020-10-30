using elFinder.NetCore;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FQCS.DeviceAdmin.Business;
using FQCS.DeviceAdmin.Business.Models;
using TNT.Core.Helpers.DI;

namespace FQCS.DeviceAdmin.WebApi.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        [Inject]
        protected readonly DbContext context;

        public string UserId
        {
            get
            {
                return User.Identity.Name;
            }
        }

        protected T Service<T>()
        {
            return HttpContext.RequestServices.GetRequiredService<T>();
        }

        protected IActionResult Error(object obj = default)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, obj);
        }

        protected string GetAuthorityLeftPart()
        {
            return new Uri(Request.GetEncodedUrl()).GetLeftPart(UriPartial.Authority);
        }

    }
}
