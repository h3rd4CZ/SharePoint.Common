using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using NSubstitute.Routing.Handlers;
using RhDev.SharePoint.Common.Utils;
using System;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Taxonomy
{
    public class TaxonomyProvider
    {
        public static TermStore GetStoreByName(SectionDesignation sectionDesignation, string storeName)
        {
            Guard.NotNull(sectionDesignation, nameof(sectionDesignation));
                        
            TermStore termStore = default;

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var site = new SPSite(sectionDesignation.Address))
                {
                    var taxonomySession = new TaxonomySession(site);

                    termStore = string.IsNullOrWhiteSpace(storeName)
                    ? taxonomySession.TermStores.Count > 0 ? taxonomySession.TermStores[0] : null
                    : taxonomySession.TermStores[storeName];
                }
            });

            Guard.NotNull(termStore, nameof(termStore));

            return termStore;
        }
        public static TermSet GetTermSet(SectionDesignation siteDesignation, Guid storeId, Guid termSetId)
        {
            Guard.NotNull(siteDesignation, nameof(siteDesignation));
            Guard.NotNull(storeId, nameof(storeId));
            Guard.NotNull(termSetId, nameof(termSetId));

            TermSet termSet = null;

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var site = new SPSite(siteDesignation.Address))
                {
                    var taxonomySession = new TaxonomySession(site);

                    var termStore = taxonomySession.TermStores[storeId];

                    Guard.NotNull(termStore, nameof(termStore));

                    termSet = termStore.GetTermSet(termSetId);

                    Guard.NotNull(termSet, nameof(termSet));
                }
            });

            return termSet;
        }
    }
}
