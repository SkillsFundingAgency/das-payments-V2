using System;
using SFA.DAS.Payments.Messages.Common;

namespace SFA.DAS.Payments.FundingSource.Messages.Commands
{
    public class ImportEmployerAccounts : IPaymentsMessage
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
    }
}