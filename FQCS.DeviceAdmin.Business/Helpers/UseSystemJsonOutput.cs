using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.Business
{
    public class UseSystemJsonOutput : ActionFilterAttribute
    {
        private static readonly JsonSerializerOptions _defaultOpts =
            new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };

        public override void OnActionExecuted(ActionExecutedContext ctx)
        {
            if (ctx.Result is ObjectResult objectResult)
                AddFormatter(objectResult);
            else if (ctx.Result is JsonResult jsonResult)
            {
                ctx.Result = new OkObjectResult(jsonResult.Value);
                objectResult = ctx.Result as ObjectResult;
                AddFormatter(objectResult);
            }
        }

        private void AddFormatter(ObjectResult objectResult)
        {
            objectResult.Formatters.Add(new SystemTextJsonOutputFormatter(_defaultOpts));
        }
    }
}
