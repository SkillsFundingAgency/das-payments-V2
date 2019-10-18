using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.Submission
{
    public class RecordSubmissionFailedHandler : IHandleMessageBatches<SubmissionFailedEvent>
    {
        private readonly IEarningsJobService earningsJobService;

        public RecordSubmissionFailedHandler(IEarningsJobService earningsJobService)
        {
            this.earningsJobService = earningsJobService ?? throw new ArgumentNullException(nameof(earningsJobService));
        }

        public async Task Handle(IList<SubmissionFailedEvent> messages, CancellationToken cancellationToken)
        {
            foreach (var submissionFailedEvent in messages)
            {
                await earningsJobService.RecordDcJobCompleted(submissionFailedEvent.JobId, true, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}