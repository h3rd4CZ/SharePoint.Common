using System;
using System.Collections.Generic;
using System.Linq;
using RhDev.SharePoint.Common.Security;
using RhDev.SharePoint.Common.Utils;

namespace RhDev.SharePoint.Common.Impl.Security
{
    public class HierarchicalGroupProvider : IHierarchicalGroupProvider
    {
        private const string GROUP_NAME_FORMAT = "{0} {1} {2}";

        public SectionGroupDefinition GetDefinition(string webTitle, ApplicationGroup group)
        {
            Guard.NotNull(group, nameof(group));

            GroupName groupName = new GroupName { Name = group.Name, Description = group.Description };

            return new SectionGroupDefinition(
                !Equals(null, group.CustomNameProvider) ? group.CustomNameProvider(webTitle) : string.Format(GROUP_NAME_FORMAT, group.ApplicationName, groupName.Name, webTitle),
                groupName.Description,
                group,
                webTitle);
        }
    }
}
