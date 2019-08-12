using System;

namespace SFA.DAS.Payments.ProviderPayments.Model.V1
{
    public class LegacyPaymentModel
    {
        public Guid PaymentId { get; set; }
        public Guid RequiredPaymentId { get; set; }
        public int DeliveryMonth { get; set; }
        public int DeliveryYear { get; set; }
        public string CollectionPeriodName { get; set; }
        public int CollectionPeriodMonth { get; set; }
        public int CollectionPeriodYear { get; set; }
        public int FundingSource { get; set; }
        public int TransactionType { get; set; }
        public decimal? Amount { get; set; }
    }
}
