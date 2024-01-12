using SFA.DAS.Payments.Messages.Common;

namespace SFA.DAS.Payments.FundingSource.Messages.Commands
{
    public class ImportPageOfAccounts : IPaymentsMessage
    {
        public int PageNumber { get; set; }
    }
}