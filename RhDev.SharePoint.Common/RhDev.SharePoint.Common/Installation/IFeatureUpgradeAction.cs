using RhDev.SharePoint.Common.Caching.Composition;
using System;

namespace RhDev.SharePoint.Common.Installation
{
    public interface IFeatureUpgradeAction<in TScope> : IAutoRegisteredService
    {
        string FeaturePart { get; }

        Version TargetVersion { get; }

        void ExecuteUpgrade(TScope scope);
    }
}
