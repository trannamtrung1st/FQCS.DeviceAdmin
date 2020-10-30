using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.WebAdmin.Models
{
    public interface IErrorModel<T> : IReturnViewModel
    {
        string RequestId { get; set; }
        bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        string Message { get; set; }
    }
}
