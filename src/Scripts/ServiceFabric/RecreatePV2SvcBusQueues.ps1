
# Usage: 
#    .\RecreatePV2SvcBusQueues.ps1 -Mode pv2-processing|pv2-monitoring|entirenamespace -ResrouceGroupName YourRGName -SubscriptionId YourSubscriptionIdGuid -Namespace YourSBNamespace
#
# -Mode
#      pv2-processing   : Deletes / recreates the nservicebus queues needed for payments v2 processing.
#      pv2-monitoring   : Deletes / recreates the nservicebus queues needed for payments v2 monitoring.
#      entirenamespace  : iterates over all queues in the target namespace, deleting and recreating each of them in turn
#
#
# Note. ensure you have already run Connect-AzureRmAccount to log you powershell session into the Azure account you want.
# $PSVersionTable.PSVersion

param([String]$Mode = "",
      [String]$ResrouceGroupName = "",
      [String]$Namespace = "",
      [String]$SubscriptionId = "",
      [Bool]$RecreateQueues = $True) 


function GetQueueList([string]$mode)
{
    switch ( $mode )
    {
        pv2-processing { $result = "error", "sfa-das-payments-audit-datalock", "sfa-das-payments-audit-datalock-errors", "sfa-das-payments-audit-earningevents", "sfa-das-payments-audit-earningevents-errors", "sfa-das-payments-audit-fundingsource", "sfa-das-payments-audit-fundingsource-errors", "sfa-das-payments-audit-requiredpayments", "sfa-das-payments-audit-requiredpayments-errors", "sfa-das-payments-datalock", "sfa-das-payments-datalock-errors", "sfa-das-payments-datalock-approvals", "sfa-das-payments-datalock-approvals-errors", "sfa-das-payments-datalock-event", "sfa-das-payments-datalock-event-errors", "sfa-das-payments-datalock-status-change", "sfa-das-payments-datalock-status-change-errors", "sfa-das-payments-earningevents", "sfa-das-payments-earningevents-errors", "sfa-das-payments-fundingsource-levy", "sfa-das-payments-fundingsource-levy-errors", "sfa-das-payments-fundingsource-nonlevy", "sfa-das-payments-fundingsource-nonlevy-errors", "sfa-das-payments-levyaccountbalance", "sfa-das-payments-levyaccountbalance-errors", "sfa-das-payments-periodend", "sfa-das-payments-periodend-errors", "sfa-das-payments-providerpayments", "sfa-das-payments-providerpayments-errors", "sfa-das-payments-requiredpayments", "sfa-das-payments-requiredpayments-errors"    }
        pv2-monitoring { $result = "sfa-das-payments-monitoring-jobs", "sfa-das-payments-monitoring-jobs-errors", "sfa-das-payments-monitoring-jobs0", "sfa-das-payments-monitoring-jobs1", "sfa-das-payments-monitoring-jobs10", "sfa-das-payments-monitoring-jobs11", "sfa-das-payments-monitoring-jobs12", "sfa-das-payments-monitoring-jobs13", "sfa-das-payments-monitoring-jobs14", "sfa-das-payments-monitoring-jobs15", "sfa-das-payments-monitoring-jobs16", "sfa-das-payments-monitoring-jobs17", "sfa-das-payments-monitoring-jobs18", "sfa-das-payments-monitoring-jobs19", "sfa-das-payments-monitoring-jobs2", "sfa-das-payments-monitoring-jobs20", "sfa-das-payments-monitoring-jobs3", "sfa-das-payments-monitoring-jobs4", "sfa-das-payments-monitoring-jobs5", "sfa-das-payments-monitoring-jobs6", "sfa-das-payments-monitoring-jobs7", "sfa-das-payments-monitoring-jobs8", "sfa-das-payments-monitoring-jobs9" }
        entirenamespace { $result = Get-AzureRmServiceBusQueue -Namespace $Namespace -ResourceGroupName $ResrouceGroupName | % { $_.name } }
    }

    return $result
}

Select-AzureRmSubscription -SubscriptionId $SubscriptionId

$queues = GetQueueList $Mode

foreach ($queue in $queues) {
    $queue
    Remove-AzureRmServiceBusQueue -Name $queue -Namespace $Namespace -ResourceGroupName $ResrouceGroupName
    if ($RecreateQueues) {
        New-AzureRmServiceBusQueue -Name $queue -Namespace $Namespace -ResourceGroupName $ResrouceGroupName -LockDuration "00:05:00" -MaxDeliveryCount 10 -EnableBatchedOperations
    }
}

