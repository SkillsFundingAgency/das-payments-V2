using System;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class SubmissionSucceededEventHandler : IHandleMessages<SubmissionSucceededEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IExecutionContext executionContext;
        private readonly IPriceEpisodesReceivedService episodesReceivedService;

        public SubmissionSucceededEventHandler(IPaymentLogger logger,
            IExecutionContext executionContext,
            IPriceEpisodesReceivedService episodesReceivedService)
        {
            this.logger = logger;
            this.executionContext = executionContext;
            this.episodesReceivedService = episodesReceivedService;
        }

        public async Task Handle(SubmissionSucceededEvent message, IMessageHandlerContext context)
        {
            var messageType = message.GetType().Name;

            logger.LogInfo($"Processing {messageType} event. Ukprn: {message.Ukprn}");

            ((ESFA.DC.Logging.ExecutionContext) executionContext).JobId = message.JobId.ToString();

            if (message.Ukprn == 0)
                throw new ArgumentException($"Ukprn cannot be 0. Job Id: {message.JobId}");

            var priceEpisodeChangesEvents = await episodesReceivedService
                .JobSucceeded(message.JobId, message.Ukprn)
                .ConfigureAwait(false);

            var tasks = priceEpisodeChangesEvents
                .Select(x => context.Publish(priceEpisodeChangesEvents));

            await Task.WhenAll(tasks).ConfigureAwait(false);

            logger.LogInfo(
                $"Successfully processed {messageType} for Job: {message.JobId}, UKPRN: {message.Ukprn}.");

        }
    }
}