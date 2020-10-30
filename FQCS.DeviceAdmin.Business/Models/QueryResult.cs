using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.Business.Models
{

    public class QueryResult<T>
    {
        [JsonProperty("list", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<T> List { get; set; }
        [JsonProperty("single", NullValueHandling = NullValueHandling.Ignore)]
        public T Single { get; set; }
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public int? Count { get; set; }
    }
}
