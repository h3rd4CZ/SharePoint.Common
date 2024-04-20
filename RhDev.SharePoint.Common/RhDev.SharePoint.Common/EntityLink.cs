using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RhDev.SharePoint.Common
{
    [Serializable]
    public class EntityLink
    {
        [XmlIgnore]
        public Uri Uri => new Uri(UriString);
        public string UriString { get; set; }

        public string Description { get; set; }

        public EntityLink(Uri uri, string description)
        {
            UriString = uri.ToString();
            Description = string.IsNullOrEmpty(description) ? UriString : description;
        }

        public EntityLink()
        {
            
        }

        protected bool Equals(EntityLink other)
        {
            
            return Uri.Equals(other.Uri) && string.Equals(Description, other.Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EntityLink)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Uri.GetHashCode() * 397) ^ Description.GetHashCode();
            }
        }

        public static bool operator ==(EntityLink left, EntityLink right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityLink left, EntityLink right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Uri.ToString();
        }
    }
}
