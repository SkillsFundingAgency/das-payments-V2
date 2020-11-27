using System;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class PeriodEndStoppedEventHandler : IHandleMessages<PeriodEndStoppedEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IExecutionContext executionContext;
        private readonly ICompletionPaymentService completionPaymentService;

        public PeriodEndStoppedEventHandler(IPaymentLogger logger, IExecutionContext executionContext, ICompletionPaymentService completionPaymentService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
            this.completionPaymentService = completionPaymentService ?? throw new ArgumentNullException(nameof(completionPaymentService));
        }

        public async Task Handle(PeriodEndStoppedEvent message, IMessageHandlerContext context)
        {
            logger.LogInfo($"Processing Month End Event for Message Id : {context.MessageId}");

            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext) executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();

            var commands = await completionPaymentService.GenerateProviderMonthEndAct1CompletionPaymentCommands(message).ConfigureAwait(false);
            
            if (!commands.Any())
            {
                logger.LogWarning($"No Provider Ukprn found for period end payment {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");
                return;
            }
            
            foreach (var command in commands)
            {
                logger.LogDebug($"Sending month end command for provider: {command.Ukprn}");
                await context.SendLocal(command).ConfigureAwait(false);
            }
            
            logger.LogInfo($"Successfully processed Period End Event for {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");
            
        }
    }
}