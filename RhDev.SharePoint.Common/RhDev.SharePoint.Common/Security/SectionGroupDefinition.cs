using System;

namespace RhDev.SharePoint.Common.Security
{
    [Serializable]
    public class SectionGroupDefinition : GroupDefinitionBase
    {
        public ApplicationGroup GroupKind { get; set; }

        public string WebTitle { get; set; }

        public SectionGroupDefinition()
        {
        }

        public SectionGroupDefinition(string name, string description, ApplicationGroup groupKind, string webTitle) 
            : base(name, description)
        {
            GroupKind = groupKind;
            WebTitle = webTitle;
        }

        private bool Equals(SectionGroupDefinition other)
        {
            return base.Equals(other)
                   && GroupKind == other.GroupKind &&
                   WebTitle.Equals(other.WebTitle);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SectionGroupDefinition) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (int) GroupKind.GetHashCode();
                hashCode = (hashCode * 397) ^ WebTitle.GetHashCode();
                return hashCode;
            }
        }
    }
}
