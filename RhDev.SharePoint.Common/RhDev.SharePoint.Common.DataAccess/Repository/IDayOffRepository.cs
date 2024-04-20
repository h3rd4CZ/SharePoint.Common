using RhDev.SharePoint.Common.DataAccess.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.DataAccess.Repository
{
    public interface IDayOffRepository : IEntityRepository<DayOffConfigurationItem>
    {        
        IList<DayOffConfigurationItem> GetDayOffsByLcid(int lcid);
    }
}
