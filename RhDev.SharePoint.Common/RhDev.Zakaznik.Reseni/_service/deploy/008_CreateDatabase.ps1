$deploymentScriptsPath = $MyInvocation.MyCommand.Path | Split-Path -Parent
$helpersPath = (Join-Path $deploymentScriptsPath "Helpers")
$helperScriptPath = "SqlHelper.ps1"
.(Join-Path $helpersPath $helperScriptPath)

$rootPath = $MyInvocation.MyCommand.Path | Split-Path -Parent | Split-Path -Parent
$sqlScriptPath = (Join-Path $rootPath "SQL")
$sqlFile = "AppDB.sql"
$sqlFile = Join-Path $sqlScriptPath $sqlFile

$sql = Get-Content $sqlFile

Write-Host "Creating database..."
$dbn = $setup.DatabaseName
$sqlHelper.ExecuteNonQuery($setup.DBCreateString, "CREATE DATABASE $dbn");

Write-Host "Creating tables..."
$sqlHelper.ExecuteNonQuery($setup.DBConnectionString, $sql);

Write-Host "Adding app pool user as owner..."
$webApp =Get-SPWebApplication $setup.WebApplication.Url
$webAppUser = $webApp.ApplicationPool.Username;

#adding user to database
$addUserParams = @();
$addUserParams += New-Object -TypeName PSObject -Property @{PropertyName='loginame'; PropertyValue =$webAppUser}

$sqlHelper.ExecuteStoredProcedure($setup.DBConnectionString, 'sp_adduser', $addUserParams);

#assign user to role
$addRoleParams = @();
$addRoleParams += New-Object -TypeName PSObject -Property @{PropertyName='rolename'; PropertyValue ='db_datareader'}
$addRoleParams += New-Object -TypeName PSObject -Property @{PropertyName='membername'; PropertyValue =$webAppUser}

$sqlHelper.ExecuteStoredProcedure($setup.DBConnectionString, 'sp_addrolemember', $addRoleParams);

$addRoleParamsWriter = @();
$addRoleParamsWriter += New-Object -TypeName PSObject -Property @{PropertyName='rolename'; PropertyValue ='db_datawriter'}
$addRoleParamsWriter += New-Object -TypeName PSObject -Property @{PropertyName='membername'; PropertyValue =$webAppUser}

$sqlHelper.ExecuteStoredProcedure($setup.DBConnectionString, 'sp_addrolemember', $addRoleParamsWriter);

