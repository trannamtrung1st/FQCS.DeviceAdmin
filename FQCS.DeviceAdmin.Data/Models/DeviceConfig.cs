﻿using System;
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
    }
}