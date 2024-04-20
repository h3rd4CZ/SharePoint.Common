using System;
using System.Collections.Generic;
using System.Linq;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Security;
using Microsoft.SharePoint;
using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Installation
{
    public abstract class FeatureInstallation<TScope> : IService
    {
        public SPFeatureReceiverProperties FeatureProperties { get; set; }

        public PermissionSetDefinition[] RequiredPermissions { get; set; }

        protected void CheckRequiredSiteFeature(SPSite site, Guid featureId)
        {
            if (site.Features.All(f => f.DefinitionId != featureId))
                throw new SiteFeatureRequiredException(featureId);
        }

        public void ExecuteInstallation(TScope scope)
        {
            Console.WriteLine("- Installing {0}", GetType().Name);
            DoExecuteInstallation(scope);
        }

        [Obsolete("Vytvořte novou třídu a implementujte rozhraní IFeatureUpgradeAction a upgrade spusťte pomocí třídy FeatureUpgradeRunneru.")]
        public void ExecuteUpgrade(TScope scope, string upgradeActionName, IDictionary<string, string> parameters)
        {
            Console.WriteLine("- Upgrading {0}", GetType().Name);
            DoExecuteUpgrade(scope, upgradeActionName, parameters);
        }

        public void ExecuteUninstallation(TScope scope)
        {
            Console.WriteLine("- Uninstalling {0}", GetType().Name);
            DoExecuteUninstallation(scope);
        }

        protected abstract void DoExecuteInstallation(TScope scope);

        [Obsolete("Vytvořte novou třídu a implementujte rozhraní IFeatureUpgradeAction a upgrade spusťte pomocí třídy FeatureUpgradeRunneru.")]
        protected virtual void DoExecuteUpgrade(TScope scope, string upgradeActionName, IDictionary<string, string> parameters)
        {

        }

        protected virtual void DoExecuteUninstallation(TScope scope)
        {
        }

        protected static void ActivateDependentFeature(SPWeb web, Guid featureId)
        {
            var feature = web.Features.SingleOrDefault(f => f.DefinitionId == featureId);

            if (feature != null)
                return;

            Console.WriteLine("- Activating dependent web feature {0} on {1}", featureId, web.Url);
            web.Features.Add(featureId);
        }

        protected static void ActivateDependentFeature(SPSite site, Guid featureId)
        {
            var feature = site.Features.SingleOrDefault(f => f.DefinitionId == featureId);

            // ve skriptu se aktivuji feature vcetne contentTypes a TaxonomyFields na kazdem webu
            // pokud je jiz na SiteCollection aktivovana feature s napr. ContentType, tak tuto aktivaci preskocime
            if (feature != null)
                return;

            Console.WriteLine("- Activating dependent site feature {0} on {1}", featureId, site.Url);
            site.Features.Add(featureId);
        }

        #region PermissionSets
        protected void SetupPermissionSets(SPWeb web)
        {
            if (RequiredPermissions == null || RequiredPermissions.Length == 0)
                return;

            foreach (PermissionSetDefinition def in RequiredPermissions)
                PermissionSetsUtility.EnsureRoleDefinition(web, def);
        }
        #endregion

        #region GroupSetup

        #endregion
    }
}
