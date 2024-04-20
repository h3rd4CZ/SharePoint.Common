using Microsoft.SharePoint.Administration;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration.Objects;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Installation;
using $ext_safeprojectname$.Common.DataAccess.SharePoint.Installation.Jobs.Definitions;
using System;
using GlobalConfiguration = $ext_safeprojectname$.Common.DataAccess.SharePoint.Configuration.Objects.GlobalConfiguration;

namespace $ext_safeprojectname$.Common.DataAccess.SharePoint.Installation.Jobs.Installations
{
    public class EmptyJobInstallation : JobDefinitionInstallationBase
    {

        private readonly GlobalConfiguration _config;

        protected override string AssignedServerName
        {
            get
            {
                try
                {
                    return _config.EmptyJobServer;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occured when retrieving AssignedServerName : {ex}");
                    return string.Empty;
                }
            }
        }

        protected override string JobName => EmptyJobDefinition.JOB_TITLE;


        private string schedule => _config.EmptyJobSchedule;

        public EmptyJobInstallation(ObjectConfigurationFactory configfactory)
        {
            _config = configfactory.GetConfigurationObject<GlobalConfiguration>();
        }

        protected override SPJobDefinition CreateJob(SPWebApplication webApplication, SPServer server)
        {
            var defaultSchedule = string.Empty;

            try
            {
                defaultSchedule = schedule;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured when retrieving schedule : {ex}");
            }

            return Equals(null, server)
                ? new EmptyJobDefinition(webApplication, defaultSchedule)
                : new EmptyJobDefinition(webApplication, server, defaultSchedule);
        }
    }

}
