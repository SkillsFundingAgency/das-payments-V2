using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class InvalidatedPayableEarningsHandler : IHandleMessages<InvalidatedPayableEarningEvent>
    {
        private readonly ITelemetry telemetry;
        private readonly IPaymentLogger paymentLogger;

        public InvalidatedPayableEarningsHandler(ITelemetry telemetry, IPaymentLogger paymentLogger)
        {
            this.telemetry = telemetry;
            this.paymentLogger = paymentLogger;
        }

        public async Task Handle(InvalidatedPayableEarningEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing InvalidatedPayableEarningEvent. Message Id : {context.MessageId}, Earning Event Id: {message.LastEarningEventId}");

            using (var operation = telemetry.StartOperation())
            {
                try
                {
                    await Task.WhenAll(message.AccountIds.Select(accountId => context.Publish(new RemoveObsoletePayments
                     {
                         AccountId = accountId,
                         LastEarningEventId = message.LastEarningEventId,
                         LastDataLockEventId = message.LastDataLockEventId,
                     }))).ConfigureAwait(false);

                    telemetry.StopOperation(operation);
                }
                catch (Exception ex)
                {
                    paymentLogger.LogError($"Error processing InvalidatedPayableEarningEvent For Earning Event Id: {message.LastEarningEventId}. Error: {ex}", ex);
                    throw;
                }
            }
        }
    }
}