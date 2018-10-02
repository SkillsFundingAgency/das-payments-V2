using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Application;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.PaymentsDueService.Handlers
{
    public class ApprenticeshipContractType2PayableEarningEventHandler : IHandleMessages<ApprenticeshipContractType2EarningEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly ILifetimeScope lifetimeScope;
        private readonly IApprenticeshipContractType2PayableEarningService act2PayableEarningService;

        public ApprenticeshipContractType2PayableEarningEventHandler(
            IApprenticeshipContractType2PayableEarningService act2PayableEarningService, 
            ILifetimeScope lifetimeScope, 
            IPaymentLogger paymentLogger)
        {
            this.act2PayableEarningService = act2PayableEarningService;
            this.lifetimeScope = lifetimeScope;
            this.paymentLogger = paymentLogger;
        }

        public async Task Handle(ApprenticeshipContractType2EarningEvent message, IMessageHandlerContext context)
        {
            using (lifetimeScope.BeginLifetimeScope())
            {
                paymentLogger.LogInfo($"Processing {typeof(ApprenticeshipContractType2EarningEvent).Name} event. Message Id : {context.MessageId}");

                var executionContext = (ESFA.DC.Logging.ExecutionContext) lifetimeScope.Resolve<IExecutionContext>();
                executionContext.JobId = message.JobId;

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
}