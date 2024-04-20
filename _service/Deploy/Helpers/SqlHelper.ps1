
$global:sqlHelper = New-Module -AsCustomObject -ScriptBlock {
		
	function ExecuteNonQuery
	{
		param(
			[string] $connString,
			[string] $cmd
		     )	
	
	
			    $connection = new-object system.data.SqlClient.SQLConnection($connString)
			    $command = new-object system.data.sqlclient.sqlcommand($cmd,$connection)
			    $connection.Open()

			    $command.ExecuteNonQuery();		    			  

			    $connection.Close()

	}

	function ExecuteStoredProcedure
	{
		param(
			[string] $connString,
			[string] $procedureName,
			$params
		     )	
	
			    $spt = [System.Data.CommandType]::StoredProcedure;
			    $connection = new-object system.data.SqlClient.SQLConnection($connString)
			    $command = new-object system.data.sqlclient.sqlcommand($procedureName,$connection)
		            $command.CommandType = $spt;

		            foreach($p in $params)
			    {
				$command.Parameters.Add("@" + $p.PropertyName, $p.PropertyValue);
   		            }

			    $connection.Open()

			    $command.ExecuteNonQuery();		    			  

			    $connection.Close()

	}
	
	
	Export-ModuleMember -Function *
}