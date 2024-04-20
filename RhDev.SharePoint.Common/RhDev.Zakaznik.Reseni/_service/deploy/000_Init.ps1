$global:setup = @{}

$setup.FarmFeatureId = "$guid2$"
$setup.WebAppFeatureId = "$ext_guid3$"

# === APPLICATION CULTURE ===
$setup.ApplicationCulture = [System.Globalization.CultureInfo]::CreateSpecificCulture("cs-CZ")

# === Database ===
$setup.DatabaseName = "<dbname>" #ENV
$setup.DatabaseServer = "dbeserver\instance" #ENV
$dbn = $setup.DatabaseName;
$dbs = $setup.DatabaseServer;
$setup.DBCreateString = "data source=$dbs;Initial Catalog=master;integrated security=True;multipleactiveresultsets=True;" #ENV
$setup.DBConnectionString = "data source=$dbs;initial catalog=$dbn;integrated security=True;multipleactiveresultsets=True;App=EntityFramework" #ENV
$plainConString = $setup.DBConnectionString; 
$setup.DBConnectionStringWithEDMX = "<EDMXSCHEMA>'$plainConString'" #ENV

$setup.Common = @{}

# === Timer JOBS ===
$setup.Jobs = @{}
$setup.Jobs.Empty = @{}
$setup.Jobs.Empty.Schedule = "daily at 03:00:00"
$setup.Jobs.Empty.Server = "<server name to bound to>" #ENV

# === APPLICATION PATHS === #
$setup.App = @{}
$setup.App.Site = "<app site url>" #ENV
$setup.App.Web = "<app web url>" #ENV
$setup.App.Name = "<app name>" #ENV

# === SITE COLLECTIONS PROPERTIES === #
$setup.Administrators = @{}
$setup.Administrators.Primary = "i:0#.w|<login>" #ENV
$setup.Administrators.Secondary = "i:0#.w|<login>" #ENV

# === WEB APPLICATION PROPERTIES === #
$setup.WebApplication = @{}
$setup.WebApplication.Url = "<webapp url>" #ENV

# === INSTALLATION PATHS === #
$packagesPath = ($MyInvocation.MyCommand.Definition  | Split-Path -Parent) + "\Packages"
$setup.Packages = @{}
$setup.Packages.RootPath = $packagesPath

$setup.Packages.MainPackage = @{}
$setup.Packages.MainPackage.FileName = "$ext_safeprojectname$.wsp"
$setup.Packages.MainPackage.Path = (Join-Path $setup.Packages.RootPath $setup.Packages.MainPackage.FileName)

$setup.Packages.TimerJobPackage = @{}
$setup.Packages.TimerJobPackage.FileName = "$ext_safeprojectname$.TimerJobs.wsp"
$setup.Packages.TimerJobPackage.Path = (Join-Path $setup.Packages.RootPath $setup.Packages.TimerJobPackage.FileName)