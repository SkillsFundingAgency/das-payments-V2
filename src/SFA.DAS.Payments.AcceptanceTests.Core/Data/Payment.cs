using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public abstract class Payment
    {
        public TransactionType Type => Helper.GetTransactionType(TransactionType);

        public FundingSourceType FundingSourceType => Helper.GetFundingSourceType(FundingSource);


        public string TransactionType { get; set; }
        public string FundingSource { get; set; }
        public byte DeliveryPeriod { get; set; }
        public decimal Amount { get; set; }
    }
}
