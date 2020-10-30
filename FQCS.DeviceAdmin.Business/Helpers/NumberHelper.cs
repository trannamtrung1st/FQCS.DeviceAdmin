using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.Business.Helpers
{
    public static class NumberHelper
    {
        public static string ToCommaGroup(this double number)
        {
            return number.ToString("#,#", CultureInfo.InvariantCulture);
        }
    }
}
