using SFA.DAS.Payments.FundingSource.Domain.Enum;

namespace SFA.DAS.Payments.FundingSource.Domain.Models
{
    public class Payment
    {
        public decimal AmountDue { get; set; }

        public FundingSourceType Type { get; set; }

    }
}