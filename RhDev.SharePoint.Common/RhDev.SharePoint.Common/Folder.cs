using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhDev.SharePoint.Common
{
    [Serializable]
    public class Folder
    {
        public string Name { get; private set; }

        public Folder()
        {
        }

        public Folder(string name)
        {
            if (Equals(null, name))
                throw new ArgumentNullException("name");

            Name = name;
        }

        public bool IsValidName()
        {
            return !string.IsNullOrEmpty(Name);
        }

        private bool Equals(Folder other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Folder)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
