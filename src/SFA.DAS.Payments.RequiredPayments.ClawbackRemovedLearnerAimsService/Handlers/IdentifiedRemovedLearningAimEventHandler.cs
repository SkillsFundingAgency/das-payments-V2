using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.RequiredPayments.Application.Processors;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace SFA.DAS.Payments.RequiredPayments.ClawbackRemovedLearnerAimsService.Handlers
{
    public class IdentifiedRemovedLearningAimEventHandler : IHandleMessages<IdentifiedRemovedLearningAim>
    {
        private readonly IApprenticeshipKeyService apprenticeshipKeyService;
        private readonly IClawbackRemovedLearnerAimPaymentsProcessor clawbackRemovedLearnerAimPaymentsProcessor;
        private readonly IPaymentLogger logger;
        private readonly IExecutionContext executionContext;

        public IdentifiedRemovedLearningAimEventHandler(IApprenticeshipKeyService apprenticeshipKeyService, IClawbackRemovedLearnerAimPaymentsProcessor clawbackRemovedLearnerAimPaymentsProcessor, IPaymentLogger logger, IExecutionContext executionContext)
        {
            this.apprenticeshipKeyService = apprenticeshipKeyService ?? throw new ArgumentNullException(nameof(apprenticeshipKeyService));
            this.clawbackRemovedLearnerAimPaymentsProcessor = clawbackRemovedLearnerAimPaymentsProcessor ?? throw new ArgumentNullException(nameof(clawbackRemovedLearnerAimPaymentsProcessor));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
        }

        public async Task Handle(IdentifiedRemovedLearningAim message, IMessageHandlerContext context)
        {
            logger.LogDebug("Processing 'IdentifiedRemovedLearningAim' message.");
            ((ExecutionContext)executionContext).JobId = message.JobId.ToString();

            var calculatedRequiredLevyAmount = await clawbackRemovedLearnerAimPaymentsProcessor.GenerateClawbackForRemovedLearnerAim(message, CancellationToken.None).ConfigureAwait(false);

            logger.LogDebug($"Got {calculatedRequiredLevyAmount?.Count ?? 0} Calculated Required Levy Amount events.");

            if (calculatedRequiredLevyAmount != null)
                await Task.WhenAll(calculatedRequiredLevyAmount.Select(context.Publish)).ConfigureAwait(false);

            logger.LogInfo("Successfully processed IdentifiedRemovedLearningAim event for " +
                           $"jobId:{message.JobId}, learnerRef:{message.Learner.ReferenceNumber}, frameworkCode:{message.LearningAim.FrameworkCode}, " +
                           $"pathwayCode:{message.LearningAim.PathwayCode}, programmeType:{message.LearningAim.ProgrammeType}, " +
                           $"standardCode:{message.LearningAim.StandardCode}, learningAimReference:{message.LearningAim.Reference}, " +
                           $"academicYear:{message.CollectionPeriod.AcademicYear}, contractType:{message.ContractType}");
        }
    }
}