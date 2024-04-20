using System;

namespace RhDev.SharePoint.Common.Security
{
    [Flags]
    public enum ItemRoleDefinition
    {
        None = 0,
        User = 1,
        Admin = 1 << 1,
    }
}
