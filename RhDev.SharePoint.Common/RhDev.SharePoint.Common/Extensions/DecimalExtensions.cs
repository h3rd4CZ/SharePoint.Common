using System;
using System.Globalization;

namespace RhDev.SharePoint.Common.Extensions
{
    public static class DecimalExtensions
    {
        public static string GetCZCurrencyValue(this Decimal d)
        {
            return string.Format(new CultureInfo(1029), "{0:c}", d);
        }
    }
}
