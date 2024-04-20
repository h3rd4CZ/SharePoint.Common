using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace RhDev.SharePoint.Common
{
    [Serializable]
    public class UserInfo : IPrincipalInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public bool IsSiteAdmin { get; set; }
        public static UserInfo UnknownUser => new UserInfo(null, 0, null, "?", null,false, false);
        public bool IsValid => Id != 0;
        public bool Isgroup => false;
        public bool IsExternal { get; set; }
        public bool HasEmail => !String.IsNullOrEmpty(Email);
        public bool IsDomainGroup { get; }

        public SectionDesignation SectionDesignation { get; set; }

        [XmlIgnore]
        public CultureInfo Culture => new CultureInfo(CultureLcid);
        public int CultureLcid { get; set; }

        public UserInfo()
        {
            
        }
        public UserInfo(SectionDesignation sectionDesignation, int id, string name, string displayName, string email,
            bool isSiteAdmin, bool isDomainGroup)
        {
            SectionDesignation = sectionDesignation;
            Id = id;
            Name = name;
            DisplayName = displayName;
            Email = email;
            IsSiteAdmin = isSiteAdmin;
            CultureLcid = 1029;
            IsDomainGroup = isDomainGroup;
        }

        public static UserInfo CreateExternalUser(SectionDesignation sectionDesignation, string email)
        {
            var ui =
                new UserInfo(sectionDesignation, 0, "ExternalUser", "External User", email, false, false) {IsExternal = true};

            return ui;
        }

        public override string ToString()
        {
            return $"{Name} (ID {Id})";
        }

        protected bool Equals(UserInfo other)
        {
            return Id == other.Id && SectionDesignation.EqualsNonScheme(other.SectionDesignation);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((UserInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ (Equals(null, SectionDesignation) ? 0 : SectionDesignation.GetHashCode());
            }
        }        
    }



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

    public class UserInfoIdEqualityComparer : IEqualityComparer<UserInfo>
    {
        public bool Equals(UserInfo x, UserInfo y)
        {
            return Int32.Equals(x.Id, y.Id);
        }

        public int GetHashCode(UserInfo obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
