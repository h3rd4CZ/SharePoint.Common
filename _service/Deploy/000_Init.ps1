$global:setup = @{}

$setup.FarmFeatureId = "7a2d9721-7980-45ca-b66e-c14b34c8a037"
$setup.WebAppFeatureId = "ef8aa402-0e7a-4d70-82b3-34b4e0f10412"

# === APPLICATION CULTURE ===
$setup.ApplicationCulture = [System.Globalization.CultureInfo]::CreateSpecificCulture("cs-CZ")

# === Timer JOBS ===
#$setup.Jobs = @{}
#$setup.Jobs.JobName = @{}
#$setup.Jobs.JobName.Schedule = "every 10 minutes"
#$setup.Jobs.JobName.Server = "<servername>"

# === WEB APPLICATION PROPERTIES === #
#$setup.WebApplication = @{}
#$setup.WebApplication.Url = "<webappurl>" #ENV

# === COMMON === #
$global:environmentName = "Production" #ENV

# === INSTALLATION PATHS === #
$packagesPath = ($MyInvocation.MyCommand.Definition  | Split-Path -Parent) + "\Packages"
$setup.Packages = @{}
$setup.Packages.RootPath = $packagesPath #ENV

$setup.Packages.MainPackage = @{}
$setup.Packages.MainPackage.FileName = "RhDev.SharePoint.wsp"
$setup.Packages.MainPackage.Path = (Join-Path $setup.Packages.RootPath $setup.Packages.MainPackage.FileName)

$setup.Packages.Externals = @{}
$setup.Packages.Externals.FileName = "RhDev.SharePoint.Externals.wsp"
$setup.Packages.Externals.Path = (Join-Path $setup.Packages.RootPath $setup.Packages.Externals.FileName)

#$setup.Packages.TimerJobPackage = @{}
#$setup.Packages.TimerJobPackage.FileName = "<JOBSOLUTIONNAME>"
#$setup.Packages.TimerJobPackage.Path = (Join-Path $setup.Packages.RootPath $setup.Packages.TimerJobPackage.FileName)