
$global:secureStoreServiceHelper = New-Module -AsCustomObject -ScriptBlock {
		
	function CreateApplication
	{  
		param($targetApplicationName)

		# Get the service instance
		$SecureStoreServiceInstances = Get-SPServiceInstance | ? {$_.GetType().Equals([Microsoft.Office.SecureStoreService.Server.SecureStoreServiceInstance])}
		$SecureStoreServiceInstance = $SecureStoreServiceInstances | ? {$_.Server.Address -eq $env:COMPUTERNAME}

		If (-not $?) { Throw " - Failed to find Secure Store service instance" }

		# Start Service instance
		If ($SecureStoreServiceInstance.Status -eq "Disabled")
		{
			Write-Host  " - Starting Secure Store Service Instance..."
			$SecureStoreServiceInstance.Provision()
    
			If (-not $?) { Throw " - Failed to start Secure Store service instance" }
    
			# Wait    
			Write-Host  " - Waiting for Secure Store service..." -NoNewline
			While ($SecureStoreServiceInstance.Status -ne "Online")
			{
				Write-Host  "." -NoNewline
				Start-Sleep 1
				$SecureStoreServiceInstances = Get-SPServiceInstance | ? {$_.GetType().ToString() -eq "Microsoft.Office.SecureStoreService.Server.SecureStoreServiceInstance"}
				$SecureStoreServiceInstance = $SecureStoreServiceInstances | ? {$_.Server.Address -eq $env:COMPUTERNAME}
			}
    
			Write-Host  $($SecureStoreServiceInstance.Status)
		}

		# Create Service Application
		$GetSPSecureStoreServiceApplication = Get-SPServiceApplication | ? {$_.GetType().Equals([Microsoft.Office.SecureStoreService.Server.SecureStoreServiceApplication])}

		If ($GetSPSecureStoreServiceApplication -eq $Null) {
			throw "Cannot get Secure Store Service application"	
		}

		$secureStore = Get-SPServiceApplicationProxy | Where {$_.GetType().Equals([Microsoft.Office.SecureStoreService.Server.SecureStoreServiceApplicationProxy])}

		$ssApp = Get-SPSecureStoreApplication –ServiceContext $serviceContext –Name $targetApplicationName -ErrorAction SilentlyContinue

		if ($ssApp -eq $Null) {
			$usernameField = New-SPSecureStoreApplicationField –Name "Windows User Name" -Type UserName –Masked:$false
			$passwordField = New-SPSecureStoreApplicationField –Name "Windows Password" –Type Password –Masked:$true
			$domainField = New-SPSecureStoreApplicationField –Name "Windows Domain" -Type Generic –Masked:$false
			$fields = $usernameField,$passwordField,$domainField
 
			$adminClaim = New-SPClaimsPrincipal –Identity $secureStoreAdministrator –IdentityType  WindowsSamAccountName
			$targetApp = New-SPSecureStoreTargetApplication -name $targetApplicationName -friendlyname $targetApplicationName -contactemail $secureStoreAdministratorEmail -applicationtype Individual -timeoutinminutes 3
			New-SPSecureStoreApplication –ServiceContext $serviceContext –TargetApplication $targetApp –Field $fields –Administrator $adminClaim
		
			Write-Warning "Nyní v Centrální administraci servisní aplikace Secure Store Service nastavte jméno a heslo pro aplikaci $targetApplicationName pro účet, pod kterým běží OWSTIMER"
			Write-Warning "Na vývoji je také třeba nastavit pro účet, pod kterým běží integrační testy (typicky Administrator)"
		} 
		Else {
				Write-Host  " - Target application $targetApplicationName already provisioned."	
		}
	}
	
	Export-ModuleMember -Function *
}