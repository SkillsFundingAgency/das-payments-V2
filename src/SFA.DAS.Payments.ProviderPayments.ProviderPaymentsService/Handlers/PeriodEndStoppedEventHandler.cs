using System;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

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
            logger.LogInfo($"Processing Month End Period End Stopped Event with Message Id : {context.MessageId}");

            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext) executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();

            var commands = await completionPaymentService.GenerateProviderMonthEndAct1CompletionPaymentCommands(message).ConfigureAwait(false);
            
            if (!commands.Any())
            {
                logger.LogWarning($"No Providers found with Act1 Completion payments for Collection Period: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");
                return;
            }
            
            foreach (var command in commands)
            {
                logger.LogDebug($"Sending Process Provider Month End Act1 Completion Payment Command for provider: {command.Ukprn}");
                await context.SendLocal(command).ConfigureAwait(false);
            }
            
            logger.LogInfo($"Successfully processed Period End Stopped Event for {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear}, job: {message.JobId}");
        }
    }
}