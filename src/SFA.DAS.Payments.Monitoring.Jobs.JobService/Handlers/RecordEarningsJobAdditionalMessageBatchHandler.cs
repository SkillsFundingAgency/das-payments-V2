using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers
{
    public class RecordEarningsJobAdditionalMessageBatchHandler: IHandleMessageBatches<RecordEarningsJobAdditionalMessages>
    {
        private readonly IEarningsJobService earningsJobService;
        private readonly IPaymentLogger logger;

        public RecordEarningsJobAdditionalMessageBatchHandler(IEarningsJobService earningsJobService, IPaymentLogger logger)
        {
            this.earningsJobService = earningsJobService ?? throw new ArgumentNullException(nameof(earningsJobService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(IList<RecordEarningsJobAdditionalMessages> messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                await earningsJobService.RecordNewJobAdditionalMessages(message, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}