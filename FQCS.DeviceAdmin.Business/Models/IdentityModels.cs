using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using FQCS.DeviceAdmin.Data.Models;

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

    public class TokenInfo
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("claims")]
        public IDictionary<string, IEnumerable<string>> Claims { get; }

        public TokenInfo() { }
        public TokenInfo(ClaimsPrincipal principal)
        {
            UserId = principal.Identity.Name;

            Claims = new Dictionary<string, IEnumerable<string>>(principal.Claims.GroupBy(c => c.Type)
                .Select(group => new KeyValuePair<string, IEnumerable<string>>(
                    group.Key, group.Select(c => c.Value).ToList())));
        }
    }

    public class AuthorizationGrantModel //Resource Owner Password Credentials Grant
    {
        [JsonProperty("username")]
        public string username { get; set; }
        //REQUIRED.  The resource owner username.

        [JsonProperty("password")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        //REQUIRED.  The resource owner password.

        [JsonProperty("grant_type")]
        public string grant_type { get; set; }
        //REQUIRED. 
        //- password: grant username and password
        //- refresh_token: grant refresh_token

        [JsonProperty("refresh_token")]
        public string refresh_token { get; set; }
        //OPTIONAL.  The refresh_token

        [JsonProperty("scope")]
        public string scope { get; set; }
        //OPTIONAL.  The scope of the access request as described by
    }

    public class AddRolesToUserModel
    {
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("roles")]
        public List<string> roles { get; set; }
    }

    public class RemoveRolesFromUserModel
    {
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("roles")]
        public List<string> roles { get; set; }
    }

    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool remember_me { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        [Display(Name = "Username")]
        public string username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Required]
        [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
        public string confirm_password { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string full_name { get; set; }
    }

    public class TokenResponseModel
    {
        [JsonProperty("user_id")]
        public string user_id { get; set; }
        [JsonProperty("access_token")]
        public string access_token { get; set; }
        [JsonProperty("token_type")]
        public string token_type { get; set; }
        [JsonProperty("expires_utc")]
        public string expires_utc { get; set; }
        [JsonProperty("issued_utc")]
        public string issued_utc { get; set; }
        [JsonProperty("refresh_token")]
        public string refresh_token { get; set; }
        [JsonProperty("roles")]
        public IEnumerable<string> roles { get; set; }
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
