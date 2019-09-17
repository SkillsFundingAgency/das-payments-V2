using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Interfaces;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsProxyService.Handlers
{
    public abstract class BaseJobMessageHandler<T> : IHandleMessages<T> where T : IJobMessage
    {
        private readonly IServiceProxyFactory proxyFactory;
        private readonly IPaymentLogger logger;

        protected BaseJobMessageHandler(IServiceProxyFactory proxyFactory,
            IPaymentLogger logger)
        {
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(T message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogVerbose($"Getting actor for job: {message.JobId}");
                var service = proxyFactory.CreateServiceProxy<IJobService>(new Uri(ServiceUris.JobService), 
                    new ServicePartitionKey(message.JobId));
                await HandleMessage(message, context, service, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to record earnings job. Job id: {message.JobId}, Error: {ex.Message}. {ex}");
                throw;
            }
        }
        protected abstract Task HandleMessage(T message, IMessageHandlerContext context, IJobService jobService, CancellationToken cancellationToken);
    }
}