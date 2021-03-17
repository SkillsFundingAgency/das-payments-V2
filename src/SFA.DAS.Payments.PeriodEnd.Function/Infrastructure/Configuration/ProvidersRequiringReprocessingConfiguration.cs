namespace SFA.DAS.Payments.PeriodEnd.Function.Infrastructure.Configuration
{
    public class ProvidersRequiringReprocessingConfiguration : IProvidersRequiringReprocessingConfiguration
    {
        public string PaymentsConnectionString { get; set; }
    }
}