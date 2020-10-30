using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.WebAdmin.Helpers
{
    public static class RoutingHelper
    {
        public static string Id(this string route, int id)
        {
            return route.Replace("{id}", id.ToString());
        }
    }
}
