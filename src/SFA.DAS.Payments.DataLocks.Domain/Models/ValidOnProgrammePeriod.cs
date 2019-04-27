namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class ValidOnProgrammePeriod : OnProgrammePeriodValidationResult
    {
        public long ApprenticeshipPriceEpisodeId { get; set; }
    }
}