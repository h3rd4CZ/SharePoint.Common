using Microsoft.SqlServer.Server;
using RhDev.SharePoint.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Logging
{
    [Serializable]
    public class DiagnosticsServiceConfiguration
    {
        public string Name { get; set; }

        public IList<TraceCategory> AvailableCategories { get; set; }

        public DiagnosticsServiceConfiguration()
        {

        }

        public DiagnosticsServiceConfiguration(string name, IList<TraceCategory> categories)
        {
            Guard.StringNotNullOrWhiteSpace(name, nameof(name));
            Guard.NotNull(categories, nameof(categories));

            this.Name = name;
            this.AvailableCategories = categories;
        }
    }
}
