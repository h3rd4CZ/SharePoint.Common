if ((Get-PSSnapin -Name Microsoft.SharePoint.PowerShell -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PsSnapin Microsoft.SharePoint.PowerShell
}

$deploymentScriptsPath = $MyInvocation.MyCommand.Path | Split-Path -Parent | Split-Path -Parent
$deploymentScriptsPath = (Join-Path $deploymentScriptsPath "Deploy")
$deploymentScriptsHelpersPath = (Join-Path $deploymentScriptsPath "Helpers")

.(Join-Path $deploymentScriptsPath "000_Init.ps1")


.(Join-Path $deploymentScriptsHelpersPath "SqlHelper.ps1")
.(Join-Path $deploymentScriptsHelpersPath "FeatureHelper.ps1")
.(Join-Path $deploymentScriptsHelpersPath "SolutionDeploymentHelper.ps1")

$global:workflowUpgradeHelper = New-Module -AsCustomObject -ScriptBlock {

	Function DisableActionQueue {
		Disable-SPFeature -Identity f1d85038-e710-4329-b551-19d83e17dbb5 -Confirm:$False -Url $setup.WebApplication.Url
	}
	
	Function EnableActionQueue {
		Enable-SPFeature -Identity f1d85038-e710-4329-b551-19d83e17dbb5 -Confirm:$False -Url $setup.WebApplication.Url
	}

	Function UpgradeSectionWorkflow {
		param($listTitle, $workflowName)
				
		Write-Host
		Write-Host "BEGIN upgrading workflow $workflowName"
		
		foreach($company in $setup.Companies)
		{
			foreach($section in $company.Sections)
			{
				$workflowImportHelper.ImportToSection($section, $listTitle, $workflowName)					
			}
		}
		
		Write-Host
		Write-Host "COMPLETED upgrading workflow" -ForegroundColor Green
    }	
		
	function UpgradeCompanyWorkflow 
	{
		param($listTitle, $workflowName)
		
		Write-Host
		Write-Host "BEGIN upgrading workflow $workflowName"	
		
		foreach($company in $setup.Companies)
		{
			$workflowImportHelper.ImportToCompany($company, $listTitle, $workflowName)
		}
		
		Write-Host
		Write-Host "COMPLETED upgrading workflow" -ForegroundColor Green
    }
	
	function ImportNewCompanyWorkflow 
	{
		param($listTitle, $contentTypeId, $workflowName)
		
		Write-Host
		Write-Host "BEGIN importing new workflow $workflowName"	
		
		$workflowImportHelper.ImportCompanyWorkflow($listTitle, $contentTypeId, $workflowName)
		
		Write-Host
		Write-Host "COMPLETED importing workflow" -ForegroundColor Green
    }

	function ImportUserDefinedAction 
	{
		param($actionName)
		
		Write-Host
		Write-Host "BEGIN importing user defined action $actionName"	
		
		$workflowImportHelper.ImportUserDefinedAction($actionName)
		
		Write-Host
		Write-Host "COMPLETED importing user defined action" -ForegroundColor Green
    }	

	function DeleteUserDefinedAction 
	{
		param([Guid]$actionId)
		
		Write-Host
		Write-Host "BEGIN deleting user defined action $actionId"	
		
		$workflowImportHelper.DeleteUserDefinedAction($actionId)
		
		Write-Host
		Write-Host "COMPLETED deleting user defined action" -ForegroundColor Green
    }	
		
	function ResetIIS {
		
		Write-Host
		Write-Host "BEGIN restarting IIS" -ForegroundColor Green		
		iisreset /restart
	}

	function RemoveWorkflowAssociation
	{
		param($listTile, $workflowName)

		Write-Host
		Write-Host "BEGIN removing workflow $workflowName asociation for $listTile"	
		
		$workflowImportHelper.RemoveWorkflowAssociation($listTile, $workflowName)
		
		Write-Host
		Write-Host "COMPLETED removing workflow asociation" -ForegroundColor Green
	}
		
	Export-ModuleMember -Function *
}

$global:listItemUpgradeHelper = New-Module -AsCustomObject -ScriptBlock {

	function UpdateListItemById {
		param ($webUrl, $listName, $itemId, $fieldId, $fieldValue)
		Start-SPAssignment -Global
				
		Write-Host
		Write-Host "BEGIN updating item by id ""$itemId"" in list ""$listName"" on web ""$webUrl"""
		
		$web = Get-SPWeb $webUrl		
		$list = $web.Lists[$listName]
		
		$itemUpdated = $FALSE
		
		foreach ($item in $list.GetItems()) 
		{			
			if ($item.ID -eq $itemId) 
			{			
				$item[$fieldId] = $fieldValue
				$item.Update()				
				$itemUpdated = $TRUE				
				break
			}			
		}
		
		if ($itemUpdated -eq $FALSE) 
		{
			throw "Item not found"
		}
		
		Stop-SPAssignment -Global
		
		Write-Host
		Write-Host "COMPLETED updating item" -ForegroundColor Green
	}
	
	function GetListItemById {
		param ($webUrl, $listName, $itemId)
		Start-SPAssignment -Global
				
		$web = Get-SPWeb $webUrl		
		$list = $web.Lists[$listName]
		
		return $list.GetItemById($itemId) 
		
		Stop-SPAssignment -Global
	}
	
	function UpdateListItemByTitle {
		param ($webUrl, $listName, $itemTitle, $fieldId, $fieldValue)
		Start-SPAssignment -Global
				
		Write-Host
		Write-Host "BEGIN updating item with title ""$itemTitle"" in list ""$listName"" on web ""$webUrl"""
		
		$web = Get-SPWeb $webUrl		
		$list = $web.Lists[$listName]
		
		$itemUpdated = $FALSE
		
		foreach ($item in $list.GetItems()) 
		{			
			if ($item.Title -eq $itemTitle) 
			{			
				$item[$fieldId] = $fieldValue
				$item.Update()				
				$itemUpdated = $TRUE				
				break
			}			
		}
		
		if ($itemUpdated -eq $FALSE) 
		{
			throw "Item not found"
		}
		
		Stop-SPAssignment -Global
		
		Write-Host
		Write-Host "COMPLETED updating item" -ForegroundColor Green
	}	

	function DeleteListItemByTitle {
		param ($webUrl, $listName, $itemTitle)
		Start-SPAssignment -Global
				
		Write-Host
		Write-Host "BEGIN deleting item with title ""$itemTitle"" in list ""$listName"" on web ""$webUrl"""
		
		$web = Get-SPWeb $webUrl		
		$list = $web.Lists[$listName]
		
		$items = $list.GetItems()

		$itemDeleted = $FALSE

		for($i = 0; $i -lt $items.Count; $i++) 
		{
			if ($items[$i].Title -eq $itemTitle) 
			{
				$items.Delete($i)
				$itemDeleted = $TRUE				
				break
			}
		}

		if ($itemDeleted -eq $FALSE) 
		{
			throw "Item not found"
		}
		
		Stop-SPAssignment -Global
		
		Write-Host
		Write-Host "COMPLETED deleting item" -ForegroundColor Green
	}
	
	Export-ModuleMember -Function *
}

$global:termsUpgradeHelper = New-Module -AsCustomObject -ScriptBlock {

	function InsertTerm {
		param ($dmsTermSet, $termName)
		
		$termFieldsSetUp = [Unicorn.MPERP.DMS.Administration.ConfigurationUtility]::GetTermFieldsSetup()
		
		$termFieldsSetUp.ImportTerm($dmsTermSet, $termName)
		
		Write-Host "COMPLETED inserting term ""$termName"" to term set ""$dmsTermSet""" -ForegroundColor Green
		Write-Host
	}
	
	Export-ModuleMember -Function *
}

$global:featureUpgradeHelper = New-Module -AsCustomObject -ScriptBlock {

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

		$fullFeatureTitle = "DMS_$featureTitle"
		Install-SPFeature -path $fullFeatureTitle
	}
	
	function ActivateFeature {
		param ([Guid]$featureId, $webUrl)	
		
		Enable-SPFeature -Identity $featureId -URL $webUrl
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
		$featuresForUpgrade = $webApplication.QueryFeatures($featureScope, $true) | ? { $_.DefinitionId -eq $featureId }

		Write-Host
		Write-Host "BEGIN upgrading feature"
		
		$featuresForUpgrade | %{ Write-Host "Will upgrade feature" $_.Definition.DisplayName "version" $_.Version "on" $_.Parent.Url }		
		
		foreach ($featureToUpgrade in $featuresForUpgrade) {
			$featureToUpgrade.Upgrade($false)
		}
		
		Stop-SPAssignment -Global		
		
		Write-Host
		Write-Host "COMPLETED feature upgrade"
	}	
	
	Export-ModuleMember -Function *
}

$global:searchConfigurationUpgradeHelper = New-Module -AsCustomObject -ScriptBlock {
	function RegisterNewProperty {
		param ($propName, $variantType, $type)
		
		Write-Host
		Write-Host "BEGIN updating search property ""$propName""" 

		$searchConfigurationHelper.RegisterProperty($propName, $variantType, $type)

		Write-Host
		Write-Host "COMPLETED search property upgrade" -ForegroundColor Green
	}

	function StartFullCrawl {
		
		$searchConfigurationHelper.StartFullCrawl()

		Write-Host
		Write-Host "Started full crawl" -ForegroundColor Green
	}

	Export-ModuleMember -Function *
}

$global:upgradeHelper = New-Module -AsCustomObject -ScriptBlock {

	function RunScript() {
		param ($scriptName)
		.(Join-Path $upgradeSetup.DeploymentScriptsPath $scriptName)
	}
	
	Export-ModuleMember -Function *
}


$global:listViewUpgradeHelper = New-Module -AsCustomObject -ScriptBlock {

	function UpgradeListViews {
		
		Write-Host
		Write-Host "BEGIN creating list views"

		$null = [System.Reflection.Assembly]::Load("Unicorn.MPERP.DMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=7440b9b4ededee65")
		$null = [System.Reflection.Assembly]::Load("Unicorn.MPERP.DMS.Composition, Version=1.0.0.0, Culture=neutral, PublicKeyToken=7440b9b4ededee65")
		$null = [System.Reflection.Assembly]::Load("Unicorn.MPERP.DMS.DocumentManagement.DataAccess.SharePoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=7440b9b4ededee65")

		$cont = [Unicorn.MPERP.DMS.Composition.CompositionRoot]::CreateFrontEndContainer()
		$listViewInstaller = $cont.GetInstance([System.Type]::GetType("Unicorn.MPERP.DMS.DocumentManagement.DataAccess.Installation.Section.DocumentListViewInstallation, Unicorn.MPERP.DMS.DocumentManagement.DataAccess.SharePoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=7440b9b4ededee65"))
		$navigationInstaller = $cont.GetInstance([System.Type]::GetType("Unicorn.MPERP.DMS.DocumentManagement.DataAccess.Installation.Section.CompanySectionNavigationFeatureInstallation, Unicorn.MPERP.DMS.DocumentManagement.DataAccess.SharePoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=7440b9b4ededee65"))

		foreach ($company in $setup.Companies)
		{
			foreach ($section in $company.Sections)
			{
				$sectionUrl = $setup.RootUrl + $section.WebUrl

				Write-Host "Upgrading views on web $sectionUrl"
				Start-SPAssignment -Global
				
				$web = Get-SPWeb $sectionUrl

				$listViewInstaller.ExecuteInstallation($web)
				$navigationInstaller.ExecuteInstallation($web)

				Stop-SPAssignment -Global
			}
		}
				
		Write-Host
		Write-Host "COMPLETED creating list view" -ForegroundColor Green
	}

	Export-ModuleMember -Function *
}

$global:activeDirectoryUpgradeHelper = New-Module -AsCustomObject -ScriptBlock {

	function UpgradeCompanyGroups {
		
		Write-Host
		Write-Host "BEGIN updating company groups in Active Directory" 

		$activeDirectoryHelper.EnsureCompanyGroups()

		Write-Host
		Write-Host "COMPLETED company groups upgrade" -ForegroundColor Green
	}

	function UpgradeApplicationGroups {
		
		Write-Host
		Write-Host "BEGIN updating application groups in Active Directory" 

		$activeDirectoryHelper.EnsureApplicationGroups()

		Write-Host
		Write-Host "COMPLETED application groups upgrade" -ForegroundColor Green
	}

	Export-ModuleMember -Function *
}

$global:termUpgradeHelper = New-Module -AsCustomObject -ScriptBlock {

	function RenameTerm {
		param($termSetName, $termPath, $newTermName)

		Write-Host
		Write-Host "BEGIN renaming term ""$termPath"" to ""$newTermName""" 

		$termFieldsSetUp = [Unicorn.MPERP.DMS.Administration.ConfigurationUtility]::GetTermFieldsSetup()
		$termFieldsSetUp.RenameTerm($termSetName, $termPath, $newTermName)

		Write-Host "COMPLETED term rename" -ForegroundColor Green
	}

	function DeleteTerm {
		param($termSetName, $termPath)

		Write-Host
		Write-Host "BEGIN deleting term ""$termPath"""

		$termFieldsSetUp = [Unicorn.MPERP.DMS.Administration.ConfigurationUtility]::GetTermFieldsSetup()
		$termFieldsSetUp.DeleteTerm($termSetName, $termPath)

		Write-Host "COMPLETED term delete" -ForegroundColor Green
	}

	function InsertTerm {
		param($termSetName, $termPath)

		Write-Host
		Write-Host "BEGIN inserting term ""$termPath"""

		$termFieldsSetUp = [Unicorn.MPERP.DMS.Administration.ConfigurationUtility]::GetTermFieldsSetup()
		$termFieldsSetUp.InsertTerm($termSetName, $termPath)

		Write-Host "COMPLETED term insert" -ForegroundColor Green
	}

	Export-ModuleMember -Function *
}

$global:jobHelper = New-Module -AsCustomObject -ScriptBlock {

	function WaitForJobToFinish([string]$jobName)
	{ 
		$job = Get-SPTimerJob -WebApplication $setup.WebApplication.Url | ?{ $_.DisplayName -like $jobName }
		if ($job -eq $null) 
		{
			Write-Host 'Job not found'
		}
		else
		{
			$jobLastRunTime = $job.LastRunTime
			$jobName = $job.Name
			
			Write-Host "Starting job $jobName, last run $jobLastRunTime"
			Start-SPTimerJob $job
	
			Write-Host -NoNewLine "Waiting for job to finish"        				
				
			while ($job.LastRunTime -eq $jobLastRunTime) 
			{
				Write-Host -NoNewLine .
				Start-Sleep -Seconds 2
			}
		
			Write-Host "`nJob $jobName finished"
		}
	}

	Export-ModuleMember -Function *
}

$global:secureStoreServiceUpgradeHelper = New-Module -AsCustomObject -ScriptBlock {

	function CreateApplication([string]$accountName) {
		
		Write-Host
		Write-Host "BEGIN creating new application in Secure Store Service" 

		$secureStoreServiceHelper.CreateApplication($accountName)

		Write-Host
		Write-Host "COMPLETED application creation" -ForegroundColor Green
	}

	Export-ModuleMember -Function *
}

$global:listViewImportUpgradeHelper = New-Module -AsCustomObject -ScriptBlock {

	function AddViewsToList($company, [string]$sourceFolder, [string]$listName, [array]$listViewsFileNames) {
		
		Write-Host
		Write-Host "BEGIN creating views to list $listName" 

		$listViewImportHelper.AddViewsToList($company, $sourceFolder, $listName, $listViewsFileNames)

		Write-Host
		Write-Host "COMPLETED views creation" -ForegroundColor Green
	}
	
	function ChangeScopeToRecursive($company, [string]$listName, [array]$listViewsNames) {
		
		Write-Host
		Write-Host "BEGIN changing Scope attribute to views to list $listName" 

		$listViewImportHelper.ChangeScopeToRecursive($company, $listName, $listViewsNames)

		Write-Host
		Write-Host "COMPLETED views change" -ForegroundColor Green
	}

	Export-ModuleMember -Function *
}
