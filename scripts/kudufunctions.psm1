function Get-PublishingProfileCredentials($resourceGroupName, $webAppName){
 
    $resourceType = "Microsoft.Web/sites/config"
    $resourceName = "$webAppName/publishingcredentials"
 
    $publishingCredentials = Invoke-AzureRmResourceAction -ResourceGroupName $resourceGroupName -ResourceType $resourceType -ResourceName $resourceName -Action list -ApiVersion 2015-08-01 -Force
 
       return $publishingCredentials
}
 
function Get-KuduApiAuthorisationHeaderValue($resourceGroupName, $webAppName){
 
    $publishingCredentials = Get-PublishingProfileCredentials $resourceGroupName $webAppName
 
    return ("Basic {0}" -f [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $publishingCredentials.Properties.PublishingUserName, $publishingCredentials.Properties.PublishingPassword))))
}



function Upload-FileToWebApp($resourceGroupName, $webAppName, $slotName = "", $localPath, $kuduPath){

    $kuduApiAuthorisationToken = Get-KuduApiAuthorisationHeaderValue $resourceGroupName $webAppName $slotName

    if ($slotName -eq ""){

        $kuduApiUrl = "https://$webAppName.scm.azurewebsites.net/api/vfs/site/wwwroot/$kuduPath"

    }

    else{

        $kuduApiUrl = "https://$webAppName`-$slotName.scm.azurewebsites.net/api/vfs/site/wwwroot/$kuduPath"

    }

    $virtualPath = $kuduApiUrl.Replace(".scm.azurewebsites.", ".azurewebsites.").Replace("/api/vfs/site/wwwroot", "")

    Write-Host " Uploading File to WebApp. Source: '$localPath'. Target: '$virtualPath'..."  -ForegroundColor DarkGray
    
    Invoke-RestMethod -Uri $kuduApiUrl  -Headers @{"Authorization"=$kuduApiAuthorisationToken;"If-Match"="*"} -Method PUT -InFile $localPath -ContentType "multipart/form-data"

}