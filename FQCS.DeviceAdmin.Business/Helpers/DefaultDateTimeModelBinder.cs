using FQCS.DeviceAdmin.Business;
using FQCS.DeviceAdmin.Business.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.Business.Helpers
{

    public class DefaultDateTimeModelBinder : IModelBinder
    {
        public static readonly Type[] SUPPORTED_TYPES = new Type[] { typeof(DateTime), typeof(DateTime?) };

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            if (!SUPPORTED_TYPES.Contains(bindingContext.ModelType))
            {
                return Task.CompletedTask;
            }

            var modelName = GetModelName(bindingContext);

            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var dateToParse = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(dateToParse))
            {
                return Task.CompletedTask;
            }

            var dateTime = ParseDate(bindingContext, dateToParse);

            bindingContext.Result = ModelBindingResult.Success(dateTime);

            return Task.CompletedTask;
        }

        private DateTime? ParseDate(ModelBindingContext bindingContext, string dateStr)
        {
            var attribute = GetBinderAttribute(bindingContext);
            var dateFormat = attribute?.DateFormat ?? Constants.AppDateTimeFormat.DEFAULT_DATE_FORMAT;
            var toUtc = attribute?.ToUtc;

            DateTime dateTime;
            if (dateStr.TryConvertToDateTime(dateFormat, out dateTime))
            {
                if (toUtc == true)
                    dateTime = dateTime.ToUtc();
                return dateTime;
            }
            return null;
        }

        private DefaultDateTimeModelBinderAttribute GetBinderAttribute(ModelBindingContext bindingContext)
        {
            var modelName = GetModelName(bindingContext);

            var paramDescriptor = bindingContext.ActionContext.ActionDescriptor.Parameters
                .Where(x => SUPPORTED_TYPES.Contains(x.ParameterType))
                .Where((x) =>
                {
                    // See comment in GetModelName() on why we do this.
                    var paramModelName = x.BindingInfo?.BinderModelName ?? x.Name;
                    return paramModelName.Equals(modelName);
                })
                .FirstOrDefault();

            var ctrlParamDescriptor = paramDescriptor as ControllerParameterDescriptor;
            if (ctrlParamDescriptor == null)
            {
                var propAttr = bindingContext.ModelMetadata
                    .ContainerType.GetProperty(modelName)
                    .GetCustomAttributes(typeof(DefaultDateTimeModelBinderAttribute), false)
                    .FirstOrDefault();
                return propAttr != null ?
                    (DefaultDateTimeModelBinderAttribute)propAttr : null;
            }

            var attribute = ctrlParamDescriptor.ParameterInfo
                .GetCustomAttributes(typeof(DefaultDateTimeModelBinderAttribute), false)
                .FirstOrDefault();

            return (DefaultDateTimeModelBinderAttribute)attribute;
        }

        private string GetModelName(ModelBindingContext bindingContext)
        {
            // The "Name" property of the ModelBinder attribute can be used to specify the
            // route parameter name when the action parameter name is different from the route parameter name.
            // For instance, when the route is /api/{birthDate} and the action parameter name is "date".
            // We can add this attribute with a Name property [DateTimeModelBinder(Name ="birthDate")]
            // Now bindingContext.BinderModelName will be "birthDate" and bindingContext.ModelName will be "date"
            if (!string.IsNullOrEmpty(bindingContext.BinderModelName))
            {
                return bindingContext.BinderModelName;
            }

            return bindingContext.ModelName;
        }
    }

    public class DefaultDateTimeModelBinderAttribute : ModelBinderAttribute
    {
        public string DateFormat { get; set; }
        public bool ToUtc { get; set; } = true;

        public DefaultDateTimeModelBinderAttribute()
            : base(typeof(DefaultDateTimeModelBinder))
        {
        }
    }
}
