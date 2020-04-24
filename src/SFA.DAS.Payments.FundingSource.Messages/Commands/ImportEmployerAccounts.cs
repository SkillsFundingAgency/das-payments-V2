using System;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.FundingSource.Messages.Commands
{
    public class ImportEmployerAccounts : IPaymentsMessage
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
    }
}