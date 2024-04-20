using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhDev.SharePoint.Common
{
    [Serializable]
    public class SectionDesignation : UnitDesignationBase
    {
        public string Address
        {
            get { return designation; }
            set { designation = value; }
        }
        private SectionDesignation(string designation)
            : base(designation)
        {
        }

        public SectionDesignation()
        {

        }

        public static SectionDesignation FromString(string designation)
        {
            return new SectionDesignation(designation);
        }
    }
}
