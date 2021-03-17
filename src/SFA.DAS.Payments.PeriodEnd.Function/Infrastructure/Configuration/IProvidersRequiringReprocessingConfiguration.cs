namespace SFA.DAS.Payments.PeriodEnd.Function.Infrastructure.Configuration
{
    public interface IProvidersRequiringReprocessingConfiguration
    {
        string PaymentsConnectionString { get; set; }
    }
}