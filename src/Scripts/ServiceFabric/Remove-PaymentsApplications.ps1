# Variables
$endpoint = 'localhost:19000'

#Earning Events
.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.EarningEvents.ServiceFabric -Verbose

#Payments Due
.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.PaymentsDue.ServiceFabric -Verbose

#Required Payments
.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.RequiredPayments.ServiceFabric -Verbose

#Funding Source 
.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.FundingSource.ServiceFabric -Verbose

#Provider Payments
.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.ProviderPayments.ServiceFabric -Verbose

#Data Lock
.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.DataLocks.ServiceFabric -Verbose

#Audit
.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.Audit.ServiceFabric -Verbose

#Monitoring
.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.Monitoring.ServiceFabric -Verbose

#PeriodEnd
.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.PeriodEnd.ServiceFabric -Verbose

#Get-ServiceFabricApplicationType  -ApplicationTypeName SFA.DAS.Payments.FundingSource.ServiceFabricType 
