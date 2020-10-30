using System;
using System.Collections.Generic;
using System.Text;
using static FQCS.DeviceAdmin.Data.Constants;

namespace FQCS.DeviceAdmin.Data.Models
{
    public class QCEvent
    {
        public int Id { get; set; }
        public string DefectTypeCode { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedTime { get; set; }
        public string LeftImage { get; set; }
        public string RightImage { get; set; }
    }
}
