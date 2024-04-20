using RhDev.SharePoint.Common.DataAccess.SharePoint.Repository.CAML;
using System;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint
{
    public abstract class CAMLCompositeBinaryFilterBase : CAMLCompositeFilterBase
    {
        public override void AddFilter(CAMLFilter filter)
        {
            if (Filters.Count > 1)
                throw new NotSupportedException("Only only two disjuncts are allowed");

            base.AddFilter(filter);
        }
    }
}