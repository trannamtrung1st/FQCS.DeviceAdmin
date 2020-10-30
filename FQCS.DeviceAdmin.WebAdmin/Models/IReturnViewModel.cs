using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.WebAdmin.Models
{
    public interface IReturnViewModel
    {
        string Layout { get; set; }
        PageInfo Info { get; set; }
    }
}
