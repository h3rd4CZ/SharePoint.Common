using System.Text;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository.CAML
{
    public class CAMLCompositeOrFilter : CAMLCompositeBinaryFilterBase
    {
        protected override string BuildExpression()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<Or>");

            foreach (CAMLFilter filter in Filters)
                builder.Append(filter.FilterExpression);

            builder.Append("</Or>");
            return builder.ToString();
        }
    }
}