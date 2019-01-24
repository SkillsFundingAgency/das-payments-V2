using System;

namespace SFA.DAS.Payments.Messages.Core.Commands
{
    public interface IPaymentsCommand: IPaymentsMessage
    {
        Guid CommandId { get; set; }
        DateTimeOffset RequestTime { get; set; }

    }
}