using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsProxyService.Handlers
{
    public class MonthEndEventHandler : IHandleMessages<MonthEndEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IExecutionContext executionContext;
        private readonly IMonthEndEventHandlerService monthEndEventHandlerService;

        public MonthEndEventHandler(IPaymentLogger paymentLogger,
            IExecutionContext executionContext,
            IMonthEndEventHandlerService monthEndEventHandlerService)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
            this.monthEndEventHandlerService = monthEndEventHandlerService;
        }

        public async Task Handle(MonthEndEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Month End Event for Message Id : {context.MessageId}");

            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();

            try
            {
                var monthEndUkprns = await monthEndEventHandlerService.GetMonthEndUkprns(message.CollectionPeriod.Year, message.CollectionPeriod.Month);

                if (monthEndUkprns == null)
                {
                    paymentLogger.LogWarning("No Provider Ukprn found for month end payment");
                    return;
                }

                foreach (var ukprn in monthEndUkprns)
                {
                    await context.SendLocal(new ProcessProviderMonthEndCommand
                    {
                        Ukprn = ukprn,
                        JobId = message.JobId,
                        CollectionPeriod = message.CollectionPeriod.Clone()
                    });
                }

                paymentLogger.LogInfo($"Successfully processed Month End Event for Job Id {message.JobId} and Message Type {message.GetType().Name}");

            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while handling Provider Payments Month End  ProxyService Event ", ex);
                throw;
            }
        }
    }
}
