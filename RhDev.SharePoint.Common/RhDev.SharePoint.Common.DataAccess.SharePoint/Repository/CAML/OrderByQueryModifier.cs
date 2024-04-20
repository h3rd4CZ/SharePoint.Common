using System;
using System.Text;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository.CAML
{
    public class OrderByQueryModifier : IQueryModifier
    {
        public Guid OrderByFieldId { get; private set; }

        public OrderByQueryModifier(Guid orderByFieldId)
        {
            OrderByFieldId = orderByFieldId;
        }

        public void ModifyQuery(StringBuilder queryStringBuilder)
        {
            queryStringBuilder.AppendFormat("<OrderBy><FieldRef ID=\"{0}\" Ascending=\"True\" /></OrderBy>",
                                            OrderByFieldId);
        }
    }
}