using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers
{
    public class RecordJobMessageProcessingStatusHandler : IHandleMessages<RecordJobMessageProcessingStatus>
    {
        private readonly IJobMessageService jobMessageService;
        private readonly IPaymentLogger logger;

        public RecordJobMessageProcessingStatusHandler(IJobMessageService jobMessageService, IPaymentLogger logger)
        {
            this.jobMessageService = jobMessageService ?? throw new ArgumentNullException(nameof(jobMessageService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(RecordJobMessageProcessingStatus message, IMessageHandlerContext context)
        {
            try
            {
                await jobMessageService.RecordCompletedJobMessageStatus(message, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to record earnings job: {message.JobId}. Error: {ex.Message}, {ex}");
                throw;
            }
        }
    }
}