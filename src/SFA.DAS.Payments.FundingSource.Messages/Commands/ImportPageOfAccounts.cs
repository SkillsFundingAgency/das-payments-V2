using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.FundingSource.Messages.Commands
{
    public class ImportPageOfAccounts : IPaymentsMessage
    {
        public int PageNumber { get; set; }
    }
}