using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.WebApi
{
    public class Settings
    {
        public string WebRootPath { get; set; }
        public string UploadFolderPath { get; set; }
        public double TokenValidHours { get; set; }
        public double RefreshTokenValidHours { get; set; }

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
