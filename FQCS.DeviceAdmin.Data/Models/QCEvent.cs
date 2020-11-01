﻿using System;
using System.Collections.Generic;
using System.Text;
using static FQCS.DeviceAdmin.Data.Constants;

namespace FQCS.DeviceAdmin.Data.Models
{
    public class QCEvent
    {
        // cache
        public static ISet<string> CheckedEvents { get; } = new HashSet<string>();

        public string Id { get; set; }
        public string DefectTypeCode { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedTime { get; set; }
        public string LeftImage { get; set; }
        public string RightImage { get; set; }
        public bool NotiSent { get; set; }
    }
}
