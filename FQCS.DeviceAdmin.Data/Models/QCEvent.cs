using System;
using System.Collections.Generic;
using System.Text;
using static FQCS.DeviceAdmin.Data.Constants;

namespace FQCS.DeviceAdmin.Data.Models
{
    public class QCEvent
    {
        public QCEvent()
        {
            Details = new List<QCEventDetail>();
        }

        // cache
        public static ISet<string> CheckedEvents { get; } = new HashSet<string>();

        public string Id { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedTime { get; set; }
        public string LeftImage { get; set; }
        public string RightImage { get; set; }
        public string SideImages { get; set; }
        public bool NotiSent { get; set; }

        public virtual IList<QCEventDetail> Details { get; set; }
    }
}
