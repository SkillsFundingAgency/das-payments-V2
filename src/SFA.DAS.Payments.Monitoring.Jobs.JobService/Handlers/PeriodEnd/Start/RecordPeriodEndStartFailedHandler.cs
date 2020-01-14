using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.PeriodEnd.Start
{
    public class RecordPeriodEndStartFailedHandler:IHandleMessageBatches<RecordPeriodEndRunStartJobFailed>
    {
        private readonly IPeriodEndJobService periodEndJobService;

        public RecordPeriodEndStartFailedHandler(IPeriodEndJobService periodEndJobService)
        {
            this.periodEndJobService = periodEndJobService ?? throw new ArgumentNullException(nameof(periodEndJobService));
        }
        
        public async Task Handle(IList<RecordPeriodEndRunStartJobFailed> messages, CancellationToken cancellationToken)
        {
            foreach (var submissionSucceededEvent in messages)
            {
                await periodEndJobService.RecordDcJobCompleted(submissionSucceededEvent.JobId, false, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}