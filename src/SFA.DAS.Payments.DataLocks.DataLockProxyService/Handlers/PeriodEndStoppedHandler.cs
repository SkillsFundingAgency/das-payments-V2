using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Infrastructure;
using SFA.DAS.Payments.DataLocks.Messages.Internal;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class PeriodEndStoppedHandler : IHandleMessages<PeriodEndStoppedEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IDasMessageSessionFactory dasMessageSessionFactory;

        public PeriodEndStoppedHandler(IPaymentLogger logger, IDasMessageSessionFactory dasMessageSessionFactory)
        {
            this.logger = logger;
            this.dasMessageSessionFactory = dasMessageSessionFactory;
        }

        public async Task Handle(PeriodEndStoppedEvent message, IMessageHandlerContext context)
        {
            logger.LogInfo("Received period end stopped event. Forwarding to DAS service bus namespace");
            var session = dasMessageSessionFactory.Create();
            await session.Send(new PublishDeferredApprovalEventsCommand {CommandId = Guid.NewGuid()}).ConfigureAwait(false);
        }
    }
}