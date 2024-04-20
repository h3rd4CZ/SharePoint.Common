using RhDev.SharePoint.Common.DataAccess;
using RhDev.SharePoint.Common.DataAccess.Repository;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Test.DayOffs
{
    public class DayOffMockList : IDayOffRepository
    {
        public int Create(DayOffConfigurationItem entity)
        {
            throw new NotImplementedException();
        }

        public int CreateElevated(DayOffConfigurationItem entity)
        {
            throw new NotImplementedException();
        }

        public int CreateFile(DayOffConfigurationItem entity, string fileName, bool requiresElevation)
        {
            throw new NotImplementedException();
        }

        public EntityLink GetAbsoluteLink(string appUrl, int itemId)
        {
            throw new NotImplementedException();
        }

        public IList<DayOffConfigurationItem> GetAllEntities()
        {
            throw new NotImplementedException();
        }

        public DayOffConfigurationItem GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IList<DayOffConfigurationItem> GetDayOffsByLcid(int lcid) =>
            new List<DayOffConfigurationItem>
            {
                new DayOffConfigurationItem{ Date = new DateTime(2020, 8, 10), Repeate = true, Lcid=1029},
                new DayOffConfigurationItem{ Date = new DateTime(2021, 8, 11), Repeate = false, Lcid=1029}
            };
        
        public LocationInfo GetLocation(int entityId)
        {
            throw new NotImplementedException();
        }

        public EntityLink GetRelativeLink(int itemId)
        {
            throw new NotImplementedException();
        }

        public void Update(DayOffConfigurationItem entity)
        {
            throw new NotImplementedException();
        }

        public void Update(DayOffConfigurationItem entity, bool createVersion)
        {
            throw new NotImplementedException();
        }

        public void UpdateElevated(DayOffConfigurationItem entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateElevated(DayOffConfigurationItem entity, bool createVersion)
        {
            throw new NotImplementedException();
        }

        public int UpdateFile(DayOffConfigurationItem entity, bool requiresElevation)
        {
            throw new NotImplementedException();
        }
    }
}
