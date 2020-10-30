using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.General;

namespace FQCS.DeviceAdmin.Business.Helpers
{
    public static class StringExtensions
    {
        public static string ToSeoString(this string str)
        {
            return str.RemoveAccents()
                        .ToLowerInvariant().Trim()
                        .Replace(' ', '-');
        }
    }

}
