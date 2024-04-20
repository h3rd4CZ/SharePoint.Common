namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository.CAML
{
    /// <summary>
    /// Class that holds filter expressions that can be used by the <see cref="CAMLQueryBuilder"/>. 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CAML")]
    public class CAMLFilter
    {
        /// <summary>
        /// The filter expression to use when building a query. 
        /// </summary>
        public virtual string FilterExpression { get; set; }

        public override string ToString()
        {
            return FilterExpression;
        }

        public static CAMLFilter operator &(CAMLFilter first, CAMLFilter second)
        {
            /* Umožňuje použít operátor & pro kombinaci s null, např. (null) & filter => filter,
             * což se hodí pro podmíněnou kombinaci filtrů v if-else */
            if (first == null)
                return second;
            
            var andFilter = new CAMLCompositeAndFilter();
            andFilter.AddFilter(first);
            andFilter.AddFilter(second);

            return andFilter;
        }
        
        public static CAMLFilter operator |(CAMLFilter first, CAMLFilter second)
        {
            var orFilter = new CAMLCompositeOrFilter();
            orFilter.AddFilter(first);
            orFilter.AddFilter(second);

            return orFilter;
        }
    }
}
