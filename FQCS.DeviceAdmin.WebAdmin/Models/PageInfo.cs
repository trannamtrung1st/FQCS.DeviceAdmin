using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.WebAdmin.Models
{

    public class PageInfo
    {
        public string Title { get; set; }
        public string Menu { get; set; }
        public string Description { get; set; }
        //back by default
        public string BackUrl { get; set; } = "javascript:history.back()";
    }
}
