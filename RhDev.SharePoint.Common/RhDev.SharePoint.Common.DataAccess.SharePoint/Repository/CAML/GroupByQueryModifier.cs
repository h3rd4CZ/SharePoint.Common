using System;
using System.Text;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository.CAML
{
    public class GroupByQueryModifier : IQueryModifier
    {
        public string GroupByFieldName { get; private set; }

        private string groupByCollapse = "FALSE";

        public GroupByQueryModifier(string groupByFieldName)
        {
            GroupByFieldName = groupByFieldName;
        }

        public GroupByQueryModifier(string groupByFieldName, bool collapse)
        {
            GroupByFieldName = groupByFieldName;
            groupByCollapse = collapse ? "TRUE" : "FALSE";
        }

        public void ModifyQuery(StringBuilder queryStringBuilder)
        {
            queryStringBuilder.AppendFormat("<GroupBy Collapse='{1}' GroupLimit='30'><FieldRef Name='{0}' /></GroupBy>",
                                            GroupByFieldName, groupByCollapse);
        }
    }
}