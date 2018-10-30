# Variables
$endpoint = 'localhost:19000'
#$thumbprint = '2779F0BB9A969FB88E04915FFE7955D0389DA7AF'
#$packagepath="C:\Users\sfuser\Documents\Visual Studio 2017\Projects\MyApplication\MyApplication\pkg\Release"

# Connect to the cluster using a client certificate.
Connect-ServiceFabricCluster -ConnectionEndpoint $endpoint 
#`
#          -KeepAliveIntervalInSec 10 `
#          -X509Credential -ServerCertThumbprint $thumbprint `
#          -FindType FindByThumbprint -FindValue $thumbprint `
#          -StoreLocation CurrentUser -StoreName My

# Get Application Type


# Remove an application instance
Remove-ServiceFabricApplication -ApplicationName fabric:/SFA.DAS.Payments.EarningEvents.ServiceFabric -Force

# Unregister the application type
Unregister-ServiceFabricApplicationType -ApplicationTypeName SFA.DAS.Payments.EarningEvents.ServiceFabricType  -ApplicationTypeVersion 1.0.0 -Force

