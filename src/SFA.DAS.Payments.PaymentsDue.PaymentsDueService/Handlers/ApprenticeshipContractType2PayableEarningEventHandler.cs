using System;
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
        private readonly IPaymentLogger _paymentLogger;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IAct2PayableEarningService _act2PayableEarningService;

        public ApprenticeshipContractType2PayableEarningEventHandler(
            IAct2PayableEarningService act2PayableEarningService, 
            ILifetimeScope lifetimeScope, 
            IPaymentLogger paymentLogger)
        {
            _act2PayableEarningService = act2PayableEarningService;
            _lifetimeScope = lifetimeScope;
            _paymentLogger = paymentLogger;
        }

        public async Task Handle(ApprenticeshipContractType2EarningEvent message, IMessageHandlerContext context)
        {
            using (_lifetimeScope.BeginLifetimeScope())
            {
                _paymentLogger.LogInfo($"Processing RequiredPaymentsProxyService event. Message Id : {context.MessageId}");

                var executionContext = (ESFA.DC.Logging.ExecutionContext) _lifetimeScope.Resolve<IExecutionContext>();
                executionContext.JobId = message.JobId;

                try
                {
                    ApprenticeshipContractType2PaymentDueEvent[] paymentsDue;

                    try
                    {
                        paymentsDue = await _act2PayableEarningService.CreatePaymentsDue(message).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _paymentLogger.LogError($"Error invoking IAct2PayableEarningService. Error: {ex.Message}", ex);
                        throw;
                    }

                    try
                    {
                        if (paymentsDue != null)
                            await context.Publish(paymentsDue).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _paymentLogger.LogError($"Error publishing the event: 'ApprenticeshipContractType2PaymentDueEvent'.  Error: {ex.Message}.", ex);
                        throw;
                    }

                    _paymentLogger.LogInfo($"Successfully processed ApprenticeshipContractType2EarningEvent event for {message.Ukprn}-{message.Learner.ReferenceNumber}");
                }
                catch (Exception ex)
                {
                    _paymentLogger.LogError($"Error while handling ApprenticeshipContractType2EarningEvent event for {message.Ukprn}-{message.Learner.ReferenceNumber}", ex);
                    throw;
                }
            }
        }
    }
}