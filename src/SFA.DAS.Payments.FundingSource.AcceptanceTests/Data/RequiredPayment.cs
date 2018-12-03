using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Data
{
    public class RequiredPayment
    {
        public string PriceEpisodeIdentifier { get; set; }

        public byte Period { get; set; }

        public byte DeliveryPeriod { get; set; }

        public TransactionType Type => EnumHelper.ToTransactionType(TransactionType);

        public string TransactionType { get; set; }

        public decimal Amount { get; set; }
    }

    
}
