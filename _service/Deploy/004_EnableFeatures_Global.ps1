$null = [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SharePoint")
$deploymentScriptsPath = $MyInvocation.MyCommand.Path | Split-Path -Parent
$helpersPath = (Join-Path $deploymentScriptsPath "Helpers")
$helperScriptPath = "FeatureHelper.ps1"
.(Join-Path $helpersPath $helperScriptPath)

#farm feature (TraceLogger)
$featureHelper.EnableFarmFeature([Guid]$setup.FarmFeatureId)