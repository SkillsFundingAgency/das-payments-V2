using System.Net.Sockets;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class ProviderPayment
    {
        public string CollectionPeriod { get; set; }

        public CollectionPeriod ParsedCollectionPeriod => new CollectionPeriodBuilder()
                                                              .WithSpecDate(this.CollectionPeriod).Build();
        public string DeliveryPeriod { get; set; }
        public CollectionPeriod ParsedDeliveryPeriod => new CollectionPeriod
        {
            Period = new DeliveryPeriodBuilder().WithSpecDate(DeliveryPeriod).Build(),
            AcademicYear = ParsedCollectionPeriod.AcademicYear
        };
        public decimal SfaCoFundedPayments { get; set; }
        public decimal EmployerCoFundedPayments { get; set; }
        public decimal SfaFullyFundedPayments { get; set; }
        public decimal LevyPayments { get; set; }
        public decimal TransferPayments { get; set; }
        public TransactionType TransactionType { get; set; }
        public string LearnerId { get; set; }
        public long Uln { get; set; }
        public string Employer { get; set; }
        public long? AccountId { get; set; }
        public int? StandardCode { get; set; }
        public string SendingEmployer { get; set; }
        public long? SendingAccountId { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public bool IsEmployerLevyPayer { get; set; }
    }
}