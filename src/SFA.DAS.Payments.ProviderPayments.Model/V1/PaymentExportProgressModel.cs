namespace SFA.DAS.Payments.ProviderPayments.Model.V1
{
    public class PaymentExportProgressModel
    {
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
    }
}
