using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class PeriodEndStoppedEventHandler : IHandleMessageBatches<PeriodEndStoppedEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IExecutionContext executionContext;
        private readonly ICompletionPaymentService completionPaymentService;
        private readonly IEndpointInstanceFactory endpointInstanceFactory;

        public PeriodEndStoppedEventHandler(IPaymentLogger logger, IExecutionContext executionContext, ICompletionPaymentService completionPaymentService, IEndpointInstanceFactory endpointInstanceFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
            this.completionPaymentService = completionPaymentService ?? throw new ArgumentNullException(nameof(completionPaymentService));
            this.endpointInstanceFactory = endpointInstanceFactory ?? throw new ArgumentNullException(nameof(endpointInstanceFactory));
        }

        public async Task Handle(PeriodEndStoppedEvent message, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Processing Month End Period End Stopped Event.  Message: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");

            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext) executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();

            cancellationToken.ThrowIfCancellationRequested();
            var commands = await completionPaymentService.GenerateProviderMonthEndAct1CompletionPaymentCommands(message).ConfigureAwait(false);
            
            if (!commands.Any())
            {
                logger.LogWarning($"No Providers found with Act1 Completion payments for Collection Period: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");
                return;
            }

            var endpointInstance = await endpointInstanceFactory.GetEndpointInstance();
            foreach (var command in commands)
            {
                cancellationToken.ThrowIfCancellationRequested();
                logger.LogDebug($"Sending Process Provider Month End Act1 Completion Payment Command for provider: {command.Ukprn} for {message.CollectionPeriod.AcademicYear}-R{message.CollectionPeriod.Period:00}");
                await endpointInstance.SendLocal(command).ConfigureAwait(false);
            }
            
            logger.LogInfo($"Successfully processed Period End Stopped Event for {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");
        }

        public async Task Handle(IList<PeriodEndStoppedEvent> messages, CancellationToken cancellationToken)
        {
            foreach (var periodEndStoppedEvent in messages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Handle(periodEndStoppedEvent, cancellationToken);
            }
        }
    }
}