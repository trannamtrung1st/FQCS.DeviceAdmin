using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using FQCS.DeviceAdmin.Data.Models;

namespace FQCS.DeviceAdmin.Business.Models
{
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

}
