using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.Submission
{
    public class RecordSubmissionFailedHandler : IHandleMessageBatches<RecordEarningsJobFailed>
    {
        private readonly IEarningsJobService earningsJobService;

        public RecordSubmissionFailedHandler(IEarningsJobService earningsJobService)
        {
            this.earningsJobService = earningsJobService ?? throw new ArgumentNullException(nameof(earningsJobService));
        }

        public async Task Handle(IList<RecordEarningsJobFailed> messages, CancellationToken cancellationToken)
        {
            foreach (var submissionFailedEvent in messages)
            {
                await earningsJobService.RecordDcJobCompleted(submissionFailedEvent.JobId, false, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}