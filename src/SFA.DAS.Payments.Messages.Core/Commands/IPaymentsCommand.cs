using System;

namespace SFA.DAS.Payments.Messages.Core.Commands
{
    public interface IPaymentsCommand: IJobMessage, ICommand
    {
        DateTimeOffset RequestTime { get; }
    }
}