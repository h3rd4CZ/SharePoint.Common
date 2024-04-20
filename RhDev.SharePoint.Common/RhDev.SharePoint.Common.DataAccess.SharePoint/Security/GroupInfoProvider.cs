using System;
using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Security;
using Microsoft.SharePoint;
using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public class GroupInfoProvider : ServiceBase, IGroupInfoProvider
    {
        public GroupInfoProvider(ITraceLogger traceLogger) : base(traceLogger)
        {
        }

        protected override TraceCategory TraceCategory => TraceCategories.Security;

        private SPGroup GetGroup(SectionDesignation sectionDesignation, string groupName)
        {
            SPGroup spGroup = null;

            SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    using (SPSite site = new SPSite(sectionDesignation.GetAddress()))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            try
                            {
                                spGroup = web.Groups[groupName];
                            }
                            catch (Exception e)
                            {
                                WriteUnexpectedTrace(e, "Unable to fetch user group by name {0}", groupName);
                            }
                        }
                    }
                });
            return spGroup;
        }

        public GroupInfo GetGroupInfo(SectionDesignation sectionDesignation, string groupName)
        {
            SPGroup spGroup = GetGroup(sectionDesignation, groupName);

            if (spGroup == null)
                return null;

            return new GroupInfo(spGroup.ID, spGroup.Name, spGroup.Description);
        }
    }
}
