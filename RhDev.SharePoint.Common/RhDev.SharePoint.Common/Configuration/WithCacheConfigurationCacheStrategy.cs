using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Configuration
{
    public class WithCacheConfigurationCacheStrategy : IConfigurationCacheStrategy
    {
        public bool UsingCache => true;
    }
}
