using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Data
{
    public class RecordedPayment
    {
        public string TransactionType { get; set; }


        public byte DeliveryPeriod { get; set; }

        public ContractType ContractType { get; set; }

        public decimal Amount { get; set; }

        public string CollectionPeriod { get; set; }

        public CollectionPeriod ParsedCollectionPeriod => new CollectionPeriodBuilder()
                                                              .WithSpecDate(this.CollectionPeriod).Build();
    }
}