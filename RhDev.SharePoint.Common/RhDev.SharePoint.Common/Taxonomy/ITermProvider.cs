using RhDev.SharePoint.Common.Caching.Composition;
using System;

namespace RhDev.SharePoint.Common.Taxonomy
{
    public interface ITermProvider : IAutoRegisteredService
    {
        TermValue GetTerm(TermSetIdentificator termSetIdentificator, string termPath);
        TermValue GetTermByGuid(TermSetIdentificator termSetIdentificator, string termGuid);
        bool IsValidTerm(TermSetIdentificator termSetIdentificator, string termPath);
        Guid GetTermStoreId(SectionDesignation sectionDesignation, string storeName);
        Guid GetTermSetId(SectionDesignation sectionDesignation, string storeName, string groupName, string termSetName, out Guid termStoreId);
    }
}
