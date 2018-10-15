using   SFA.DAS.Payments.FundingSource.Model.Enum;

namespace SFA.DAS.Payments.FundingSource.Domain.Models
{
    public class CoInvestedPayment
    {
        public decimal AmountDue { get; set; }

        public FundingSourceType Type { get; set; }

    }

}