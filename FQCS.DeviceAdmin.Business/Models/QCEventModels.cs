using Newtonsoft.Json;
using FQCS.DeviceAdmin.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.Business.Models
{
    public class CreateQCEventModel : MappingModel<QCEvent>
    {
        public CreateQCEventModel()
        {
        }

        public CreateQCEventModel(QCEvent src) : base(src)
        {
        }

        [JsonProperty("defect_type_code")]
        public string DefectTypeCode { get; set; }
        [JsonProperty("created_time_str")]
        public string CreatedTimeStr { get; set; }
        [JsonProperty("date_format")]
        public string DateFormat { get; set; }
        [JsonProperty("left_image")]
        public string LeftImage { get; set; }
        [JsonProperty("right_image")]
        public string RightImage { get; set; }
    }

    #region Query
    public class QCEventQueryProjection
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


    public class QCEventQuerySort
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

    public class QCEventQueryFilter
    {
        public string defect_type { get; set; }
    }

    public class QCEventQueryPaging
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

    public class QCEventQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }
    #endregion
}
