using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers
{
    public class RecordPeriodEndStopJobHandler : IHandleMessageBatches<RecordPeriodEndStopJob>
    {
        private readonly IPaymentLogger logger;

        public RecordPeriodEndStopJobHandler(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(IList<RecordPeriodEndStopJob> messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                logger.LogInfo($"Period end not handled yet. Job: {message.ToJson()}");
            }
        }
    }
}