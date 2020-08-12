using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.Metrics.PeriodEndService.Handlers
{
   public class PeriodEndRunningEventHandler : IHandleMessages<PeriodEndRequestReportsEvent>
    {
        private readonly IPaymentLogger logger;

        public PeriodEndRunningEventHandler(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(PeriodEndRequestReportsEvent message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Handling PeriodEndRequestReportsEvent for monitoring metrics period end service. Message: {message.ToJson()}");



            logger.LogInfo($"Handled PeriodEndRequestReportsEvent for monitoring metrics period end service. Message: {message.ToJson()}");
            return Task.CompletedTask;;
        }
    }
}
