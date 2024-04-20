using System;
using Microsoft.SharePoint.Administration;
using RhDev.SharePoint.Common.Logging;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Installation
{
    public abstract class JobDefinitionInstallationBase : FeatureInstallation<SPWebApplication>
    {
        
        protected abstract string AssignedServerName { get; }

        protected abstract string JobName { get; }

        protected override void DoExecuteInstallation(SPWebApplication webApplication)
        {
            Console.WriteLine("Installation of timer job {0} started", JobName);

            DeleteExistingJob(webApplication);
            SPServer assignedServer = GetAssignedServer();

            SPJobDefinition timer = CreateJob(webApplication, assignedServer);
            timer.Update();

            Console.WriteLine("Job installed successfully");
        }

        private SPServer GetAssignedServer()
        {
            if (String.IsNullOrEmpty(AssignedServerName))
            {
                Console.WriteLine("Job {0} will NOT be bound to any server", JobName);
                return null;
            }

            SPServerCollection spServerCollection = SPFarm.Local.Servers;
            SPServer server = spServerCollection[AssignedServerName];

            if (Equals(null, server))
                throw new InvalidOperationException($"Server {AssignedServerName} was not found in this farm");

            Console.WriteLine("Job {0} will be bound to the {1} server", JobName, server.DisplayName);

            return server;
        }
        
        private void DeleteExistingJob(SPWebApplication webApplication)
        {
            foreach (SPJobDefinition job in webApplication.JobDefinitions)
            {
                if (job.Name != JobName)
                    continue;

                job.Delete();
                Console.WriteLine("Existing job {0} deleted", job.Name);
            }
        }

        protected abstract SPJobDefinition CreateJob(SPWebApplication webApplication, SPServer server);

        protected override void DoExecuteUninstallation(SPWebApplication webApplication)
        {
            Console.WriteLine("Job {0} uninstallation started", JobName);
            DeleteExistingJob(webApplication);
            Console.WriteLine("Job uninstalled successfully");
        }
    }
}
