using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.Business.Helpers
{
    public class DefaultDateTimeConverter : IsoDateTimeConverter
    {
        protected string[] dateFormats;
        public DefaultDateTimeConverter()
        {
            this.Culture = CultureInfo.InvariantCulture;
            this.DateTimeStyles = DateTimeStyles.None;
            this.DateTimeFormat = Constants.AppDateTimeFormat.DEFAULT_DATE_FORMAT;
        }

        public DefaultDateTimeConverter(string dateFormatsStr) : this()
        {
            var split = dateFormatsStr?.Split('\t');
            if (split?.Length > 0)
                this.dateFormats = split;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                DateTime? dateTime = null;
                if (this.dateFormats == null)
                    dateTime = base.ReadJson(reader, objectType, existingValue, serializer) as DateTime?;
                else
                {
                    var dateStr = reader.Value as string;
                    DateTime dt;
                    if (dateStr.TryConvertToDateTime(dateFormats: this.dateFormats, out dt))
                        dateTime = dt;
                }
                if (dateTime != null)
                    dateTime = dateTime?.ToUtc();
                return dateTime ?? default;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
