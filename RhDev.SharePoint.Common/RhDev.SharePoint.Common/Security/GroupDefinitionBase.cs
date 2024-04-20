using System;

namespace RhDev.SharePoint.Common.Security
{
    /// <summary>
    /// Uzivatelska skupina
    /// </summary>
    [Serializable]
    public abstract class GroupDefinitionBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public GroupDefinitionBase()
        {
            
        }

        protected GroupDefinitionBase(string name, string description)
        {
            Name = name;
            Description = description;
        }

        private bool Equals(GroupDefinitionBase other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Description, other.Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GroupDefinitionBase) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode()*397) ^ Description.GetHashCode();
            }
        }
    }
}
