Param([string]$resourcegroup ='devccvcloud1', [string]$webapp = 'rockwebapp', [string]$datasource , [string]$catalog,[string]$user, [string]$userPass)
$connectionStrConfig = @"
<?xml version="1.0"?>
<connectionStrings>
         <add name="RockContext" connectionString="Data Source=$datasource;Initial Catalog=$catalog;
             Integrated Security=False;User ID=$user;Password=$userPass;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;
             ApplicationIntent=ReadWrite;MultiSubnetFailover=False; MultipleActiveResultSets=true" providerName="System.Data.SqlClient"/>
  </connectionStrings>
"@
$connectionStrConfig | out-file .\web.ConnectionStrings.config -Force
$kudufile = get-item .\web.ConnectionStrings.config
ipmo .\kudufunctions.psm1 -Force
$rockwebPubProfile = Get-PublishingProfileCredentials -resourceGroupName $resourcegroup -webAppName $webapp
$rockwebAuthHeader = Get-KuduApiAuthorisationHeaderValue -resourceGroupName $resourcegroup -webAppName $webapp
Upload-FileToWebApp -resourceGroupName $resourcegroup -webAppName $webapp -localPath $kudufile.FullName -kuduPath $kudufile.name
