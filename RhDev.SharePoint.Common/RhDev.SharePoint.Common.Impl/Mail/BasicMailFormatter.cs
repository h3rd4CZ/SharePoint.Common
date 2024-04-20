using RhDev.SharePoint.Common.Mail;
using System.Collections.Generic;
using System.Text;

namespace RhDev.SharePoint.Common.Impl.Mail
{
    public class BasicMailFormatter : IMailFormatter
    {
        private const string TOKEN_FORMAT = "[[{0}]]";

        public string Format(string template, object input)
        {
            var bld = new StringBuilder(template);

            ReplaceTokens(input, bld);
            return bld.ToString();
        }

        private static void ReplaceTokens(object input, StringBuilder bld)
        {
            IDictionary<string, string> replacements = input as IDictionary<string, string>;

            if (replacements == null)
                return;

            foreach (var replacement in replacements)
                bld.Replace(string.Format(TOKEN_FORMAT, replacement.Key), replacement.Value);
        }
    }
}
