using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhDev.SharePoint.Common.Security
{
    public class UserInfoNameEqualityComparer : IEqualityComparer<UserInfo>
    {
        public bool Equals(UserInfo x, UserInfo y)
        {
            return String.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(UserInfo obj)
        {
            return obj.Name != null ? obj.Name.GetHashCode() : 0;
        }
    }
}
