using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Messages.Internal;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Handlers
{
    public class PublishDeferredApprovalEventsCommandHandler : IHandleMessages<PublishDeferredApprovalEventsCommand>
    {
        private readonly IPaymentLogger logger;
        private readonly IContainerScopeFactory factory;

        public const string DeferredMessageIdHeader = "DeferredMessageId";

        public PublishDeferredApprovalEventsCommandHandler(IPaymentLogger logger, IContainerScopeFactory factory)
        {
            this.logger = logger;
            this.factory = factory;
        }

        public async Task Handle(PublishDeferredApprovalEventsCommand message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogVerbose("Handling PeriodEndStoppedEvent");

                using(var scope = factory.CreateScope())
                while (true)
                {
                    logger.LogVerbose("Getting deferred events");

                    var deferredApprovalsEventRepository = scope.Resolve<IDeferredApprovalsEventRepository>();
                    var deferredEvents = await deferredApprovalsEventRepository.GetDeferredEvents(CancellationToken.None).ConfigureAwait(false);
                    if (deferredEvents.Count == 0)
                        break;

                    logger.LogVerbose($"Sending {deferredEvents.Count} deferred approval events back to queue");

                    foreach (var deferredEvent in deferredEvents)
                    {
                        var sendOptions = new SendOptions();
                        sendOptions.SetHeader(DeferredMessageIdHeader, deferredEvent.Id.ToString());

                        await context.SendLocal(deferredEvent.ApprovalsEvent).ConfigureAwait(false);
                    }

                    var eventIds = deferredEvents.Select(e => e.Id).ToList();

                    logger.LogVerbose($"Deleting {eventIds.Count} deferred approval events from Db");

                    await deferredApprovalsEventRepository.DeleteDeferredEvents(eventIds, CancellationToken.None).ConfigureAwait(false);

                    logger.LogInfo($"Re-published {eventIds.Count} deferred approval events and deleted from Db");
                }

                logger.LogInfo("Finished Handling PeriodEndStoppedEvent");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling PeriodEndStoppedEvent message. Error: {ex.Message}", ex);
                throw;
            }
        }
    }
}