using Microsoft.SharePoint.Taxonomy;
using RhDev.SharePoint.Common.Caching;
using RhDev.SharePoint.Common.Taxonomy;
using RhDev.SharePoint.Common.Utils;
using System;
using System.Linq;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Taxonomy
{
    public class TermProvider : ITermProvider
    {
        private readonly ICache<TermSet> termSetCache;

        public TermProvider(ICache<TermSet> termSetCache)
        {
            this.termSetCache = termSetCache;
        }
                
        public TermValue GetTerm(TermSetIdentificator termSetIdentificator, string termPath)
        {
            TermSet termSet = GetTermSet(termSetIdentificator);
            return BuildTerm(GetTermByPath(termSet, termPath));
        }

        public TermValue GetTermByGuid(TermSetIdentificator termSetIdentificator, string termGuid)
        {
            TermSet termSet = GetTermSet(termSetIdentificator);
            var term = termSet.GetTerm(new Guid(termGuid));
            Guard.NotNull(term, nameof(term), $"Term with ID : {termGuid} was not found");
            return BuildTerm(term);
        }

        public Guid GetTermStoreId(SectionDesignation sectionDesignation, string storeName)
        {
            Guard.NotNull(sectionDesignation, nameof(sectionDesignation));
            Guard.StringNotNullOrWhiteSpace(storeName, nameof(storeName));

            var store = TaxonomyProvider.GetStoreByName(sectionDesignation, storeName);

            return store.Id;
        }

        public Guid GetTermSetId(SectionDesignation sectionDesignation, string storeName, string groupName, string termSetName, out Guid termStoreId)
        {
            Guard.NotNull(sectionDesignation, nameof(sectionDesignation));
            Guard.StringNotNullOrWhiteSpace(groupName, nameof(groupName));
            Guard.StringNotNullOrWhiteSpace(termSetName, nameof(termSetName));

            var store = TaxonomyProvider.GetStoreByName(sectionDesignation, storeName);

            termStoreId = store.Id;

            var tg = store.Groups[groupName];

            Guard.NotNull(tg, nameof(tg), $"Termset group was not found for name : {groupName}");

            var ts = tg.TermSets[termSetName];

            Guard.NotNull(ts, nameof(ts), $"Termset was not found for name : {termSetName}");

            return ts.Id;
        }

        public bool IsValidTerm(TermSetIdentificator termSetIdentificator, string termPath)
        {
            TermSet termSet = GetCachedTermSet(termSetIdentificator);
            return IsValidTerm(termSet, termPath);
        }
        
        private TermSet GetTermSet(TermSetIdentificator termSetIdentificator)
        {
            Guard.NotNull(termSetIdentificator, nameof(termSetIdentificator));

            return GetCachedTermSet(termSetIdentificator);
        }

        private Term GetTermByName(string termName, TermCollection terms)
        {
            foreach (Term term in terms)
                if (term.Name.Equals(termName, StringComparison.Ordinal))
                    return term;

            throw new InvalidOperationException($"Term was not found for term name : {termName}");
        }
                
        private TermValue BuildTerm(Term term) => 
            new TermValue(term.Id, term.GetPath(), term.Name, term.Labels.Select(l => new TermValueLabel(l.Value, l.Language, l.IsDefaultForLanguage)).ToList());

        private TermSet GetCachedTermSet(TermSetIdentificator termSetIdentificator)
        {
            var key = 
                CacheKey
                .ForValuesWhere("Site", termSetIdentificator.SessionSite.Address)
                .CombineWith(
                    CacheKey
                    .ForValuesWhere("TermSetId", termSetIdentificator.TermSetId.ToString()));

            return termSetCache.GetOrFetchValue(key, () => TaxonomyProvider.GetTermSet(termSetIdentificator.SessionSite, termSetIdentificator.StoreId, termSetIdentificator.TermSetId));
        }

        private Term GetTermByPath(TermSet termSet, string termPath)
        {
            string[] pathElements = TermPath.GetPathElements(termPath);

            int depth = 0;
            Term term = GetTermByName(pathElements[depth], termSet.Terms);
            Guard.NotNull(term, nameof(term));

            while (depth < pathElements.Length - 1)
            {
                term = GetTermByName(pathElements[++depth], term.Terms);
            }

            return term;
        }
                        
        private bool IsValidTerm(TermSet termSet, string termPath)
        {
            return GetTermByPath(termSet, termPath) != null;
        }
    }
}
