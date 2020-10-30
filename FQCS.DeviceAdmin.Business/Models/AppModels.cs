using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FQCS.DeviceAdmin.Data;
using TNT.Core.Helpers.General;

namespace FQCS.DeviceAdmin.Business.Models
{

    public class AppResult
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("data")]
        public object Data { get; set; }
        [JsonProperty("code")]
        public Constants.AppResultCode? Code { get; set; }

        public static AppResult Success(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = Constants.AppResultCode.Success,
                Message = mess ?? Constants.AppResultCode.Success.DisplayName(),
                Data = data,
            };
        }

        public static AppResult Error(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = Constants.AppResultCode.UnknownError,
                Message = mess ?? Constants.AppResultCode.UnknownError.DisplayName(),
                Data = data,
            };
        }

        public static AppResult DependencyDeleteFail(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = Constants.AppResultCode.DependencyDeleteFail,
                Message = mess ?? Constants.AppResultCode.DependencyDeleteFail.DisplayName(),
                Data = data,
            };
        }

        public static AppResult FailValidation(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = Constants.AppResultCode.FailValidation,
                Message = mess ?? Constants.AppResultCode.FailValidation.DisplayName(),
                Data = data,
            };
        }

        public static AppResult NotFound(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = Constants.AppResultCode.NotFound,
                Message = mess ?? Constants.AppResultCode.NotFound.DisplayName(),
                Data = data,
            };
        }

        public static AppResult Unsupported(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = Constants.AppResultCode.Unsupported,
                Message = mess ?? Constants.AppResultCode.Unsupported.DisplayName(),
                Data = data,
            };
        }

        public static AppResult Unauthorized(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = Constants.AppResultCode.Unauthorized,
                Message = mess ?? Constants.AppResultCode.Unauthorized.DisplayName(),
                Data = data,
            };
        }

        public static AppResult DuplicatedUsername(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = Constants.AppResultCode.DuplicatedUsername,
                Message = mess ?? Constants.AppResultCode.DuplicatedUsername.DisplayName(),
                Data = data,
            };
        }

    }

    public class ValidationData
    {
        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }
        [JsonProperty("results")]
        public List<AppResult> Results { get; set; }
        [JsonIgnore]
        public IDictionary<string, object> TempData { get; set; }

        public ValidationData()
        {
            Results = new List<AppResult>();
            IsValid = true;
            TempData = new Dictionary<string, object>();
        }

        public T GetTempData<T>(string key)
        {
            object data = null;
            if (TempData.TryGetValue(key, out data))
                return (T)data;
            return default;
        }

        public ValidationData Fail(string mess = null, Constants.AppResultCode? code = null, object data = null)
        {
            Results.Add(new AppResult
            {
                Message = mess ?? code?.DisplayName(),
                Data = data,
                Code = code
            });
            IsValid = false;
            return this;
        }

    }

}
