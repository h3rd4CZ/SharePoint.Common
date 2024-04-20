using System;
using System.Linq;

namespace RhDev.SharePoint.Common.Extensions
{
    public static class StringExtensions
    {
        public static string TrimLoginDomain(this string login)
        {
            if (string.IsNullOrEmpty(login)) throw new InvalidOperationException("String is null or empty");

            string[] data = login.Split('\\');
            if (data.Count() > 1) return data[1];
            return login;
        }
    }
}
