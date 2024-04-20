using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Logging
{
    public enum EventSeverity
    {
        None = 0,
        ErrorCritical = 30,
        Error = 40,
        Warning = 50,
        Information = 80,
        Verbose = 100
    }
}
