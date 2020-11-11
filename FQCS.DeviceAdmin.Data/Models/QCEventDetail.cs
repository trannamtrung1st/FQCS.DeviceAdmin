using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.DeviceAdmin.Data.Models
{
    public class QCEventDetail
    {
        public string Id { get; set; }
        public string DefectTypeCode { get; set; }
        public string EventId { get; set; }

        public virtual QCEvent Event { get; set; }
    }
}
