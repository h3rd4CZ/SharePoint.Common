using System.Text;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository.CAML
{
    public class CAMLCompositeAndFilter : CAMLCompositeBinaryFilterBase
    {
        protected override string BuildExpression()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<And>");

            foreach (CAMLFilter filter in Filters)
                builder.Append(filter.FilterExpression);

            builder.Append("</And>");
            return builder.ToString();
        }
    }
}
