using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Application;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.PaymentsDue.PaymentsDueService.Handlers
{
    public class ApprenticeshipContractType2PayableEarningEventHandler : IHandleMessages<ApprenticeshipContractType2EarningEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IExecutionContext executionContext;
        private readonly IApprenticeshipContractType2PayableEarningService act2PayableEarningService;

        public ApprenticeshipContractType2PayableEarningEventHandler(
            IApprenticeshipContractType2PayableEarningService act2PayableEarningService,
            IExecutionContext executionContext,
            IPaymentLogger paymentLogger)
        {
            this.act2PayableEarningService = act2PayableEarningService;
            this.executionContext = executionContext;
            this.paymentLogger = paymentLogger;
        }

        public async Task Handle(ApprenticeshipContractType2EarningEvent message, IMessageHandlerContext context)
        {

            paymentLogger.LogInfo($"Processing {typeof(ApprenticeshipContractType2EarningEvent).Name} event. Message Id : {context.MessageId}");

            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
            currentExecutionContext.JobId = message.JobId;

            try
            {
                ApprenticeshipContractType2PaymentDueEvent[] paymentsDue;

                try
                {
                    paymentsDue = act2PayableEarningService.CreatePaymentsDue(message);
                }
                catch (Exception ex)
                {
                    paymentLogger.LogError($"Error invoking {typeof(IApprenticeshipContractType2PayableEarningService).Name}. Error: {ex.Message}", ex);
                    throw;
                }

                try
                {
                    if (paymentsDue != null)
                        await Task.WhenAll(paymentsDue.Select(context.Publish)).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    paymentLogger.LogError($"Error publishing the event: {typeof(ApprenticeshipContractType2PaymentDueEvent).Name}.  Error: {ex.Message}.", ex);
                    throw;
                }

                paymentLogger.LogInfo($"Successfully processed {typeof(ApprenticeshipContractType2EarningEvent)} event for {message.Ukprn}-{message.Learner.ReferenceNumber}");
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while handling {typeof(ApprenticeshipContractType2EarningEvent)} event for {message.Ukprn}-{message.Learner.ReferenceNumber}", ex);
                throw;
            }

        }
    }
}