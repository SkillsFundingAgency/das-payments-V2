using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Domain;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.Handlers
{
    public class ProcessLearnerHandler: IHandleMessages<ProcessLearnerCommand>
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
            logger.LogDebug($"Handling ILR learner submission. Job: {message.JobId}, Ukprn: {message.Ukprn}, Collection year: {message.CollectionYear}, Learner: {message.Learner.LearnRefNumber}");
            var processorResult = learnerSubmissionProcessor.GenerateEarnings(message);
            if (processorResult.Validation.Failed)
            {
                logger.LogInfo($"ILR Learner Submission failed validation. Job: {message.JobId}, Ukprn: {message.Ukprn}, Collection year: {message.CollectionYear}, Learner: {message.Learner.LearnRefNumber}");
                context.DoNotContinueDispatchingCurrentMessageToHandlers();
                return;
            }

            foreach (var earningEvent in processorResult.EarningEvents)
            {
                await context.Publish(earningEvent).ConfigureAwait(false);
            }

            logger.LogInfo($"Finished handling ILR learner submission.Job: { message.JobId}, Ukprn: { message.Ukprn}, Collection year: { message.CollectionYear}, Learner: { message.Learner.LearnRefNumber}.");
        }
    }
}