using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsService.Handlers
{
    public class RecordEarningsJobAdditionalMessagesHandler : IHandleMessages<RecordEarningsJobAdditionalMessages>
    {
        private readonly IEarningsJobService earningsJobService;
        private readonly IPaymentLogger logger;

        public RecordEarningsJobAdditionalMessagesHandler(IEarningsJobService earningsJobService, IPaymentLogger logger)
        {
            this.earningsJobService = earningsJobService ?? throw new ArgumentNullException(nameof(earningsJobService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(RecordEarningsJobAdditionalMessages message, IMessageHandlerContext context)
        {
            try
            {
                //await earningsJobService.RecordNewJobAdditionalMessages(message, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to record earnings job: {message.JobId}. Error: {ex.Message}, {ex}");
                throw;
            }
        }
    }
}