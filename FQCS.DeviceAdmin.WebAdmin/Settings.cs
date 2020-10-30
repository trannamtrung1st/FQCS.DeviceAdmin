using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.WebAdmin
{
    public class Settings
    {
        public double CookiePersistentHours { get; set; }
        public double TokenValidHours { get; set; }
        public double RefreshTokenValidHours { get; set; }
        public string ApiUrl { get; set; }

        private static Settings _instance;
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Settings();
                return _instance;
            }
        }
    }

}
