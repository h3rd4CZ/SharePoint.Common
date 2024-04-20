using RhDev.SharePoint.Common.Caching.Composition;
using System;

namespace RhDev.SharePoint.Common.Configuration
{
    public interface IConfigurationManager<T> : IAutoRegisteredService where T : ConfigurationObject
    {
        P GetConfiguration<P>(Func<T, P> propertySelector);

        P GetConfiguration<P>(Func<T, P> propertySelector, string fallbackWebUrl);
    }
}
