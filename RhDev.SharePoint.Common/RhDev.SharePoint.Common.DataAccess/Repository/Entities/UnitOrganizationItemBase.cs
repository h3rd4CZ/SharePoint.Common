using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhDev.SharePoint.Common.DataAccess.Repository.Entities
{
    [Serializable]
    public class UnitOrganizationItemBase : HistoryEntityBase
    {
        /// <summary>
        /// ID organizacni jednotky 
        /// </summary>
        public string UnitId { get; set; }
    }
}
