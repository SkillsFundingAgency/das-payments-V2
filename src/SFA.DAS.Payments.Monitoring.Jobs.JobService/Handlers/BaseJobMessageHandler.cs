using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Interfaces;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsService.Handlers
{
    public abstract class BaseJobMessageHandler<T> : IHandleMessages<T> where T : IJobMessage
    {
        private readonly IServiceProxyFactory proxyFactory;
        protected IPaymentLogger Logger { get; }

        protected BaseJobMessageHandler(IServiceProxyFactory proxyFactory, IPaymentLogger logger)
        {
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(T message, IMessageHandlerContext context)
        {
            try
            {
                var hashedJobId = GetHashJobId(message.JobId); //TO ensure jobs aren't placed into the same partition
                Logger.LogVerbose($"Getting actor for job: {message.JobId}");
                var service = proxyFactory.CreateServiceProxy<IJobService>(new Uri(ServiceUris.JobService), 
                    new ServicePartitionKey(hashedJobId));
                await HandleMessage(message, context, service, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Failed to record earnings job. Job id: {message.JobId}, Error: {ex.Message}. {ex}");
                throw;
            }
        }

        protected virtual long GetHashJobId(long jobId)
        {
            var guidBytes = Encoding.UTF8.GetBytes(jobId.ToString());
            var hasher = new SHA1CryptoServiceProvider();
            var hashedBytes = hasher.ComputeHash(guidBytes);
            return BitConverter.ToInt64(hashedBytes, 0);
        }

        protected abstract Task HandleMessage(T message, IMessageHandlerContext context, IJobService jobService, CancellationToken cancellationToken);
    }
}