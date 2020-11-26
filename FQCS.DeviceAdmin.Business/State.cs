using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace FQCS.DeviceAdmin.Business
{
    public class State
    {
        public DateTime? LastEventTime { get; set; }

        private static State _instance;
        public static State Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new State();
                return _instance;
            }
        }
    }

}
