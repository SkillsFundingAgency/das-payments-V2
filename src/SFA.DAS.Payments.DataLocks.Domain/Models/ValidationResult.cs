namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class ValidationResult
    {
        public DataLockErrorCode? DataLockErrorCode { get; set; }

        public long ApprenticeshipId { get; set; }

        public byte Period { get; set; }

        public long ApprenticeshipPriceEpisodeIdentifier { get; set; }
    }
}