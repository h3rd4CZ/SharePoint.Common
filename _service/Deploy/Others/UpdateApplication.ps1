param (
[bool]$updateCore = $true
#[bool]$updateJobs = $false
)

if ((Get-PSSnapin -Name Microsoft.SharePoint.PowerShell -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PsSnapin Microsoft.SharePoint.PowerShell
}

$deploymentScriptsPath = $MyInvocation.MyCommand.Path | Split-Path -Parent | Split-Path -Parent
.(Join-Path $deploymentScriptsPath "000_Init.ps1")

$helpersPath = (Join-Path $deploymentScriptsPath "Helpers")
.(Join-Path $helpersPath "SolutionDeploymentHelper.ps1")

function DisableJobs($webApplication)
{
	Write-Host "Disabling jobs"
	$webApplication.JobDefinitions | ? { $_.Title -match $jobDefinitionTitleMask } | % { Disable-SPTimerJob $_ }
}

function EnableJobs($webApplication)
{
	Write-Host "Enabling jobs"
	$webApplication.JobDefinitions | ? { $_.Title -match $jobDefinitionTitleMask } | % { Enable-SPTimerJob $_ }
}

function UpdateSolution($jobs)
{			
	if($jobs -eq $true)
	{
		Write-Host "Timerjob solution update will start now..." -ForegroundColor:Yellow
		$deployed = $solutionDeploymentHelper.UpdateSolution($setup.Packages.TimerJobPackage.FileName, $setup.Packages.TimerJobPackage.Path)
	}
	else
	{
		Write-Host "Solution update will start now..." -ForegroundColor:Yellow
		$deployed = $solutionDeploymentHelper.UpdateSolution($setup.Packages.MainPackage.FileName, $setup.Packages.MainPackage.Path)
	}
	Write-Host
}

function RestartTimer
{
	Write-Host "SPTimer will be restarted now..." -ForegroundColor:Yellow
		
	.(Join-Path $deploymentScriptsPath "801_RestartTimer.NOAUTORUN.ps1")
}

function TakeIisOffline($webApplication)
{
	Write-Host "Taking IIS offline"
	
	$settings = $webApplication.IisSettings[[Microsoft.SharePoint.Administration.SPUrlZone]::Default]
	$appOfflineIisFilePath = Join-Path $settings.Path.FullName "app_offline.htm"
	
	Copy-Item $appOfflineTemplateFilePath $appOfflineIisFilePath -Force
}

function TakeIisOnline
{
	Write-Host "Taking IIS online"
	
	$settings = $webApplication.IisSettings[[Microsoft.SharePoint.Administration.SPUrlZone]::Default]
	$appOfflineIisFilePath = Join-Path $settings.Path.FullName "app_offline.htm"
	
	Remove-Item $appOfflineIisFilePath -Force
}

Write-Host "Starting application update" -ForegroundColor:Cyan

Start-SPAssignment -Global
$webApplication = Get-SPWebApplication $setup.WebApplication.Url

if($updateCore -eq $true)
{
  UpdateSolution $false
}

if($updateJobs -eq $true)
{
  UpdateSolution $true
}

RestartTimer

Stop-SPAssignment -Global	
Write-Host "Completed application update" -ForegroundColor:Green
