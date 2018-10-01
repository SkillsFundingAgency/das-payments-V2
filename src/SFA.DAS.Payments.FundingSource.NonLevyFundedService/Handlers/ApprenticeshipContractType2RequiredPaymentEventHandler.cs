using Autofac;
using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.NonLevyFundedService.Handlers
{
    public class ApprenticeshipContractType2RequiredPaymentEventHandler : IHandleMessages<ApprenticeshipContractType2RequiredPaymentEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly ILifetimeScope lifetimeScope;
        private readonly IContractType2RequiredPaymentEventFundingSourceService contractType2RequiredPaymentService;

        public ApprenticeshipContractType2RequiredPaymentEventHandler(IPaymentLogger paymentLogger, 
                                                                     ILifetimeScope lifetimeScope, 
                                                                      IContractType2RequiredPaymentEventFundingSourceService contractType2RequiredPaymentService)
        {
            this.paymentLogger = paymentLogger;
            this.lifetimeScope = lifetimeScope;
            this.contractType2RequiredPaymentService = contractType2RequiredPaymentService;
        }

        public async Task Handle(ApprenticeshipContractType2RequiredPaymentEvent message, IMessageHandlerContext context)
        {
            using (var scope = lifetimeScope.BeginLifetimeScope())
            {
                paymentLogger.LogInfo($"Processing Required Payment Service event for Message Id : {context.MessageId}");

                var executionContext = (ESFA.DC.Logging.ExecutionContext)scope.Resolve<IExecutionContext>();
                executionContext.JobId = message.JobId;

                try
                {
                    var payments = contractType2RequiredPaymentService.GetFundedPayments(message);

                    foreach (var recordablePaymentEvent in payments)
                    {
                        try
                        {
                            await context.Publish(recordablePaymentEvent);

                            paymentLogger.LogInfo($"Successfully published CoInvestedPayment of Type  {recordablePaymentEvent.GetType().Name}");
                        }
                        catch (Exception ex)
                        {
                            paymentLogger.LogError($"Error publishing the event: RecordablePaymentEvent", ex);
                            throw;
                        }
                    }

                    paymentLogger.LogInfo($"Successfully processed NonLevyFunded Service event for Job Id {message.JobId}");
                }
                catch (Exception ex)
                {
                    paymentLogger.LogError($"Error while handling NonLevyFundedService event", ex);
                    throw;
                }
            }
        }
    }
}