using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsProxyService.Handlers
{
    public class RecordEarningsJobHandler: IHandleMessages<RecordEarningsJob>
    {
        private readonly IServiceProxyFactory proxyFactory;
        private readonly IPaymentLogger logger;

        public RecordEarningsJobHandler(IServiceProxyFactory proxyFactory,
            IPaymentLogger logger)
        {
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(RecordEarningsJob message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogVerbose($"Getting actor for job: {message.JobId}");
                var partitionResolver = new ServicePartitionResolver();
                var resolved = await partitionResolver.ResolveAsync(new Uri(ServiceUris.JobService),
                    new ServicePartitionKey(message.JobId), CancellationToken.None);

                //var actorId = new ActorId(message.JobId.ToString());
                var service = proxyFactory.CreateServiceProxy<IJobService>(new Uri(ServiceUris.JobService),
                    new ServicePartitionKey(message.JobId));
                await service.RecordEarningsJob(message, CancellationToken.None).ConfigureAwait(false);  
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to record earnings job. ukprn: {message.Ukprn}, Job id: {message.JobId}, Period: {message.CollectionPeriod}-{message.CollectionYear}  Error: {ex.Message}. {ex}");
                throw;
            }
        }
    }
}