using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class InvalidatedPayableEarningsHandler : IHandleMessages<InvalidatedPayableEarningEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IProviderPaymentFactory paymentFactory;
        private readonly IProviderPaymentsService providerPaymentsService;

        public InvalidatedPayableEarningsHandler(IPaymentLogger paymentLogger, IProviderPaymentFactory paymentFactory, IProviderPaymentsService providerPaymentsService)
        {
            this.paymentLogger = paymentLogger;
            this.paymentFactory = paymentFactory;
            this.providerPaymentsService = providerPaymentsService;
        }

        public async Task Handle(InvalidatedPayableEarningEvent message, IMessageHandlerContext context)
        {
            var messageType = message.GetType().Name;

            try
            {
                paymentLogger.LogDebug($"Processing {messageType} for Event Id : {message.LastEarningEventId}");

                await providerPaymentsService.RemoveObsoletePayments(message, default(CancellationToken));
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while handling {messageType}. Error: {ex.Message}, Job: {message.LastEarningEventId}", ex);
                throw;
            }

            paymentLogger.LogDebug($"Finished processing {messageType} for Event Id : {message.LastEarningEventId}. ");

        }
    }
}