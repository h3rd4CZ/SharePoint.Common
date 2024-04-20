using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Utilities;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Resources
{
    public class Resx
    {
        public const string RESX = "RhDev.SharePoint.Common";

        /// <summary>
        /// Lock resx.
        /// </summary>
        static readonly object lockresx = new object();

        /// <summary>
        /// Get resource value responding to key in a user ui culture.
        /// </summary>
        /// <param name="key">Resources key.</param>
        /// <example>
        /// <code>
        /// var resx = Resx.GetValue("KeyNameFromResourceFile");
        /// </code>
        /// </example>
        /// <remarks>Your remarks.</remarks>
        /// <returns>Value frmo the resources.</returns>
        public static string GetValue(string key)
        {
            return GetValue(key, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Get resource value responding to key in a specific culture.
        /// </summary>
        /// <param name="key">Resources key.</param>
        /// <param name="lang">Prefer language of the result.</param>
        /// <example>
        /// <code>
        /// var resx = Resx.GetValue("KeyNameFromResourceFile", new CultureInfo((int)lcid));
        /// </code>
        /// </example>
        /// <returns>Value from the resources in specified culture.</returns>
        public static string GetValue(string key, CultureInfo lang)
        {
            return GetValue(key, lang.LCID);
        }

        /// <summary>
        /// Get resource value responding to key in a specific culture.
        /// </summary>
        /// <param name="key">Resources key.</param>
        /// <param name="lcid">Prefer language of the result.</param>
        /// <example>
        /// <code>
        /// var resx = Resx.GetValue("KeyNameFromResourceFile", (int)lcid);
        /// </code>
        /// </example>
        /// <returns>Value from the resources in specified culture.</returns>
        public static string GetValue(string key, int lcid)
        {
            return GetValue(key, lcid, RESX);
        }

        /// <summary>
        /// Get resource value coresponding to key from resx file.
        /// </summary>
        /// <param name="key">Resources key.</param>
        /// <param name="resxFile">Resources file.</param>
        /// <example>
        /// <code>
        /// var resx = Resx.GetValue("KeyNameFromResourceFile", "ResourceFileName");
        /// </code>
        /// </example>
        /// <returns>Value from the resources.</returns>
        public static string GetValue(string key, string resxFile)
        {
            return GetValue(key, CultureInfo.CurrentUICulture.LCID, resxFile);
        }

        /// <summary>
        /// Get resource value responding to key in a specific culture.
        /// </summary>
        /// <param name="key">Resources key.</param>
        /// <param name="lcid">Prefer language of the result.</param>
        /// <param name="resxFile">Resources file.</param>
        /// <example>
        /// <code>
        /// var resx = Resx.GetValue("KeyNameFromResourceFile", (int)lcid, "ResourceFileName");
        /// </code>
        /// </example>
        /// <returns>Value from the resources.</returns>
        public static string GetValue(string key, int lcid, string resxFile)
        {
            if (string.IsNullOrEmpty(resxFile))
                resxFile = RESX;

            lock (lockresx)
            {
                string resx = string.Empty;
                int index = 0;
                while (true)
                {
                    try
                    {
                        resx = SPUtility.GetLocalizedString("$Resources:" + key, resxFile, (uint)lcid);
                    }
                    catch { }

                    if (index >= 3 || !string.IsNullOrEmpty(resx))
                        break;

                    index++;
                }
                return resx;
            }
        }

    }
}
