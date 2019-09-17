using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService
{
    [StatePersistence(StatePersistence.Volatile)]
    public class JobService : StatefulService, IJobService, IService
    {
        private readonly IPaymentLogger logger;
        private readonly IUnitOfWorkScopeFactory scopeFactory;

        public JobService(StatefulServiceContext context, IPaymentLogger logger, IUnitOfWorkScopeFactory scopeFactory
            )
            : base(context)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }


        public async Task RecordEarningsJob(RecordEarningsJob message, CancellationToken cancellationToken)
        {
            using (var scope = scopeFactory.Create("JobService.RecordEarningsJob"))
            {
                try
                {
                    var earningsJobService = scope.Resolve<IEarningsJobService>();
                    await earningsJobService.RecordNewJob(message, cancellationToken).ConfigureAwait(false);
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

        public async Task RecordEarningsJobAdditionalMessages(RecordEarningsJobAdditionalMessages message,
            CancellationToken cancellationToken)
        {
            using (var scope = scopeFactory.Create("JobService.RecordEarningsJobAdditionalMessages"))
            {
                try
                {
                    var earningsJobService = scope.Resolve<IEarningsJobService>();
                    await earningsJobService.RecordNewJobAdditionalMessages(message, cancellationToken).ConfigureAwait(false);
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

        public async Task RecordJobMessageProcessingStatus(RecordJobMessageProcessingStatus message, CancellationToken cancellationToken)
        {
            using (var scope = scopeFactory.Create("JobService.RecordJobMessageProcessingStatus"))
            {
                try
                {
                    var jobMessageService = scope.Resolve<IJobMessageService>();
                    await jobMessageService.RecordCompletedJobMessageStatus(message, cancellationToken).ConfigureAwait(false);
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

        public Task RecordJobMessageProcessingStartedStatus(RecordStartedProcessingJobMessages message,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
