using Microsoft.AspNetCore.Mvc.ModelBinding;
using FQCS.DeviceAdmin.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FQCS.DeviceAdmin.Business;

namespace FQCS.DeviceAdmin.Business
{
    public static class ResultHelper
    {
        public static AppResult MakeInvalidAccountRegistrationResults(ModelStateDictionary modelState)
        {
            var validationData = new ValidationData();
            if (modelState.ContainsKey("password")
                && modelState["password"].ValidationState == ModelValidationState.Invalid)
                validationData.Fail(mess: "Invalid password", code: Constants.AppResultCode.FailValidation);
            if (modelState.ContainsKey("confirm_password")
                && modelState["confirm_password"].ValidationState == ModelValidationState.Invalid)
                validationData.Fail(mess: "The password and confirm password are not matched", code: Constants.AppResultCode.FailValidation);
            if (modelState.ContainsKey("username")
                && modelState["username"].ValidationState == ModelValidationState.Invalid)
                validationData.Fail(mess: "Invalid username", code: Constants.AppResultCode.FailValidation);
            if (modelState.ContainsKey("DuplicateUserName")
                && modelState["DuplicateUserName"].ValidationState == ModelValidationState.Invalid)
                return AppResult.DuplicatedUsername();
            var appResult = AppResult.FailValidation(data: validationData);
            return appResult;
        }
    }
}
