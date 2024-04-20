if ((Get-PSSnapin -Name Microsoft.SharePoint.PowerShell -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PsSnapin Microsoft.SharePoint.PowerShell
}

Start-Transcript .\InstallLog.log

$installPath = $MyInvocation.MyCommand.Path | Split-Path -Parent | Split-Path -Parent
$initScript = "000_Init.ps1"
$installScripts = Get-ChildItem -Path $installPath | Sort-Object Name

$script:autorunScript = $false
$script:skipNoAutorunScript = $false

$AllowedMenuChoices =
        97,  # 1
		49,
        98,  # 2
		50,
		#99,  # 3
		#51,
		105, # 9
		57
		
$AllowedKeys =
		89,  # y
        13,  # Enter
		78,  # n
		27  # Escape

function Execute($script, $path)
{
	$error.clear()
	if ($script.Attributes -ne "Directory")
	{
		if ($script -like '*000*')
			{ return }	

		$color = "green"
		
		
		Write-Host "Execute" $script"?" -foregroundcolor $color

		While (!($AllowedKeys -Contains $Key.VirtualKeyCode)) {
			$Key = $Host.UI.RawUI.ReadKey("NoEcho, IncludeKeyDown")
		}
		
		if ($Key.VirtualKeyCode -eq 89 -Or $Key.VirtualKeyCode -eq 13 -Or ($script:autorunScript -And !$breakAutorun))
		{
			Write-Host "Processing" $script "..." -foregroundcolor $color
			try
			{
				$elapsed = Measure-Command { .(Join-Path $path $script)	 }
				Write-Host "(Script took" $elapsed.TotalSeconds "seconds to process)"
				Write-Host
			}
			catch
			{
				Write-Host -foregroundcolor "red" $error
			}	
		}
		else
		{
			Write-Host "... Skipped" -foregroundcolor "red"			
		}	
	}	
}

function ExecuteInit()
{
	.(Join-Path $installPath $initScript)
	Write-Host "Init Completed" -foregroundcolor "green"
	write-Host
}

function Install()
{
	ExecuteInit
	foreach ($installScript in $installScripts) 
	{
		Execute $installScript $installPath
	}
}

While (!($AllowedMenuChoices -Contains $Choice.VirtualKeyCode)) 
{
	Clear-Host
	Write-Host "$ext_safeprojectname$ installation"
	Write-Host "------------------------------------------------------"
	Write-Host " -- 1     | Start installation" -foregroundcolor Green
	Write-Host " -- ESC   | EXIT" -foregroundcolor Blue
	

	$Choice = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	switch ($Choice.VirtualKeyCode) 
	{
		{$_ -eq 27} 
		{
			Write-Host
			Exit 
		}
	}
}			
	Write-Host $Choice.Character
		
	try
	{	
		switch ($Choice.VirtualKeyCode) 
		{ 
			{($_ -eq 97) -Or ($_ -eq 49)}
			{
				clear-Host
				Write-Host 
				Write-Host "INSTALLATION"
				Write-Host "-----------------------------------------------------------------"
				Write-Host " -- 'y' or ENTER     | Run current script" -foregroundcolor Green
				Write-Host " -- 'n' or ESC		 | Skip current script" -foregroundcolor Red
				Write-Host
				Install
				break
			} 
		}
	}
	catch
	{
		Write-Host -foregroundcolor "red" $error
		$error.clear()
		Exit
	}