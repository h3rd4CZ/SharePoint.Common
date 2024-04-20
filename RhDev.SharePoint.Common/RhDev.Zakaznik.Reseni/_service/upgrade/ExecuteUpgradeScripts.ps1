param
(
	#[int]$version
)

if ((Get-PSSnapin -Name Microsoft.SharePoint.PowerShell -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PsSnapin Microsoft.SharePoint.PowerShell
}

Start-Transcript .\UpgradeLog.log

$deploymentScriptsPath = $MyInvocation.MyCommand.Path | Split-Path -Parent | Split-Path -Parent
$deploymentScriptsPath = (Join-Path $deploymentScriptsPath "Deploy")
$deploymentScriptsHelpersPath = (Join-Path $deploymentScriptsPath "Helpers")

.(Join-Path $deploymentScriptsPath "000_Init.ps1")
.(Join-Path $deploymentScriptsHelpersPath "SqlHelper.ps1")
.(Join-Path $deploymentScriptsHelpersPath "FeatureHelper.ps1")
.(Join-Path $deploymentScriptsHelpersPath "SolutionDeploymentHelper.ps1")

$AllowedKeys =
		89,  # y
        13,  # Enter
		78,  # n
        32   # Space
		
$upgradeDirectory = $MyInvocation.MyCommand.Path | Split-Path -Parent
$upgradeDirectoryItems = Get-ChildItem -Path $upgradeDirectory
$counter = 0;
$versions = @();

foreach ($item in $upgradeDirectoryItems) 
{
	if ($item.Attributes -match "Directory")
	{
		$versions += $item.Name
	}
}

Clear-Host
Write-Host
Write-Host " Rewards Upgrade"
Write-Host "------------------------------------- AVAILABLE VERSIONS --------"
Write-Host
foreach ($version in $versions) 
{
	Write-Host -NoNewline -ForegroundColor "yellow" $counter
	Write-Host "`t" $version
	$counter = $counter + 1
}
Write-Host
Write-Host "------------------------------------------------------- HELP ----"
Write-Host " YES (y, return) / NO (n, space)"
Write-Host "-----------------------------------------------------------------"
Write-Host
$choice = Read-Host " Write your yellow choice? "

$version = $versions[$choice]
if ($version -eq $null)
{
	Write-Host
	Write-Host "Invalid choice" -foregroundColor "red"
	Write-Host
	exit	
}
Write-Host
Write-Host "-----------------------------------------------------------------" -foregroundColor "yellow"
Write-Host "Upgrading to version " $version.toString() -foregroundColor "yellow"
Write-Host "-----------------------------------------------------------------" -foregroundColor "yellow"
	
$versionPath = $upgradeDirectory + "\" + $version

foreach ($upgradeScript in (Get-ChildItem -Path $versionPath | Sort-Object Name))
{
	$Key = $null
	Write-Host
	Write-Host "Execute" $upgradeScript"?"

	While (!($AllowedKeys -Contains $Key.VirtualKeyCode)) 
	{
		$Key = $Host.UI.RawUI.ReadKey("NoEcho, IncludeKeyDown")
	}
	
	if ($Key.VirtualKeyCode -eq 89 -Or $Key.VirtualKeyCode -eq 13)
	{
		Write-Host
		Write-Host "Processing" $upgradeScript "..." -foregroundcolor "cyan"
		try
		{
			$elapsed = Measure-Command { .(Join-Path $versionPath $upgradeScript)	 }
			Write-Host "(Script took" $elapsed.TotalSeconds "seconds to process)"
		}
		catch
		{
			Write-Host -foregroundcolor "red" $error
			exit
		}
	}
	else
	{
		Write-Host "... Skipped" -foregroundcolor "red"			
	}
}