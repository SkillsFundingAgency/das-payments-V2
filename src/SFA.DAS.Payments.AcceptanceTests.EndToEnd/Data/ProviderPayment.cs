using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class ProviderPayment
    {
        public string CollectionPeriod { get; set; }
        private CollectionPeriod parsedCollectionPeriod;

        public CollectionPeriod ParsedCollectionPeriod => parsedCollectionPeriod ??
                                                          (parsedCollectionPeriod = new CollectionPeriodBuilder()
                                                              .WithSpecDate(CollectionPeriod).Build());
        public string DeliveryPeriod { get; set; }
        private CollectionPeriod parsedDeliveryPeriod;
        public CollectionPeriod ParsedDeliveryPeriod => parsedDeliveryPeriod ??
                                                          (parsedDeliveryPeriod = new CollectionPeriodBuilder()
                                                              .WithSpecDate(DeliveryPeriod).Build());
        public decimal SfaCoFundedPayments { get; set; }
        public decimal EmployerCoFundedPayments { get; set; }
        public decimal SfaFullyFundedPayments { get; set; }
        public decimal LevyPayments { get; set; }
        public TransactionType TransactionType { get; set; }
        public string LearnerId { get; set; }
        public long Uln { get; set; }
        public long Ukprn { get; set; }
    }
}