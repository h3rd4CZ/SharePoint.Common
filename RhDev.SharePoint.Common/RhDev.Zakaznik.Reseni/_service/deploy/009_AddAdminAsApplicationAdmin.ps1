Write-Host "Adding admin to application admin group..."

$admin = $setup.Administrators.Primary;
$web = Get-SPWeb $setup.App.Web
$usr  =$web.EnsureUser($admin)

$group = $web.SiteGroups["<site group name>"]

$group.AddUser($usr);

