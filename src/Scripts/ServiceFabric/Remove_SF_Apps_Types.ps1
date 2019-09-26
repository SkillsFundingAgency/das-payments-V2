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

    Get-ServiceFabricApplicationType | ?  { $_.ApplicationTypeName -like "$applicationFilter*" } | % {
        $applicationTypeName = $_.ApplicationTypeName
        $applicationTypeVersion  = $_.ApplicationTypeVersion
        write-host "Removing... $applicationTypeName : $applicationTypeVersion " 
        Unregister-ServiceFabricApplicationType -ApplicationTypeName $applicationTypeName -ApplicationTypeVersion $applicationTypeVersion -Force
    }
}

Remove-SfAppsAndTypes
