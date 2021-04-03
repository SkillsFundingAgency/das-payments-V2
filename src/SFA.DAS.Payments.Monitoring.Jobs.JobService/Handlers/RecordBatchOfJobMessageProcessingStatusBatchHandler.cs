using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobMessageProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers
{
    public class RecordBatchOfJobMessageProcessingStatusBatchHandler : IHandleMessageBatches<RecordBatchOfJobMessageProcessingStatus>
    {
        private readonly IJobMessageService jobMessageService;
        private readonly IPaymentLogger logger;

        public RecordBatchOfJobMessageProcessingStatusBatchHandler(IJobMessageService jobMessageService,
            IPaymentLogger logger)
        {
            this.jobMessageService = jobMessageService ?? throw new ArgumentNullException(nameof(jobMessageService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(IList<RecordBatchOfJobMessageProcessingStatus> messages, CancellationToken cancellationToken)
        {
            var jobProcessingStatuses = messages.SelectMany(message => message.JobMessageProcessingStatuses).ToList();
            foreach (var message in jobProcessingStatuses)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await jobMessageService.RecordCompletedJobMessageStatus(message, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}