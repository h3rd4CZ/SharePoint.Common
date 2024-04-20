using System;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository
{
    public class Config
    {        
        public class Fields
        {
            //Config list fields
            public static Guid APPCONFIGKEY = new Guid("{E83D8FDC-A950-4350-B46B-B8BBF5C087F0}");
            public static Guid APPCONFIGVAL = new Guid("{60C71F08-B907-40E5-A784-BA4D26503C91}");
            public static Guid APPCONFIGMODULE = new Guid("{5AC64C63-7B68-4259-AE12-B15E3BFE119C}");

            //Log list fields
            public static Guid LOGLIST_MESSAGE = new Guid("{D46B34A7-97F5-47F6-BE65-3834DB76847F}");
            public static Guid LOGLIST_TIME = new Guid("{81FED35D-6DFC-4748-94E2-EFFDCE4BC9CD}");
            public static Guid LOGLIST_LEVEL = new Guid("{17D19FC2-A261-48BF-8F8C-FA3D503E5A44}");
            public static Guid LOGLIST_SOURCE = new Guid("{F39ECE57-3CD5-4817-B683-995AA4D890CC}");

            //DayOff fields
            public static Guid DAYOFFLIST_DATE = new Guid("{51921175-4B7D-4398-B34F-C64DE84746F5}");
            public static Guid DAYOFFLIST_REPEAT = new Guid("{723F65C0-8534-4B13-B9AF-1997B40703B5}");
            public static Guid DAYOFFLIST_LCID = new Guid("{656EF610-C202-47DE-8631-B9B3CC1FE7FC}");
        }

        public static class Features
        {
            public static class Site
            {
                public static Guid ID = new Guid("{e0f3966e-25d5-4427-959d-6f5cf5c19095}");
            }

            public static class Web
            {
                public static Guid ID = new Guid("{d691910d-4c56-49c5-a87c-0c7c3cd1e83e}");
            }
        }

        public static class Lists
        {
            public const string APPCONFIGURL = "/Lists/ApplicationConfiguration/AllItems.aspx";
            public const string LOGURL = "/Lists/ApplicationLog/AllItems.aspx";
            public const string DAYOFFURL = "/Lists/DayOff/AllItems.aspx";

            public static class ContentTypes
            {
                
            }

            public static class ContentTypeDefinitions
            {
                
            }
        }
    }
}
