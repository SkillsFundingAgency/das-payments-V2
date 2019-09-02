using System;

namespace SFA.DAS.Payments.DataLocks.Messages.Internal
{
    public class PublishDeferredApprovalEventsCommand : Payments.Messages.Core.Commands.ICommand
    {
        public Guid CommandId { get; set; }
    }
}
