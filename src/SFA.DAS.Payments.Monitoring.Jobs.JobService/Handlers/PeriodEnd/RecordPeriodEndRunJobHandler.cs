using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.PeriodEnd
{
    public class RecordPeriodEndRunJobHandler : IHandleMessageBatches<RecordPeriodEndRunJob>
    {
        private readonly IPaymentLogger logger;
        private readonly IPeriodEndJobService periodEndJobService;

        public RecordPeriodEndRunJobHandler(IPaymentLogger logger, IPeriodEndJobService periodEndJobService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndJobService = periodEndJobService ?? throw new ArgumentNullException(nameof(periodEndJobService));
        }

        public async Task Handle(IList<RecordPeriodEndRunJob> messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                logger.LogInfo($"Handling period end run job: {message.ToJson()}");
                await periodEndJobService.RecordPeriodEndRun(message.JobId, message.CollectionYear,
                    message.CollectionPeriod,message.GeneratedMessages,  cancellationToken);
            }
        }
    }
}