using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using RhDev.SharePoint.Common;
using RhDev.SharePoint.Common.Extensions;

namespace RhDev.SharePoint.Common.Configuration
{
    public class ConfigurationValue : EntityBase
    {
        public ConfigurationKey Key { get; private set; }

        public object AsObject { get; private set; }

        public string AsString => Convert.ToString(AsObject, CultureInfo.InvariantCulture);

        public int AsInteger => int.Parse(AsString, CultureInfo.InvariantCulture);

        public double? AsDoubleSafe
        {
            get
            {
                double res;
                if (double.TryParse(AsString, out res)) return res;
                return null;
            }
        }

        public int? AsIntegerSafe
        {
            get
            {
                int res;
                if (int.TryParse(AsString, out res)) return res;
                return null;
            }
        }

        public int? AsValidMonthDayIntegerSafe
        {
            get
            {
                int res;
                if (int.TryParse(AsString, out res)) return res <= 31 && res > 0 ? res : 1;
                return null;
            }
        }

        public bool AsBoolean => bool.Parse(AsString);

        public bool? AsBooleanNullable => string.IsNullOrEmpty(AsString) ? null : (bool?)bool.Parse(AsString);

        public TimeSpan AsTimeSpan => TimeSpan.Parse(AsString);

        public TEnum AsEnum<TEnum>() where TEnum : struct 
        {
            return (TEnum) Enum.Parse(typeof (TEnum), AsString, true);
        }

        public TEnum AsEnumDescription<TEnum>() where TEnum : struct, IConvertible
        {
            var s = AsString;

            if (string.IsNullOrEmpty(s)) return default(TEnum);

            return s.GetEnumType<TEnum>();
        }

        public ConfigurationValue(ConfigurationKey key, object value)
        {
            Key = key;
            AsObject = value;
        }

        public override string ToString()
        {
            return $"Key = {Key}, Value = {AsObject}";
        }
    }
}
