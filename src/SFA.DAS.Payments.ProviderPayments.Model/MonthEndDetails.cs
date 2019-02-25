namespace SFA.DAS.Payments.ProviderPayments.Model
{
    public class MonthEndDetails
    {
        public long Ukprn { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public long JobId { get; set; }
    }
}