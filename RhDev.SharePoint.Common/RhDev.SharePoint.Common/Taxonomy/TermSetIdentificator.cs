using RhDev.SharePoint.Common.Utils;
using System;

namespace RhDev.SharePoint.Common.Taxonomy
{
    public class TermSetIdentificator
    {
        public Guid StoreId { get; }
        public Guid TermSetId { get; }
        public SectionDesignation SessionSite { get; }

        private TermSetIdentificator(SectionDesignation sessionSite, Guid storeId, Guid termSetId) 
        {
            Guard.NotDefault(storeId, nameof(storeId));
            Guard.NotDefault(termSetId, nameof(termSetId));
            Guard.NotNull(sessionSite, nameof(sessionSite));

            this.SessionSite = sessionSite;
            this.StoreId = storeId;
            this.TermSetId = termSetId;
        }

        public static TermSetIdentificator Create(SectionDesignation sessionSite, Guid storeId, Guid termSetId) =>
            new TermSetIdentificator(sessionSite, storeId, termSetId);
    }
}
