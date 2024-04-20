using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhDev.SharePoint.Common
{
 [Serializable]
    public class GroupInfo : IPrincipalInfo
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string DisplayName {get { return Name; }}
        public bool IsValid { get { return Id != 0; } }
        public bool Isgroup {get { return true; }}
        public bool IsExternal => false;
        public bool IsDomainGroup => false;

        public GroupInfo(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        protected bool Equals(GroupInfo other)
        {
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GroupInfo) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} (ID {1})", Name, Id);
        }
    }
}
