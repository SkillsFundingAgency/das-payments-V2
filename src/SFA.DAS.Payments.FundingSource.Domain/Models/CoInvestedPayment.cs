using SFA.DAS.Payments.FundingSource.Domain.Enum;

namespace SFA.DAS.Payments.FundingSource.Domain.Models
{
    public class CoInvestedPayment
    {
        public decimal AmountDue { get; set; }

        public virtual FundingSourceType Type { get; set; }

        public byte ContractType { get; set; }

    }

}