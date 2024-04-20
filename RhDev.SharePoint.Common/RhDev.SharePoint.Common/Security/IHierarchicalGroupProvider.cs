using RhDev.SharePoint.Common.Caching.Composition;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Security
{
    public interface IHierarchicalGroupProvider : IAutoRegisteredService
    {
        SectionGroupDefinition GetDefinition(string webTitle, ApplicationGroup groupKind);
    }
}
