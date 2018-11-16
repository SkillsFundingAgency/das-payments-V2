using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class ProviderPayment
    {
        public string CollectionPeriod { get; set; }
        public string DeliveryPeriod { get; set; }
        public decimal SfaCoFundedPayments { get; set; }
        public decimal EmployerCoFundedPayments { get; set; }
        public TransactionType TransactionType { get; set; }
        public string LearnerId { get; set; }
    }
}