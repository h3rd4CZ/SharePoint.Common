$null = [System.Reflection.Assembly]::Load("$ext_safeprojectname$.Common.DataAccess.SharePoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=78afb44363f8be41")

$configuration = new-object "$ext_safeprojectname$.Common.DataAccess.SharePoint.Configuration.Powershell.ConfigurationUtility"

$configuration.GlobalConfiguration.ConnectionString = $setup.DBConnectionStringWithEDMX;

$configuration.GlobalConfiguration.EmptyJobSchedule = $setup.Jobs.Empty.Schedule;
$configuration.GlobalConfiguration.EmptyJobServer = $setup.Jobs.Empty.Server;
