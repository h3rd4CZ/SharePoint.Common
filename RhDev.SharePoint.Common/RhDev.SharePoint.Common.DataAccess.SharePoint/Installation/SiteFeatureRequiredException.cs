using System;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Installation
{
    public class SiteFeatureRequiredException : Exception
    {
        public Guid FeatureId { get; private set; }

        public SiteFeatureRequiredException(Guid featureId)
            : base(
            string.Format("Site collection feature '{0}' required to be activated", featureId))
        {
            FeatureId = featureId;
        }
    }
}
