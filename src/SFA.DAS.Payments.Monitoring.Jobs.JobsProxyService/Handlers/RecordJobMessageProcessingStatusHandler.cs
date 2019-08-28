using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure;
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
                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    var actor = proxyFactory.CreateActorProxy<IJobsService>(
                        new Uri("fabric:/SFA.DAS.Payments.Monitoring.ServiceFabric/JobsServiceActorService"), actorId);
                    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(2));
                    var jobStatus = await actor.RecordJobMessageProcessingStatus(message, cancellationTokenSource.Token)
                        .ConfigureAwait(false);
                    if (jobStatus == JobStatus.InProgress)
                        return;
                    //await DeleteActor(actorId);
                }
            }
            catch (AggregateException aggregateException)
            {
                if (aggregateException.InnerExceptions.Any(ex => ex is OperationCanceledException))
                {
                    await HandleOperationCancelledException(message, context);
                    return;
                }
                logger.LogWarning($"Failed to record job message status. Job id: {message.JobId}, message id: {message.Id}, name: {message.MessageName}.  Errors: {aggregateException}");
                throw;
            }
            catch (OperationCanceledException)
            {
                await HandleOperationCancelledException(message, context);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to record job message status. Job id: {message.JobId}, message id: {message.Id}, name: {message.MessageName}.  Error: {ex.Message}. {ex}");
                throw;
            }
        }

        private async Task<bool> HandleOperationCancelledException(RecordJobMessageProcessingStatus message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Couldn't get actor within time allocated time. Job id: {message.JobId}, message id: {message.Id}, name: {message.MessageName}.");
            if (!await context.Defer(message, TimeSpan.FromSeconds(1), "no_actor_available", 20))
                throw new InvalidOperationException($"Tried over 10 times to get an actor for job: {message.Id} for message: {message.MessageName}");
            return true;
        }

        private async Task DeleteActor(ActorId actorId)
        {
            var actor = ActorServiceProxy.Create(new Uri("fabric:/SFA.DAS.Payments.Monitoring.ServiceFabric/JobsServiceActorService"), actorId);
            await actor.DeleteActorAsync(actorId, CancellationToken.None);
        }
    }
}