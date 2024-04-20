using Microsoft.SharePoint;
using RhDev.SharePoint.Common.Configuration;

namespace $ext_safeprojectname$.Common.DataAccess.SharePoint.Configuration.Objects
{
    public class GlobalConfiguration : RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration.Objects.GlobalConfiguration
    {

        public const string EMPTYJOB_SCHEDULE_DEFAULT = "daily at 03:00:00";

        private ConfigurationKey EmptyJobAssignedServerKey = new ConfigurationKey(MODULE_NAME, "EmptyJobAssignedServer");
        private ConfigurationKey EmptyJobScheduleKey = new ConfigurationKey(MODULE_NAME, "EmptyJobSchedule");

        public string EmptyJobSchedule
        {
            get { return DataSource.GetValue(EmptyJobScheduleKey, EMPTYJOB_SCHEDULE_DEFAULT).AsString; }
            set { DataSource.SetValue(EmptyJobScheduleKey, value); }
        }

        public string EmptyJobServer
        {
            get { return DataSource.GetValue(EmptyJobAssignedServerKey).AsString; }
            set { DataSource.SetValue(EmptyJobAssignedServerKey, value); }
        }

        public GlobalConfiguration(IConfigurationDataSource dataSource) : base(dataSource)
        {
        }
    }
}
