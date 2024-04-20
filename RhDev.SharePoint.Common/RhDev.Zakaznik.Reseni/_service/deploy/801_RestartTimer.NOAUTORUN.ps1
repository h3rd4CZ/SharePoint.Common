#Restart-Service SPTimerV4

foreach ($server in get-spserver | where {$_.Role -eq "Application" -or $_.Role -eq "Custom"})
{
	Write-Host "Stopping SPTimerV4 on application server " $server.Name
	$service = Get-WmiObject -computer $server.Name Win32_Service -Filter "Name='SPTimerV4'"
	$null = $service.InvokeMethod('StopService',$Null)
	start-sleep -s 5
	
	Write-Host "Starting SPTimerV4 on application server " $server.Name
	$null = $service.InvokeMethod('StartService',$Null)
	start-sleep -s 5
	$service.State
}