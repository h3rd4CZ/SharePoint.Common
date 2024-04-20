using System;

namespace RhDev.SharePoint.Common
{
    [Serializable]
    public class UnitDesignationBase : IUnitDesignation
    {
        protected string designation;

        protected UnitDesignationBase(string designation)
        {
            this.designation = designation;
        }

        public UnitDesignationBase()
        {

        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
                return true;

            if (ReferenceEquals(obj, null))
                return false;

            if (obj.GetType() != GetType())
                return false;

            var other = (IUnitDesignation)obj;
            return String.Equals(other.GetAddress(), GetAddress(), StringComparison.OrdinalIgnoreCase);
        }

        public bool EqualsNonScheme(IUnitDesignation other)
        {
            if (ReferenceEquals(other, this))
                return true;

            if (ReferenceEquals(other, null))
                return false;

            if (other.GetType() != GetType())
                return false;

            var thisAddress = GetAddress();
            var otherAddress = other.GetAddress();

            var thisScheme = new Uri(thisAddress).Scheme;
            var otherScheme = new Uri(otherAddress).Scheme;

            thisAddress = thisAddress.Replace(thisScheme, string.Empty);
            otherAddress = otherAddress.Replace(otherScheme, string.Empty);

            return String.Equals(thisAddress, otherAddress, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            string str = GetAddress();
            return str != null ? str.GetHashCode() : 0;
        }

        public string GetAddress()
        {
            return designation;
        }

        public override string ToString()
        {
            return GetType().Name + " " + GetAddress();
        }
    }
}
