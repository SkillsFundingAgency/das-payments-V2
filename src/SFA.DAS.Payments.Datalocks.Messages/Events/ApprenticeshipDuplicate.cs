namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class ApprenticeshipDuplicate
    {
        public long ApprenticeshipId { get; set; }
        public long Ukprn { get; set; }
    }
}