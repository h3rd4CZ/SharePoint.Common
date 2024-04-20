using RhDev.SharePoint.Common.Caching.Composition;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Security
{
    public interface IGroupMembershipManager : IAutoRegisteredService
    {
        void AddUserToGroup(SectionDesignation designation, ApplicationGroup groupKind, IPrincipalInfo user);
        void AddUserToGroup(SectionDesignation designation, ApplicationGroup groupKind, IList<IPrincipalInfo> users);

    }
}
