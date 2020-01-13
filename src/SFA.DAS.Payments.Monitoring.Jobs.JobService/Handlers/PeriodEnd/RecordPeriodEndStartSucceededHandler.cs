using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.PeriodEnd
{
    public class RecordPeriodEndStartSucceededHandler :IHandleMessageBatches<RecordPeriodEndRunStartJobSucceeded>
    {
        private readonly IPeriodEndJobService periodEndJobService;

        public RecordPeriodEndStartSucceededHandler(IPeriodEndJobService periodEndJobService)
        {
            this.periodEndJobService = periodEndJobService ?? throw new ArgumentNullException(nameof(periodEndJobService));
        }

        public async Task Handle(IList<RecordPeriodEndRunStartJobSucceeded> messages, CancellationToken cancellationToken)
        {
            foreach (var submissionSucceededEvent in messages)
            {
                await periodEndJobService.RecordDcJobCompleted(submissionSucceededEvent.JobId, true, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}