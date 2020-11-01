using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.DeviceAdmin.Data.Models
{
    public class DeviceConfig
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string KafkaServer { get; set; }
        public string KafkaUsername { get; set; }
        // encrypted
        public string KafkaPassword { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsCurrent { get; set; }

        // Remove events job settings
        public string RemoveOldEventsJobSettings { get; set; }
        public DateTime? NextROEJobStart { get; set; }

        // Send unsent job settings
        public string SendUnsentEventsJobSettings { get; set; }
        public DateTime? NextSUEJobStart { get; set; }

    }
}
