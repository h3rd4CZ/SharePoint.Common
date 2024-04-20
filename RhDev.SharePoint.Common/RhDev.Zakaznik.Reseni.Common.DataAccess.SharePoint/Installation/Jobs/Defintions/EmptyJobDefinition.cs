using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RhDev.SharePoint.Common.Logging;
using $ext_safeprojectname$.Common.DataAccess.SharePoint.Configuration.Objects;
using $ext_safeprojectname$.Common.Setup;
using System;

namespace $ext_safeprojectname$.Common.DataAccess.SharePoint.Installation.Jobs.Definitions
{
    public class EmptyJobDefinition : SPJobDefinition
    {
        public const string JOB_TITLE = "RhDev Empty JOB title";

        public ITraceLogger TraceLogger { get; set; }

        public EmptyJobDefinition()
        {

        }
        public EmptyJobDefinition(SPWebApplication webApp, SPServer server, string schedule)
            : base(JOB_TITLE, webApp, server, SPJobLockType.None)
        {
            Title = JOB_TITLE;

            Schedule = string.IsNullOrEmpty(schedule)
                ? SPSchedule.FromString(GlobalConfiguration.EMPTYJOB_SCHEDULE_DEFAULT)
                : SPSchedule.FromString(schedule);
        }

        public EmptyJobDefinition(SPWebApplication webApplication, string schedule)
            : base(JOB_TITLE, webApplication, null, SPJobLockType.Job)
        {
            Title = JOB_TITLE;

            Schedule = string.IsNullOrEmpty(schedule)
                ? SPSchedule.FromString(GlobalConfiguration.EMPTYJOB_SCHEDULE_DEFAULT)
                : SPSchedule.FromString(schedule);
        }

        public override void Execute(Guid targetInstanceId)
        {
            IoC.Get.Backend.BuildUp(this);
        }
    }
}
