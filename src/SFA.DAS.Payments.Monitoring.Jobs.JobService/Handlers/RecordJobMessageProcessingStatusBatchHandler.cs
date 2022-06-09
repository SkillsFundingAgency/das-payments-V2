using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobMessageProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers
{
    public class RecordJobMessageProcessingStatusBatchHandler : IHandleMessageBatches<RecordJobMessageProcessingStatus>
    {
        private readonly IJobMessageService jobMessageService;
        private readonly IPaymentLogger logger;

        public RecordJobMessageProcessingStatusBatchHandler(IJobMessageService jobMessageService,
            IPaymentLogger logger)
        {
            this.jobMessageService = jobMessageService ?? throw new ArgumentNullException(nameof(jobMessageService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(IList<RecordJobMessageProcessingStatus> messages, CancellationToken cancellationToken)
        {

            cancellationToken.ThrowIfCancellationRequested();
            await jobMessageService.RecordCompletedJobMessageStatus(messages, cancellationToken).ConfigureAwait(false);
        }
    }
}