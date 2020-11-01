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
        public bool Read { get; }
        public bool Write { get; }
        public string[] DateFormat { get; }
        public DefaultDateTimeConverter()
        {
            this.Culture = CultureInfo.InvariantCulture;
            this.DateTimeStyles = DateTimeStyles.None;
            this.DateTimeFormat = Constants.AppDateTimeFormat.DEFAULT_DATE_FORMAT;
            Read = true; Write = true;
        }

        public DefaultDateTimeConverter(string dateFormatsStr) : this()
        {
            var split = dateFormatsStr?.Split('\t');
            if (split?.Length > 0)
                this.DateFormat = split;
        }

        public DefaultDateTimeConverter(string dateFormatsStr, bool read, bool write) : this(dateFormatsStr)
        {
            Read = read; Write = write;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!Write || this.DateFormat == null)
            {
                base.WriteJson(writer, value, serializer);
                return;
            }
            var dt = value as DateTime?;
            var df = this.DateFormat.First();
            writer.WriteValue(dt?.ToString(df));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            DateTime? dateTime = null;
            if (!Read || this.DateFormat == null)
                dateTime = base.ReadJson(reader, objectType, existingValue, serializer) as DateTime?;
            else try
                {
                    var dateStr = reader.Value as string;
                    DateTime dt;
                    if (dateStr.TryConvertToDateTime(dateFormats: this.DateFormat, out dt))
                        dateTime = dt;
                    if (dateTime != null)
                        dateTime = dateTime?.ToUtc();
                }
                catch (Exception) { }
            return dateTime;

        }
    }
}
