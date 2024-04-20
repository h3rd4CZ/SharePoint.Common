using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.SharePoint.Utilities;
using System.Text;
using RhDev.SharePoint.Common.Utils;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository.CAML
{
    public static class CAMLFilters
    {
        // SP nepodporuje uložení hodnoty DateTime.MinValue
        private static readonly DateTime OldestPossibleDate = new DateTime(1970, 1, 1);

        public static DateTime AdjustDateForQuery(DateTime date)
        {
            return date < OldestPossibleDate ? OldestPossibleDate : date;
        }
                
        public static CAMLFilter Equal(string name, object value, CAMLType camlType)
        {
            return new CAMLFilter
                       {
                           FilterExpression = String.Format(CultureInfo.InvariantCulture,
                                                            "<Eq><FieldRef Name='{0}'/><Value Type='{1}'>{2}</Value></Eq>"
                                                            , name, camlType, value)
                       };
        }

        public static CAMLFilter Equal(string name, DateTime value)
        {
            string valueStr = SPUtility.CreateISO8601DateTimeFromSystemDateTime(value);

            return new CAMLFilter
                       {
                           FilterExpression = String.Format(CultureInfo.InvariantCulture,
                                                            "<Eq><FieldRef Name='{0}'/><Value Type='{1}'>{2}</Value></Eq>"
                                                            , name, CAMLType.DateTime, valueStr)
                       };
        }

        public static CAMLFilter ContentTypeEqual(string contentTypeName)
        {
            return new CAMLFilter
                       {
                           FilterExpression = String.Format(CultureInfo.InvariantCulture,
                                                            "<Eq><FieldRef Name='ContentType'/><Value Type='Text'>{0}</Value></Eq>",
                                                            contentTypeName)
                       };
        }

        public static CAMLFilter Equal(Guid fieldId, object value, CAMLType camlType)
        {
            return new CAMLFilter
                       {
                           FilterExpression = String.Format(CultureInfo.InvariantCulture,
                                                            "<Eq><FieldRef ID='{0}'/><Value Type='{1}'>{2}</Value></Eq>"
                                                            , fieldId.ToString(), camlType, value)
                       };
        }

        public static CAMLFilter NotEqual(string name, object value, CAMLType camlType)
        {
            return new CAMLFilter
                       {
                           FilterExpression = String.Format(CultureInfo.InvariantCulture,
                                                            "<Neq><FieldRef Name='{0}'/><Value Type='{1}'>{2}</Value></Neq>"
                                                            , name, camlType, value)
                       };
        }

        public static CAMLFilter NotEqual(string name, DateTime value)
        {
            string valueStr = SPUtility.CreateISO8601DateTimeFromSystemDateTime(value);

            return new CAMLFilter
                       {
                           FilterExpression = String.Format(CultureInfo.InvariantCulture,
                                                            "<Neq><FieldRef Name='{0}'/><Value Type='{1}'>{2}</Value></Neq>"
                                                            , name, CAMLType.DateTime, valueStr)
                       };
        }

        public static CAMLFilter NotEqual(Guid fieldId, object value, CAMLType camlType)
        {
            return new CAMLFilter
                       {
                           FilterExpression = String.Format(CultureInfo.InvariantCulture,
                                                            "<Neq><FieldRef ID='{0}'/><Value Type='{1}'>{2}</Value></Neq>"
                                                            , fieldId.ToString(), camlType, value)
                       };
        }

        public static CAMLFilter LessOrEqual(Guid fieldId, long value, CAMLType camlType)
        {
            return new CAMLFilter
                       {
                           FilterExpression =
                               String.Format("<Leq><FieldRef ID='{0}'/><Value Type='{1}'>{2}</Value></Leq>", fieldId,
                                             camlType, value.ToString(CultureInfo.InvariantCulture))
                       };
        }

        public static CAMLFilter Less(Guid fieldId, long value, CAMLType camlType)
        {
            return new CAMLFilter
            {
                FilterExpression =
                    $"<Lt><FieldRef ID='{fieldId}'/><Value Type='{camlType}'>{value.ToString(CultureInfo.InvariantCulture)}</Value></Lt>"
            };
        }

        public static CAMLFilter LessOrEqual(Guid fieldId, string value, CAMLType camlType)
        {
            return new CAMLFilter
            {
                FilterExpression =
                    String.Format("<Leq><FieldRef ID='{0}'/><Value Type='{1}'>{2}</Value></Leq>", fieldId,
                                  camlType, value)
            };
        }

        public static CAMLFilter LessOrEqual(Guid fieldId, DateTime value, bool includeTime)
        {
            return new CAMLFilter
            {
                FilterExpression =
                    String.Format("<Leq><FieldRef ID='{0}'/><Value Type='{1}' IncludeTimeValue='{2}'>{3}</Value></Leq>", fieldId,
                                  CAMLType.DateTime, GetBooleanString(includeTime), SPUtility.CreateISO8601DateTimeFromSystemDateTime(value))
            };
        }
        
        public static CAMLFilter Less(Guid fieldId, DateTime value, bool includeTime)
        {
            return new CAMLFilter
            {
                FilterExpression =
                    String.Format("<Lt><FieldRef ID='{0}'/><Value Type='{1}' IncludeTimeValue='{2}'>{3}</Value></Lt>", fieldId,
                                  CAMLType.DateTime, GetBooleanString(includeTime), SPUtility.CreateISO8601DateTimeFromSystemDateTime(value))
            };
        }

        private static string GetBooleanString(bool value)
        {
            return value ? true.ToString().ToUpperInvariant() : false.ToString().ToUpperInvariant();
        }

        public static CAMLFilter GreaterOrEqual(Guid fieldId, string value, CAMLType camlType)
        {
            return new CAMLFilter
                       {
                           FilterExpression =
                               String.Format("<Geq><FieldRef ID='{0}'/><Value Type='{1}'>{2}</Value></Geq>", fieldId,
                                             camlType, value)
                       };
        }

        public static CAMLFilter GreaterOrEqual(Guid fieldId, long value, CAMLType camlType)
        {
            return new CAMLFilter
            {
                FilterExpression =
                    String.Format("<Geq><FieldRef ID='{0}'/><Value Type='{1}'>{2}</Value></Geq>", fieldId,
                                  camlType, value.ToString(CultureInfo.InvariantCulture))
            };
        }

        public static CAMLFilter Greater(Guid fieldId, string value, CAMLType camlType)
        {
            return new CAMLFilter
            {
                FilterExpression =
                    String.Format("<Gt><FieldRef ID='{0}'/><Value Type='{1}'>{2}</Value></Gt>", fieldId,
                                  camlType, value)
            };
        }

        public static CAMLFilter Greater(Guid fieldId, DateTime value, bool includeTime)
        {
            return new CAMLFilter
            {
                FilterExpression =
                    String.Format("<Gt><FieldRef ID='{0}'/><Value Type='{1}' IncludeTimeValue='{2}'>{3}</Value></Gt>", fieldId,
                                  CAMLType.DateTime, GetBooleanString(includeTime), SPUtility.CreateISO8601DateTimeFromSystemDateTime(value))
            };
        } 

        public static CAMLFilter GreaterOrEqual(Guid fieldId, DateTime value, bool includeTime)
        {
            return new CAMLFilter
            {
                FilterExpression =
                    String.Format("<Geq><FieldRef ID='{0}'/><Value Type='{1}' IncludeTimeValue='{2}'>{3}</Value></Geq>", fieldId,
                                  CAMLType.DateTime, GetBooleanString(includeTime), SPUtility.CreateISO8601DateTimeFromSystemDateTime(value))
            };
        }

        public static CAMLFilter LookupEqual(Guid fieldId, int value)
        {
            return new CAMLFilter
                       {
                           FilterExpression =
                               String.Format(
                                   "<Eq><FieldRef ID='{0}' LookupId='TRUE'/><Value Type='Lookup'>{1}</Value></Eq>",
                                   fieldId, value)
                       };
        }

        public static CAMLFilter LookupEqual(string fieldInternalName, int value)
        {
            return new CAMLFilter
            {
                FilterExpression =
                    String.Format(
                        "<Eq><FieldRef Name='{0}' LookupId='TRUE'/><Value Type='Lookup'>{1}</Value></Eq>",
                        fieldInternalName, value)
            };
        }

        public static CAMLFilter LookupIn(string fieldInternalName, IEnumerable<int> value)
        {
            return new CAMLFilter
            {
                FilterExpression =
                    String.Format(
                        "<In><FieldRef Name='{0}' LookupId='TRUE'/><Values>{1}</Values></In>",
                        fieldInternalName, 
                           string.Join(string.Empty, value.Select(v => string.Format("<Value Type='Lookup'>{0}</Value>", v)).ToArray()) 
                           )
            };
        }

        public static CAMLFilter LookupIn(Guid fieldId, IEnumerable<int> values)
        {
            string joinedValues = String.Join(String.Empty,
                                              values.Select(value => String.Format("<Value Type='Lookup'>{0}</Value>", value)).ToArray());

            return new CAMLFilter
                {
                    FilterExpression =
                        String.Format(
                            "<In><FieldRef ID='{0}' LookupId='TRUE'/><Values>{1}</Values></In>",
                            fieldId, joinedValues)
                };
        }

        public static CAMLFilter In<T>(Guid fieldId, IEnumerable<T> values)
        {
            Guard.NotNull(values, nameof(values));

            CAMLType camlType = default;

            if (typeof(T) == typeof(int)) camlType = CAMLType.Integer;
            else if (typeof(T) == typeof(string)) camlType = CAMLType.Text;
            else throw new InvalidOperationException("Only int and string types are allowed");

            string joinedValues = string.Join(String.Empty,
                                              values.Select(value => $"<Value Type='{camlType}'>{value}</Value>").ToArray());

            return new CAMLFilter
            {
                FilterExpression =
                        String.Format(
                            "<In><FieldRef ID='{0}'/><Values>{1}</Values></In>",
                            fieldId, joinedValues)
            };
        }

        public static CAMLFilter LookupNotEqual(Guid fieldId, int value)
        {
            return new CAMLFilter
            {
                FilterExpression =
                    String.Format(
                        "<Neq><FieldRef ID='{0}' LookupId='TRUE'/><Value Type='Lookup'>{1}</Value></Neq>",
                        fieldId, value)
            };
        }

        public static CAMLFilter IsNull(Guid fieldId)
        {
            return new CAMLFilter
                       {
                           FilterExpression =
                               String.Format("<IsNull><FieldRef ID='{0}'/></IsNull>", fieldId)
                       };
        }

        public static CAMLFilter IsNotNull(Guid fieldId)
        {
            return new CAMLFilter
            {
                FilterExpression =
                    String.Format("<IsNotNull><FieldRef ID='{0}'/></IsNotNull>", fieldId)
            };
        }

        public static CAMLFilter IsNotNull(string fieldName)
        {
            return new CAMLFilter
            {
                FilterExpression =
                    String.Format("<IsNotNull><FieldRef Name='{0}'/></IsNotNull>", fieldName)
            };
        }

        public static CAMLFilter UserEqual(Guid fieldId, int userId)
        {
            return new CAMLFilter
            {
                FilterExpression = String.Format(CultureInfo.InvariantCulture,
                                                 "<Eq><FieldRef ID='{0}' LookupId='TRUE' /><Value Type='User'>{1}</Value></Eq>"
                                                 , fieldId.ToString(), userId)
            };
        }

        public static CAMLFilter CurrentUserEqual(Guid fieldId)
        {
            return new CAMLFilter
            {
                FilterExpression = String.Format(CultureInfo.InvariantCulture,
                                                 "<Eq><FieldRef ID='{0}'/><Value Type='Integer'><UserID /></Value></Eq>"
                                                 , fieldId.ToString())
            };
        }

        public static CAMLFilter IsInCurrentUserGroups(Guid fieldId)
        {
            return new CAMLFilter
            {
                FilterExpression = String.Format(CultureInfo.InvariantCulture,
                                                 "<Membership Type='CurrentUserGroups'><FieldRef ID='{0}'/></Membership>"
                                                 , fieldId.ToString())
            };
        }

        public static CAMLFilter Contains(Guid fieldId, string value) 
        {
            return new CAMLFilter
            {
                FilterExpression = String.Format(CultureInfo.InvariantCulture,
                                                "<Contains><FieldRef ID=\"{0}\"/><Value Type=\"Text\">{1}</Value></Contains>",
                                                fieldId.ToString(), value)
            };
        }
    }
}
