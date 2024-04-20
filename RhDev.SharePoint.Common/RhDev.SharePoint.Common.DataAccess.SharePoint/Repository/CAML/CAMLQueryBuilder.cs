using System;
using System.Text;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository.CAML
{
    public enum SPQueryScope
    {
        None,
        Recursive,
        RecursiveAll
    };

    public static class CAMLQueryBuilder
    {
        private const string ScopeRecursiveAll = "Scope='RecursiveAll'";
        private const string ScopeRecursive = "Scope='Recursive'";

        public static SPQuery BuildQuery(CAMLFilter filter, SPQueryScope? scope = null, params Guid[] viewFieldIds)
        {
            return BuildQuery(filter, new IQueryModifier[0], scope, viewFieldIds);
        }

        public static SPQuery BuildQuery(CAMLFilter filter, IQueryModifier queryModifier, SPQueryScope? scope = null, params Guid[] viewFieldIds)
        {
            return BuildQuery(filter, queryModifier != null ? new[] {queryModifier} : null, scope, viewFieldIds);
        }

        public static SPQuery BuildQuery(CAMLFilter filter, IQueryModifier[] queryModifiers, SPQueryScope? scope = null, params Guid[] viewFieldIds)
        {
            var spQuery = new SPQuery { Query = BuildExpression(filter, queryModifiers) };

            if (!Equals(null, scope) && !Equals(scope, SPQueryScope.None))
                spQuery.ViewAttributes = scope == SPQueryScope.Recursive ? ScopeRecursive : ScopeRecursiveAll;

            if (viewFieldIds != null && viewFieldIds.Length > 0)
                AddViewFields(spQuery, viewFieldIds);

            return spQuery;
        }
        
        private static void AddViewFields(SPQuery spQuery, string[] viewFieldNames)
        {
            StringBuilder bld = new StringBuilder();

            foreach (string viewFieldName in viewFieldNames)
                bld.AppendFormat("<FieldRef Name=\"{0}\" />\n", viewFieldName);

            spQuery.ViewFields = bld.ToString();
            spQuery.ViewFieldsOnly = true;
        }

        private static void AddViewFields(SPQuery spQuery, Guid[] viewFieldNames)
        {
            StringBuilder bld = new StringBuilder();

            foreach (Guid viewFieldName in viewFieldNames)
                bld.AppendFormat("<FieldRef ID=\"{0}\" />", viewFieldName);

            spQuery.ViewFields = bld.ToString();
            spQuery.ViewFieldsOnly = true;
        }

        public static string BuildExpression(CAMLFilter filter, params IQueryModifier[] queryModifiers)
        {
            var queryBuilder = new StringBuilder("<Where>");

            if (!String.IsNullOrEmpty(filter.FilterExpression)) 
                queryBuilder.Append(filter.FilterExpression);

            queryBuilder.Append("</Where>");

            ApplyQueryModifiers(queryModifiers, queryBuilder);
            return queryBuilder.ToString();
        }

        private static void ApplyQueryModifiers(IQueryModifier[] queryModifiers, StringBuilder queryBuilder)
        {
            if (queryModifiers == null || queryModifiers.Length == 0) 
                return;

            foreach (IQueryModifier queryModifier in queryModifiers)
                queryModifier.ModifyQuery(queryBuilder);
        }
    }
}