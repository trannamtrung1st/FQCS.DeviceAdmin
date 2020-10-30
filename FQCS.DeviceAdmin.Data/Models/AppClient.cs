using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.DeviceAdmin.Data.Models
{
    public class AppClient
    {
        public string Id { get; set; }
        public string SecretKey { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
    }
}
