$null = [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SharePoint")
$deploymentScriptsPath = $MyInvocation.MyCommand.Path | Split-Path -Parent
$helpersPath = (Join-Path $deploymentScriptsPath "Helpers")
$helperScriptPath = "FeatureHelper.ps1"
.(Join-Path $helpersPath $helperScriptPath)

#RhDev common web, activates dependant site automatically
Write-Host "RhDev common Web feature enabling" -ForegroundColor Yellow
$featureHelper.EnableWebFeature($setup.App.Web, [Guid]"d691910d-4c56-49c5-a87c-0c7c3cd1e83e")

#SITE
Write-Host "Site feature enabling..." -ForegroundColor Yellow
$featureHelper.EnableSiteFeature($setup.App.Site, [Guid]"$guid4$")

Write-Host

#SITE ROOT WEB
Write-Host "Web feature enabling" -ForegroundColor Yellow
$featureHelper.EnableWebFeature($setup.App.Web, [Guid]"$guid6$")

Write-Host