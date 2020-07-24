using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.Earnings;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.Submission
{
    public class RecordJobAdditionalMessageBatchHandler: IHandleMessageBatches<RecordJobAdditionalMessages>
    {
        private readonly ICommonJobService commonJobService;
        private readonly IPaymentLogger logger;

        public RecordJobAdditionalMessageBatchHandler(ICommonJobService earningsJobService, IPaymentLogger logger)
        {
            this.commonJobService = earningsJobService ?? throw new ArgumentNullException(nameof(earningsJobService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(IList<RecordJobAdditionalMessages> messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                await commonJobService.RecordNewJobAdditionalMessages(message, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}