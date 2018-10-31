[CmdletBinding()]
Param(
    [Parameter(Mandatory=$False)]
    [string]$endpoint = 'localhost:19000',

    [Parameter(Mandatory=$True)]
    [string]$applicationName,

    [Parameter(Mandatory=$False)]
    [string]$applicationTypeVersion = '1.0.0'

)

if (-not $connection){
    write-verbose "Connection to local service fabric cluster: $endpoint"
    Connect-ServiceFabricCluster -ConnectionEndpoint $endpoint
}
    

$uri = [System.Uri]"fabric:/$applicationName"
$applicationUri = $uri.ToString()
$applicationTypeName = $applicationName+"Type"
write-verbose "Application Name: $applicationName"
write-verbose "Application Type Name: $applicationTypeName"
write-verbose "Application uri: $applicationUri"

$sfApp = Get-ServiceFabricApplication -ApplicationName $uri
if ($sfApp) {
    # Remove an application instance
    Remove-ServiceFabricApplication -ApplicationName $uri -Force
}
else {
    write-host "Application not found."
}

if ((Get-ServiceFabricApplication -ApplicationTypeName $applicationTypeName)) {
    # Unregister the application type
    Unregister-ServiceFabricApplicationType -ApplicationTypeName $applicationTypeName  -ApplicationTypeVersion $applicationTypeVersion -Force
}
else {
    write-host "Application type not found."
}
