using System;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Application;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.PaymentsDueService.Handlers
{
    public class FunctionalSkillsEarningEventHandler : IHandleMessages<FunctionalSkillEarningsEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IExecutionContext executionContext;
        private readonly IFunctionalSkillsEarningService functionalSkillsEarningService;

        public FunctionalSkillsEarningEventHandler(IFunctionalSkillsEarningService functionalSkillsEarningService, IPaymentLogger paymentLogger, IExecutionContext executionContext)
        {
            this.paymentLogger = paymentLogger;
            this.executionContext = executionContext;
            this.functionalSkillsEarningService = functionalSkillsEarningService;
        }

        public async Task Handle(FunctionalSkillEarningsEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing {typeof(FunctionalSkillEarningsEvent).Name} event. Message Id : {context.MessageId}");

            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();

            try
            {
                IncentivePaymentDueEvent[] paymentsDue;

                try
                {
                    paymentsDue = functionalSkillsEarningService.CreatePaymentsDue(message);
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