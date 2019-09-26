using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers
{
    public class RecordJobMessageProcessingStatusHandler : IHandleMessages<RecordJobMessageProcessingStatus>
    {
        private readonly IUnitOfWorkScopeFactory scopeFactory;
        private readonly IPaymentLogger logger;

        public RecordJobMessageProcessingStatusHandler(IUnitOfWorkScopeFactory scopeFactory,
            IPaymentLogger logger)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(RecordJobMessageProcessingStatus message, IMessageHandlerContext context)
        {
            using (var scope = scopeFactory.Create("JobService.RecordJobMessageProcessingStatus"))
            {
                try
                {
                    var jobMessageService = scope.Resolve<IJobMessageService>();
                    await jobMessageService.RecordCompletedJobMessageStatus(message, CancellationToken.None).ConfigureAwait(false);
                    await scope.Commit();
                }
                catch (Exception ex)
                {
                    scope.Abort();
                    logger.LogWarning($"Failed to record earnings job: {message.JobId}. Error: {ex.Message}, {ex}");
                    throw;
                }
            }
        }
    }
}