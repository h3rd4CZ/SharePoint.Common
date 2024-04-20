$sln = Add-SPSolution $setup.Packages.Externals.Path

Install-SPSolution –Identity $setup.Packages.Externals.FileName -GACDeployment -Force

while($sln.JobExists)
{
    Write-Host " > Deployment in progress..." 
    start-sleep -s 2
}

Write-Host ("Deployed: " + $sln.Deployed) -ForegroundColor Cyan
if($sln.Deployed -eq $false)
{
  Write-Host "Please go to central admin and deploy solution manually" -Foregroundcolor Yellow
}