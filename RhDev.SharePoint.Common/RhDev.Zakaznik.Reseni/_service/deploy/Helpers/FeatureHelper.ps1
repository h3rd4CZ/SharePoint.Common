
$global:featureHelper = New-Module -AsCustomObject -ScriptBlock {
		
	function EnableFarmFeature
	{
		param([Guid]$featureId)		
		Start-SPAssignment -global
	
		Enable-SPFeature -Identity $featureId

		Stop-SPAssignment -global
	}

	function EnableWebAppFeature {
		param([string]$siteUrl, [Guid]$featureId)
		
		Start-SPAssignment -global
				
		Enable-SPFeature -Identity $featureId -URL $siteUrl
		
		Stop-SPAssignment -global
	}

	function EnableSiteFeature {
		param([string]$siteUrl, [Guid]$featureId)
		
		Start-SPAssignment -global
	
		$site = Get-SPSite $siteUrl
		$feature = $site.Features[$featureId]
	
		if ($feature -eq $null) {
			Enable-SPFeature -Identity $featureId -URL $siteUrl
		}
		else {
			Write-Host "- Feature" $feature.Definition.DisplayName "already activated on site" $siteUrl -ForegroundColor Gray
		}

		Stop-SPAssignment -global
	}

	function EnableWebFeature {
		param([string]$webUrl, [Guid]$featureId)
		
		Start-SPAssignment -global
	
		$web = Get-SPWeb $webUrl
		$feature = $web.Features[$featureId]
	
		if ($feature -eq $null) {
			Enable-SPFeature -Identity $featureId -URL $webUrl	
		}
		else {
			Write-Host "- Feature" $feature.Definition.DisplayName "already activated on web" $siteUrl -ForegroundColor Gray
		}

		Stop-SPAssignment -global
	}

	function UpgradeSiteFeature {
		param ([Guid]$featureId)				
		UpgradeFeature "Site" $featureId
	}	
	
	function UpgradeWebFeature {
		param ([Guid]$featureId)				
		UpgradeFeature "Web" $featureId
	}	
	
	function InstallNewFeature {
		param ($featureTitle)

		$fullFeatureTitle = featureTitle
		Install-SPFeature -path $fullFeatureTitle
	}
	
	function ActivateFeature {
		param ([Guid]$featureId, $webUrl)	

		Enable-SPFeature -Identity $featureId -URL $webUrl -ErrorAction Stop
	}

	function DeactivateFeature {
		param ([Guid]$featureId, $webUrl)	
		
		Disable-SPFeature -Identity $featureId -URL $webUrl
	}

	function InstallAndActivateNewFeature {
		param ([Guid]$featureId, $featureTitle, $webUrl)

		InstallNewFeature $featureTitle		
		ActivateFeature $featureId $webUrl
	}

	function UpgradeFeature {
		param ($featureScope, [Guid]$featureId)		
		
		Start-SPAssignment –Global 

		$webApplication = Get-SPWebApplication $setup.WebApplication.Url
		$featuresForUpgrade = $webApplication.QueryFeatures($featureId, $true);


		Write-Host
		Write-Host "BEGIN upgrading feature"

		#$featuresForUpgrade | %{ Write-Host "Will upgrade feature" $_.Definition.DisplayName "version" $_.Version "on" $_.Parent.Url }		
		
		foreach ($featureToUpgrade in $featuresForUpgrade) {
			Write-Host "Upgrading feature at" $featureToUpgrade.Parent
			$featureToUpgrade.Upgrade($false)
		}
		
		Stop-SPAssignment -Global		
		
		Write-Host
		Write-Host "COMPLETED feature upgrade"
	}		

	Export-ModuleMember -Function *
}