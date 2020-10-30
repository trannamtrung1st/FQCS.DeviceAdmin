using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TNT.Core.Http.DI;
using TNT.Core.Helpers.DI;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using FQCS.DeviceAdmin.Business;
using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.WebAdmin.Models;
using System.Diagnostics;
using FQCS.DeviceAdmin.WebAdmin.Helpers;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace FQCS.DeviceAdmin.WebAdmin.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route(Constants.Routing.ERROR_CONTROLLER)]
    [ApiController]
    [InjectionFilter]
    public class ErrorController : BaseController, IErrorModel<ErrorController>
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Route("")]
        public IActionResult HandleException()
        {
            dynamic context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (context == null || context.Error == null) return LocalRedirect(Constants.Routing.DASHBOARD);
            var exception = context.Error as Exception;
            if (exception != null)
                _logger.Error(exception);
            var isApiRequest = context.Path.StartsWith("/api");
            if (isApiRequest)
            {
#if DEBUG
                return Error(AppResult.Error(data: exception));
#else
                return Error(AppResult.Error());
#endif
            }
            if (exception == null) return LocalRedirect(Constants.Routing.DASHBOARD);
            HttpContext.Items["model"] = this;
            SetPageInfo();
#if !RELEASE
            Message = exception.Message;
#else
            Message = "Something's wrong. Please contact admin for more information.";
#endif
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return this.ErrorView();

        }

        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string Message { get; set; }
        public string Layout { get; set; } = null;
        public PageInfo Info { get; set; }

        protected void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Error",
                BackUrl = Constants.Routing.DASHBOARD
            };
            Message = "Nothing";
        }
    }
}