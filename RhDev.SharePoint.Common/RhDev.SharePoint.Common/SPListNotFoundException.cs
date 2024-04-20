using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhDev.SharePoint.Common
{
    public class SPListNotFoundException : Exception
    {
        public SPListNotFoundException(string message)
            : base(message)
        {
        }
    }
}
