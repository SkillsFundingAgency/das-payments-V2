<#
    .SYNOPSIS
    Disables or enables triggers associated with a Data Factory.

    .DESCRIPTION
    Disables or enables triggers associated with a Data Factory allowing for safe release.

    .PARAMETER DataFactoryName
    The name of the Data Factory to run the script against.

    .PARAMETER ResourceGroupName
    The name of the Resource Group hosting the Data Factory.

    .PARAMETER TriggerState
    The state in which to set the triggers, either enable or disable.

    .EXAMPLE
    Set-AzDataFactoryTriggerState -DataFactoryName aDataFactory -ResourceGroupName aResourceGroup -TriggerState disable
    Set-AzDataFactoryTriggerState -DataFactoryName aDataFactory -ResourceGroupName aResourceGroup -TriggerState enable
#>
Param (
    [Parameter(Mandatory = $true)]
    [ValidateNotNull()]
    [String]$DataFactoryName,
    [Parameter(Mandatory = $true)]
    [ValidateNotNull()]
    [String]$ResourceGroupName,
    [Parameter(Mandatory = $true)]
    [ValidateSet("enable", "disable")]
    [String]$TriggerState
)

try {
    $ResourceGroupExists = Get-AzResourceGroup $ResourceGroupName
    if (!$ResourceGroupExists) {
        throw "Resource Group $ResourceGroupName does not exist."
    }

    $DataFactoryExists = Get-AzDataFactoryV2 -ResourceGroupName $ResourceGroupName -Name $DataFactoryName
    if (!$DataFactoryExists) {
        throw "The Data Factory $DataFactoryName in Resource Group $ResourceGroupName Does not exists."
    }

    $Triggers = Get-AzDataFactoryV2Trigger -DataFactoryName $DataFactoryName -ResourceGroupName  $ResourceGroupName
    if (!$Triggers) {
        Write-Output  "No Triggers Associated with Datafactory $DataFactoryName"
    }
    else {

        switch ($TriggerState) {
            "enable" {
                foreach ($Trigger in $Triggers) {
                    Start-AzDataFactoryV2Trigger -ResourceGroupName $ResourceGroupName -DataFactoryName $DataFactoryName -Name $Trigger.name -Force
                }
                break
            }
            "disable" {
                foreach ($Trigger in $Triggers) {
                    Stop-AzDataFactoryV2Trigger -ResourceGroupName $ResourceGroupName -DataFactoryName $DataFactoryName -Name $Trigger.name -Force
                }
                break
            }
        }
    }
}

catch {
    throw "$_"
}
