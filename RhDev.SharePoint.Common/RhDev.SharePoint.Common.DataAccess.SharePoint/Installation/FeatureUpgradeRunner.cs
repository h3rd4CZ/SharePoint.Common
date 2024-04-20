using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Installation;
using RhDev.SharePoint.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Installation
{
    public class FeatureUpgradeRunner<TScope> : IService where TScope : class
    {
        public const string FeaturePartParameterKey = "FeaturePart";
        public const string TargetVersionParameterKey = "TargetVersion";

        private readonly IFeatureUpgradeAction<TScope>[] featureUpgrades;

        public FeatureUpgradeRunner(IFeatureUpgradeAction<TScope>[] featureUpgrades, ITraceLogger traceLogger)
        {
            this.featureUpgrades = featureUpgrades;
        }

        public void ExecuteUpgrade(TScope scope, IDictionary<string, string> parameters)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");

            if (parameters == null)
                throw new ArgumentNullException("parameters");

            string featurePart = GetFeaturePart(parameters);
            Version targetVersion = GetTargetVersion(parameters);

            IFeatureUpgradeAction<TScope> upgradeAction = FindUpgradeAction(featurePart, targetVersion);

            Console.WriteLine("- Executing upgrade action for feature part {0} for target version {1}", featurePart,
                targetVersion);

            try
            {
                upgradeAction.ExecuteUpgrade(scope);
            }
            catch (Exception ex)
            {
                WriteUpgradeActionFailedMessage(ex);
                throw;
            }

            Console.WriteLine("- Completed upgrade action");
        }

        private void WriteUpgradeActionFailedMessage(Exception ex)
        {
            string message = String.Format("Upgrade action FAILED because: {0}", ex.Message)
                .Replace('{', ' ')
                .Replace('}', ' ');

            Console.WriteLine(message);
            Console.WriteLine($"Upgrade action FAILED : {ex}");
        }

        private static string GetFeaturePart(IDictionary<string, string> parameters)
        {
            return GetRequiredParameter(parameters, FeaturePartParameterKey);
        }

        private static Version GetTargetVersion(IDictionary<string, string> parameters)
        {
            return new Version(GetRequiredParameter(parameters, TargetVersionParameterKey));
        }

        private static string GetRequiredParameter(IDictionary<string, string> parameters, string parameterKey)
        {
            if (!parameters.ContainsKey(parameterKey))
                throw new ArgumentException(
                    string.Format(
                        "Custom upgrade action parameter with name \"{0}\" not found. The parameter must be present in the Feature manifest inside CustomUpgradeAction/Parameters element.",
                        parameterKey));

            return parameters[parameterKey];
        }

        private IFeatureUpgradeAction<TScope> FindUpgradeAction(string featurePart, Version targetVersion)
        {
            var upgradeActions =
                featureUpgrades.Where(
                    upgrade =>
                        upgrade.FeaturePart.Equals(featurePart, StringComparison.Ordinal) && upgrade.TargetVersion == targetVersion)
                    .ToList();

            if (upgradeActions.Count == 0)
                throw new ArgumentException(
                    String.Format("Upgrade action for feature part \"{0}\" and target version \"{1}\" not found.",
                        featurePart, targetVersion));

            if (upgradeActions.Count > 1)
                throw new ArgumentException(
                    String.Format("Multiple upgrade actions for feature part \"{0}\" and target version \"{1}\" found.",
                        featurePart, targetVersion));

            return upgradeActions.First();
        }
    }
}
