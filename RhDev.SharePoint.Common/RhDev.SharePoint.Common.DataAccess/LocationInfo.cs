using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhDev.SharePoint.Common.DataAccess
{
    public class LocationInfo
    {
        public string RelativeUrl { get; private set; }

        public string ItemTitle { get; private set; }

        public LocationInfo(string relativeUrl, string itemTitle)
        {
            ItemTitle = itemTitle;
            RelativeUrl = relativeUrl;
        }
    }
}
