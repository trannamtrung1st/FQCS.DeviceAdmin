using FQCS.DeviceAdmin.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FQCS.DeviceAdmin.Business.Models
{
    public class CreateAppUserModel : MappingModel<AppUser>
    {
        public CreateAppUserModel()
        {
        }

        public CreateAppUserModel(AppUser src) : base(src)
        {
        }

        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("username")]
        public string UserName { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }
    }

    public class UpdateAppUserModel : MappingModel<AppUser>
    {
        public UpdateAppUserModel()
        {
        }

        public UpdateAppUserModel(AppUser src) : base(src)
        {
        }

        [JsonProperty("password_reset")]
        public string PasswordReset { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
    }

    #region Query
    public class AppUserQueryProjection
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
        public const string ROLE = "role";

        public static readonly IDictionary<string, string[]> MAPS = new Dictionary<string, string[]>
        {
            { ROLE, new[]{ nameof(AppUser.UserRoles) } }
        };
    }

    public class AppUserQuerySort
    {
        public const string USERNAME = "username";
        private const string DEFAULT = "a" + USERNAME;
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

    public class AppUserQueryFilter
    {
        public string id { get; set; }
        public string[] ids { get; set; }
        public string not_eq_id { get; set; }
        public string uname_contains { get; set; }
    }

    public class AppUserQueryPaging
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

    public class AppUserQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }
    #endregion
}
