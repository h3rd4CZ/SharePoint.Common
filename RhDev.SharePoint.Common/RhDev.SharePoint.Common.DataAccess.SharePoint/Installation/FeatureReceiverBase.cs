using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.BusinessData.Administration;
using RhDev.SharePoint.Common.Composition;
using RhDev.SharePoint.Common.Utils;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Installation
{
    public abstract class FeatureReceiverBase<TScope, TInstallation> : SPFeatureReceiver where TInstallation : FeatureInstallation<TScope> where TScope : class
    {
        private readonly Action<object> componentBuilder;

        public TInstallation Installation { get; set; }

        public FeatureUpgradeRunner<TScope> UpgradeRunner { get; set; }

        protected FeatureReceiverBase(Action<object> componentBuilder)
        {
            Guard.NotNull(componentBuilder, nameof(componentBuilder));
            this.componentBuilder = componentBuilder;
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            componentBuilder(this);

            Installation.FeatureProperties = properties;

            var scope = (TScope)properties.Feature.Parent;
            Installation.ExecuteInstallation(scope);
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            componentBuilder(this);

            Installation.FeatureProperties = properties;

            var scope = (TScope)properties.Feature.Parent;
            Installation.ExecuteUninstallation(scope);
        }

        public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, IDictionary<string, string> parameters)
        {
            componentBuilder(this);

            var scope = (TScope)properties.Feature.Parent;
            UpgradeRunner.ExecuteUpgrade(scope, parameters);
        }
    }
}