using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsService.Handlers
{
    public class RecordEarningsJobAdditionalMessagesHandler : IHandleMessages<RecordEarningsJobAdditionalMessages>
    {
        private readonly IUnitOfWorkScopeFactory scopeFactory;
        private readonly IPaymentLogger logger;

        public RecordEarningsJobAdditionalMessagesHandler(IUnitOfWorkScopeFactory scopeFactory, IPaymentLogger logger)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //protected override async Task HandleMessage(RecordEarningsJobAdditionalMessages message, IMessageHandlerContext context, IJobService jobService,
        //    CancellationToken cancellationToken)
        //{
        //    await jobService.RecordEarningsJobAdditionalMessages(message, cancellationToken).ConfigureAwait(false);
        //}

        public async Task Handle(RecordEarningsJobAdditionalMessages message, IMessageHandlerContext context)
        {
            using (var scope = scopeFactory.Create("JobService.RecordEarningsJobAdditionalMessages"))
            {
                try
                {
                    var earningsJobService = scope.Resolve<IEarningsJobService>();
                    await earningsJobService.RecordNewJobAdditionalMessages(message, CancellationToken.None).ConfigureAwait(false);
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