using System;

namespace RhDev.SharePoint.Common.DataAccess.Repository.Entities
{
    public class DayOffConfigurationItem : EntityBase
    {
        public DateTime Date { get; set; }

        public bool Repeate { get; set; }

        public double Lcid { get; set; }
    }
}
