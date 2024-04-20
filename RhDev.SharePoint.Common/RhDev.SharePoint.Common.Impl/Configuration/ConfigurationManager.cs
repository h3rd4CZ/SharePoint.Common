using System;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.Security;

namespace RhDev.SharePoint.Common.Impl.Configuration
{
    public class ConfigurationManager<T> : IConfigurationManager<T> where T : ConfigurationObject
    {
        private readonly ISecurityContext _securityContext;
        private readonly IObjectConfigurationFactory _objectConfigurationfactory;

        public ConfigurationManager(
            ISecurityContext securityContext,
            IObjectConfigurationFactory objectConfigurationfactory)
        {
            _securityContext = securityContext;
            _objectConfigurationfactory = objectConfigurationfactory;
        }

        /// <summary>
        /// Returns configuration value given by lambda with current context web as web source
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="propertySelector"></param>
        /// <returns></returns>
        public P GetConfiguration<P>(Func<T, P> propertySelector)
        {
            return GetConfiguration(propertySelector, string.Empty);
        }

        /// <summary>
        /// Returns configuration value given by lambda with current context web as web source and optional with fallback web as source web for instance timer job running context
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="selector"></param>
        /// <param name="fallbackWebUrl"></param>
        /// <returns></returns>
        public P GetConfiguration<P>(Func<T, P> selector, string fallbackWebUrl = null)
        {
            if (!_securityContext.IsFrontend && string.IsNullOrWhiteSpace(fallbackWebUrl))
                throw new InvalidOperationException("When securitycontext is backend or is not present spcontext fallback weburl parameter must be specified");

            var url = !_securityContext.IsFrontend ? fallbackWebUrl : _securityContext.CurrentWebUrl;

            var configObject = _objectConfigurationfactory.GetConfigurationObject<T>(url);

            return selector(configObject);
        }
    }
}
