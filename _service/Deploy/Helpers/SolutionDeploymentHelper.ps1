$global:solutionDeploymentHelper = New-Module -AsCustomObject -ScriptBlock {
		
	function UpdateSolution {
		param ([string] $packageName, [string] $packagePath)
		
		Update-SPSolution -Identity $packageName -LiteralPath $packagePath -GACDeployment

		$solution = Get-SPSolution $packageName
		Write-Host -NoNewline "Updating solution" $packageName

		while ($solution.JobExists -eq $true)
		{
		    Write-Host "." -NoNewline
		    sleep 1
		    $solution = Get-SPSolution $packageName
		}

		if ($solution.Deployed -eq $false) {
			throw "Solution deployment failed, check Central Administration for more information"			
		}
	}

	Export-ModuleMember -Function *
}