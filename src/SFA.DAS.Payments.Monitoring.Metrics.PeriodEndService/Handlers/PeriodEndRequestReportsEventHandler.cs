using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Metrics.Application.PeriodEnd;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.Metrics.PeriodEndService.Handlers
{
   public class PeriodEndRequestReportsEventHandler : IHandleMessages<PeriodEndRequestReportsEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IPeriodEndMetricsService periodEndMetricsService;

        public PeriodEndRequestReportsEventHandler(IPaymentLogger logger, IPeriodEndMetricsService periodEndMetricsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndMetricsService = periodEndMetricsService ?? throw new ArgumentNullException(nameof(periodEndMetricsService));
        }

        public async Task Handle(PeriodEndRequestReportsEvent message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Handling PeriodEndRequestReportsEvent for monitoring metrics period end service. Message: {message.ToJson()}");

            await periodEndMetricsService.BuildMetrics(message.JobId, message.CollectionPeriod.AcademicYear,
                message.CollectionPeriod.Period,  CancellationToken.None);

            logger.LogInfo($"Handled PeriodEndRequestReportsEvent for monitoring metrics period end service. Message: {message.ToJson()}");
        }
    }
}
