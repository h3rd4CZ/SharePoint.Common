using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhDev.SharePoint.Common.DataAccess.Repository.Entities
{
    public enum ApplicationLogLevel
    {
        Unknown = 0,
        Information = 1,
        Debug = 1 << 1,
        Error = 1 << 2
    }

    public class LogItem : EntityBase
    {
        public string Message { get; set; }
        public ApplicationLogLevel Level { get; set; }
        public string Source { get; set; }
    }
}
