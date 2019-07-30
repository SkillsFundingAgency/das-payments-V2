using System;

namespace SFA.DAS.Payments.Messages.Core.Commands
{
    public interface ICommand: IPaymentsMessage
    {
        Guid CommandId { get; }
    }
}