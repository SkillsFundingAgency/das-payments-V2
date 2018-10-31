# Variables
$endpoint = 'localhost:19000'

#Earning Events
.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.EarningEvents.ServiceFabric

#Payments Due
.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.PaymentsDue.ServiceFabric

#Required Payments
.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.RequiredPayments.ServiceFabric

#Funding Source
.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.FundingSource.ServiceFabric

#Provider Payments
.\Remove-ServiceFabricApplicationAndType.ps1 -applicationName SFA.DAS.Payments.ProviderPayments.ServiceFabric