using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Domain;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.Handlers
{
    public class ProcessLearnerHandler : IHandleMessages<ProcessLearnerCommand>
    {
        private readonly ILearnerSubmissionProcessor learnerSubmissionProcessor;
        private readonly IPaymentLogger logger;

        public ProcessLearnerHandler(ILearnerSubmissionProcessor learnerSubmissionProcessor, IPaymentLogger logger)
        {
            this.learnerSubmissionProcessor = learnerSubmissionProcessor ?? throw new ArgumentNullException(nameof(learnerSubmissionProcessor));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ProcessLearnerCommand message, IMessageHandlerContext context)
        {
            Guid correlationId = Guid.NewGuid();
            string jobInfoString =
                $"Job: {message.JobId}, Ukprn: {message.Ukprn}, Collection year: {message.CollectionYear}, Learner: {message.Learner.LearnRefNumber}, CorrelationId: {correlationId}";
            logger.LogWarning($"Handling ILR learner submission. {jobInfoString}");
            var processorResult = learnerSubmissionProcessor.GenerateEarnings(message);
            logger.LogWarning($"Processed {nameof(learnerSubmissionProcessor)}.GenerateEarnings. {jobInfoString}");
            if (processorResult.Validation.Failed)
            {
                logger.LogWarning($"ILR Learner Submission failed validation. {jobInfoString}");
                context.DoNotContinueDispatchingCurrentMessageToHandlers();
                return;
            }

            foreach (var earningEvent in processorResult.EarningEvents)
            {
                logger.LogWarning($"Publishing event for earning event {earningEvent.EventId}. Correlation id: {correlationId}");
                await context.Publish(earningEvent).ConfigureAwait(false);
            }
            var summary = string.Join(", ", processorResult.EarningEvents.GroupBy(e => e.GetType().Name).Select(g => $"{g.Key}: {g.Count()}"));
            logger.LogWarning($"Finished handling ILR learner submission. {jobInfoString}. Published events: {summary}");
        }
    }
}