using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.JobsService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsProxyService.Handlers
{
    public class RecordJobMessageProcessingStatusHandler : IHandleMessages<RecordJobMessageProcessingStatus>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger logger;

        public RecordJobMessageProcessingStatusHandler(IActorProxyFactory proxyFactory,
            IPaymentLogger logger)
        {
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(RecordJobMessageProcessingStatus message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogVerbose($"Getting actor for job: {message.JobId}");
                var actorId = new ActorId(message.JobId.ToString());
                var actor = proxyFactory.CreateActorProxy<IJobsService>(new Uri("fabric:/SFA.DAS.Payments.Monitoring.ServiceFabric/JobsServiceActorService"), actorId);
                var jobStatus = await actor.RecordJobMessageProcessingStatus(message).ConfigureAwait(false);
                if (jobStatus == JobStatus.InProgress)
                    return;
                //await DeleteActor(actorId);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to record job message status. Job id: {message.JobId}, message id: {message.Id}, name: {message.MessageName}.  Error: {ex.Message}. {ex}");
                throw;
            }
        }

        private async Task DeleteActor(ActorId actorId)
        {
            var actor = ActorServiceProxy.Create(new Uri("fabric:/SFA.DAS.Payments.Monitoring.ServiceFabric/JobsServiceActorService"), actorId);
            await actor.DeleteActorAsync(actorId, CancellationToken.None);
        }
    }
}