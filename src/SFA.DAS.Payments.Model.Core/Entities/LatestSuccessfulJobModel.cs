namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class LatestSuccessfulJobModel
    {
        public long JobId { get; set; }
        public long Ukprn { get; set; }
        public long DcJobId { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
    }
}