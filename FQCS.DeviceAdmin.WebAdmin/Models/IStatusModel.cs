using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.WebAdmin.Models
{
    public interface IStatusModel : IReturnViewModel
    {
        string Message { get; set; }
        string MessageTitle { get; set; }
        string OriginalUrl { get; set; }
        string StatusCodeStyle { get; set; }
        int Code { get; set; }
    }
}
