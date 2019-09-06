# Connect to Service Fabric
# Connect-ServiceFabricCluster -ConnectionEndpoint dcol-dsi-servicefabric-weu.westeurope.cloudapp.azure.com:19000 -KeepAliveIntervalInSec 10  -X509Credential -ServerCertThumbprint F0DB178FBABF4F7E7B02E5732521085D0BC4B2AA -FindType FindByThumbprint -FindValue F0DB178FBABF4F7E7B02E5732521085D0BC4B2AA -StoreLocation CurrentUser -StoreName My
Function Remove-SfApp {
    Param (
    [Parameter(Mandatory=$True)]
    [string]$applicationName
    )

    $uri = [System.Uri]"fabric:/$applicationName"
    write-host "Removing..."
    write-host "    Application: $applicationName"
    write-host "    Application uri: $uri"
    $sfApp = Get-ServiceFabricApplication -ApplicationName $uri 
    if ($sfApp) {
        # Remove an application instance
        Remove-ServiceFabricApplication -ApplicationName $uri -Force
     }
    else {
        write-host "Application not found: $applicationName"
    }
}
# Remove an application instance
 Remove-SfApp -ApplicationName SFA.DAS.Payments.Audit.ServiceFabric 
 Remove-SfApp -ApplicationName SFA.DAS.Payments.Monitoring.ServiceFabric 
 Remove-SfApp -ApplicationName SFA.DAS.Payments.RequiredPayments.ServiceFabric 
 Remove-SfApp -ApplicationName SFA.DAS.Payments.ProviderPayments.ServiceFabric 
 Remove-SfApp -ApplicationName SFA.DAS.Payments.FundingSource.ServiceFabric 
 Remove-SfApp -ApplicationName SFA.DAS.Payments.DataLocks.ServiceFabric 
 Remove-SfApp -ApplicationName SFA.DAS.Payments.EarningEvents.ServiceFabric 
 Remove-SfApp -ApplicationName SFA.DAS.Payments.PeriodEnd.ServiceFabric 

