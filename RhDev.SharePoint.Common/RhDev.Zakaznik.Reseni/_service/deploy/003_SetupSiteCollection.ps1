$null = [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SharePoint")

$template = Get-SPWebTemplate "STS#0"

function SetCulture
{
	param($web)			
	
	$web.Locale = $culture
	$Web.RegionalSettings.Time24 = $TRUE;
	$web.IsMultilingual = $true;
	$web.AddSupportedUICulture($setup.ApplicationCulture);
	$web.Update()	
}

function CreateAppWeb
{
	param($appUrl)
	
	Write-Host "- Creating app web " $appUrl

	$url = $appUrl;

	$web = Get-SPWeb $url -ErrorAction SilentlyContinue
	$name = $setup.App.Name;

	if ($web -eq $null) 
	{
		$error.clear()
	
		$web = New-SPWeb -Url $url -Name $name -Template $template -Language 1029	
		
		SetCulture $web	
	}
	else 
	{
		Write-Host "-- Web ""$url"" already exist."
	}
}

function CreateAppSiteCollection
{
	param($appUrl)
	
	Write-Host "- Creating app site " $appUrl

	$url = $appUrl;

	$site = Get-SPSite $url -ErrorAction SilentlyContinue
	$name = $setup.App.Name;

	if ($site -eq $null) 
	{
		$error.clear()
	
		$site = New-SPSite -Url $url -Name $name -Template $template -OwnerAlias $setup.Administrators.Primary -Language 1029	
		
		SetCulture $site.RootWeb	
	}
	else 
	{
		Write-Host "-- Site ""$url"" already exist."
	}
}


# =======================================================================
# Zalozeni webu aplikace
# =======================================================================
CreateAppSiteCollection $setup.App.Site
CreateAppWeb $setup.App.Web

