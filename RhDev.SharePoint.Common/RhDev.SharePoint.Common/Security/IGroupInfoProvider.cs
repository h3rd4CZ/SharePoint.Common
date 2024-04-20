using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.Security
{
    public interface IGroupInfoProvider : IAutoRegisteredService
    {
        GroupInfo GetGroupInfo(SectionDesignation sectionDesignation, string groupName);
    }
}
