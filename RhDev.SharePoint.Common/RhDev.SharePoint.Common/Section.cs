using System;

namespace RhDev.SharePoint.Common
{
    [Serializable]
    public class Section
    {
        public string CompanyTitle { get; set; }
        public string OrgUnitTitle { get; set; }
        public string OrgUnitId { get; set; }
        public SectionDesignation SectionDesignation { get; set; }


        public Section()
        {
        }

        public Section(string cTitle, string ouTitle, string ouId, SectionDesignation sd)
        {
            
            CompanyTitle = cTitle;
            OrgUnitTitle = ouTitle;
            OrgUnitId = ouId;
            SectionDesignation = sd;
        }
        public Section(string cTitle, string ouTitle, string ouId, string cUrl)
        {
            
            CompanyTitle = cTitle;
            OrgUnitTitle = ouTitle;
            OrgUnitId = ouId;
            SectionDesignation = SectionDesignation.FromString(cUrl);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} ({2})",CompanyTitle, OrgUnitTitle, OrgUnitId);            
        }
    }
}
