﻿using RhDev.SharePoint.Common.Caching.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Configuration
{
    public interface IConfigurationCacheStrategy : IService
    {
        bool UsingCache { get; }
    }
}
