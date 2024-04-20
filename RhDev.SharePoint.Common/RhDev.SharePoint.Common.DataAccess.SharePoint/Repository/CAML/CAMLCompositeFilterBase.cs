using System;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository.CAML
{
    public abstract class CAMLCompositeFilterBase : CAMLFilter
    {
        protected CAMLCompositeFilterBase()
        {
            Filters = new List<CAMLFilter>();
        }

        protected IList<CAMLFilter> Filters { get; private set; }

        public override string FilterExpression
        {
            get { return BuildExpression(); }
            set { throw new NotSupportedException(); }
        }

        protected abstract string BuildExpression();

        /// <summary>
        /// Add a filter expression to the CAML Query builder
        /// </summary>
        /// <param name="filter">The filter expression to add. </param>
        public virtual void AddFilter(CAMLFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            Filters.Add(filter);
        }
    }
}