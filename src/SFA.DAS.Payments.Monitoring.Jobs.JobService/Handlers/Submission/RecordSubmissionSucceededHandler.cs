using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.Submission
{
    public class RecordSubmissionSucceededHandler : IHandleMessageBatches<SubmissionSucceededEvent>
    {
        private readonly IEarningsJobService earningsJobService;

        public RecordSubmissionSucceededHandler(IEarningsJobService earningsJobService)
        {
            this.earningsJobService = earningsJobService ?? throw new ArgumentNullException(nameof(earningsJobService));
        }

        public async Task Handle(IList<SubmissionSucceededEvent> messages, CancellationToken cancellationToken)
        {
            foreach (var submissionSucceededEvent in messages)
            {
                await earningsJobService.RecordDcJobCompleted(submissionSucceededEvent.JobId, true, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}