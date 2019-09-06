Function Remove-SfAppsAndTypes {
    Param (
    [Parameter(Mandatory=$false)]
    [string]$applicationFilter = 'SFA.DAS'
    )

    Get-ServiceFabricApplication | ?  { $_.ApplicationName -like "fabric:/$applicationFilter*" } | % {
        $applicationNameUri = $_.ApplicationName.OriginalString
        write-host "Removing..."
        write-host "    Application uri: $applicationNameUri"
        Remove-ServiceFabricApplication -ApplicationName $applicationNameUri -Force
    }

    Get-ServiceFabricApplication | ?  { $_.ApplicationTypeName -like "$applicationFilter*" } | % {
        $applicationTypeName = $_.ApplicationTypeName
        write-host "Removing..."
        write-host "    Application Type Name: $applicationTypeName"

        Get-ServiceFabricApplicationType -ApplicationTypeName $applicationTypeName | % {
            $applicationTypeVersion  = $_.ApplicationTypeVersion
            Unregister-ServiceFabricApplicationType -ApplicationTypeName $applicationTypeName  -ApplicationTypeVersion $applicationTypeVersion -Force
        }
    }
}

Remove-SfAppsAndTypes
