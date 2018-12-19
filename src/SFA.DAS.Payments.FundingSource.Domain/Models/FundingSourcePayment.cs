using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.Models
{
    public class FundingSourcePayment
    {
        public decimal AmountDue { get; set; }
        public FundingSourceType Type { get; set; }

    }

}