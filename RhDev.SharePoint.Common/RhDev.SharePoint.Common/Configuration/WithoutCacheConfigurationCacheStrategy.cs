using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Configuration
{
    public class WithoutCacheConfigurationCacheStrategy : IConfigurationCacheStrategy
    {
        public bool UsingCache => false;
    }
}
