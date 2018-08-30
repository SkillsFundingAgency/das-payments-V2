using Autofac;
using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.NonLevyFundedService.Handlers
{
    public class RequiredPaymentEventHandler : IHandleMessages<RequiredPaymentEvent>
    {
        private readonly IPaymentLogger PaymentLogger;
        private readonly ILifetimeScope LifetimeScope;

        public RequiredPaymentEventHandler(IPaymentLogger paymentLogger, ILifetimeScope lifetimeScope)
        {
            PaymentLogger = paymentLogger;
            LifetimeScope = lifetimeScope;
        }

        public async Task Handle(RequiredPaymentEvent message, IMessageHandlerContext context)
        {
            using (var scope = LifetimeScope.BeginLifetimeScope())
            {
                PaymentLogger.LogInfo($"Processing CalculatedPaymentDueEvent Service event. Message Id : {context.MessageId}");

                var executionContext = (ESFA.DC.Logging.ExecutionContext)scope.Resolve<IExecutionContext>();
                executionContext.JobId = message.JobId;

                try
                {
                    //TODO Logic to generate Payments
                    var payments = new List<RecordablePaymentEvent>
                    {
                        new RecordablePaymentEvent
                        {
                            JobId =  message.JobId,
                            EventTime = DateTime.UtcNow
                         }
                    };

                    foreach (var recordablePaymentEvent in payments)
                    {
                        try
                        {
                            await context.Publish(recordablePaymentEvent);
                        }
                        catch (Exception ex)
                        {
                            //TODO: add more details when we flesh out the event.
                            PaymentLogger.LogError($"Error publishing the event: RecordablePaymentEvent", ex);
                            throw;
                            //TODO: update the job
                        }
                    }

                    PaymentLogger.LogInfo($"Successfully processed NonLevyFunded Service event for Actor Id {message.JobId}");
                }
                catch (Exception ex)
                {
                    PaymentLogger.LogError($"Error while handling NonLevyFundedService event", ex);
                    throw;
                }
            }
        }
    }
}