using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhDev.SharePoint.Common
{
    public class ContentTypeFormsUrl
    {
        public string NewFormUrl { get; set; }

        public string EditFormUrl { get; set; }

        public string DisplayFormUrl { get; set; }

        public ContentTypeFormsUrl()
        {

        }

        public ContentTypeFormsUrl(string formsUrl)
        {
            NewFormUrl = formsUrl;
            EditFormUrl = formsUrl;
            DisplayFormUrl = formsUrl;
        }

        public ContentTypeFormsUrl(string newFormUrl = null, string displayFormUrl = null, string editFormUrl = null)
        {
            NewFormUrl = newFormUrl;
            DisplayFormUrl = displayFormUrl;
            EditFormUrl = editFormUrl;
        }
    }

}
