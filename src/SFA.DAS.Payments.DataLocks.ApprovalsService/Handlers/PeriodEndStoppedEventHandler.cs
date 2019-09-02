using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Handlers
{
    public class PeriodEndStoppedEventHandler : IHandleMessages<PeriodEndStoppedEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IDeferredApprovalsEventRepository deferredApprovalsEventRepository;

        public const string DeferredMessageIdHeader = "DeferredMessageId";

        public PeriodEndStoppedEventHandler(IPaymentLogger logger, IDeferredApprovalsEventRepository deferredApprovalsEventRepository)
        {
            this.logger = logger;
            this.deferredApprovalsEventRepository = deferredApprovalsEventRepository;
        }

        public async Task Handle(PeriodEndStoppedEvent message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogVerbose("Handling PeriodEndStoppedEvent");

                while (true)
                {
                    logger.LogVerbose("Getting deferred events");

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