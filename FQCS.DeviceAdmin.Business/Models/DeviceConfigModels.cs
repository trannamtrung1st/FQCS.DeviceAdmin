using Newtonsoft.Json;
using FQCS.DeviceAdmin.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.Business.Models
{
    public class CreateDeviceConfigModel : MappingModel<DeviceConfig>
    {
        public CreateDeviceConfigModel()
        {
        }

        public CreateDeviceConfigModel(DeviceConfig src) : base(src)
        {
        }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }
        [JsonProperty("kafka_server")]
        public string KafkaServer { get; set; }
        [JsonProperty("kafka_username")]
        public string KafkaUsername { get; set; }
        // encrypted
        [JsonProperty("kafka_password")]
        public string KafkaPassword { get; set; }
    }

    public class UpdateDeviceConfigModel : MappingModel<DeviceConfig>
    {
        public UpdateDeviceConfigModel()
        {
        }

        public UpdateDeviceConfigModel(DeviceConfig src) : base(src)
        {
        }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }
        [JsonProperty("kafka_server")]
        public string KafkaServer { get; set; }
        [JsonProperty("kafka_username")]
        public string KafkaUsername { get; set; }
        // encrypted
        [JsonProperty("kafka_password")]
        public string KafkaPassword { get; set; }

    }

    public class ChangeCurrentDeviceConfigModel
    {
        [JsonProperty("config_id")]
        public int ConfigId { get; set; }
    }

    #region Query
    public class DeviceConfigQueryProjection
    {
        private const string DEFAULT = INFO;
        private string _fields = DEFAULT;
        public string fields
        {
            get
            {
                return _fields;
            }
            set
            {
                if (value?.Length > 0)
                {
                    _fields = value;
                    _fieldsArr = value.Split(',').OrderBy(v => v).ToArray();
                }
            }
        }

        private string[] _fieldsArr = DEFAULT.Split(',');
        public string[] GetFieldsArr()
        {
            return _fieldsArr;
        }

        //---------------------------------------

        public const string INFO = "info";
        public const string SELECT = "select";

        public static readonly IDictionary<string, string[]> MAPS = new Dictionary<string, string[]>
        {
        };
    }


    public class DeviceConfigQuerySort
    {
        public const string TIME = "time";
        private const string DEFAULT = "a" + TIME;
        private string _sorts = DEFAULT;
        public string sorts
        {
            get
            {
                return _sorts;
            }
            set
            {
                if (value?.Length > 0)
                {
                    _sorts = value;
                    _sortsArr = value.Split(',');
                }
            }
        }

        public string[] _sortsArr = DEFAULT.Split(',');
        public string[] GetSortsArr()
        {
            return _sortsArr;
        }

    }

    public class DeviceConfigQueryFilter
    {
        public int? id { get; set; }
    }

    public class DeviceConfigQueryPaging
    {
        private int _page = 1;
        public int page
        {
            get
            {
                return _page;
            }
            set
            {
                _page = value > 0 ? value : _page;
            }
        }

        private int _limit = 10;
        public int limit
        {
            get
            {
                return _limit;
            }
            set
            {
                if (value >= 1 && value <= 100)
                    _limit = value;
            }
        }
    }

    public class DeviceConfigQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }
    #endregion
}
