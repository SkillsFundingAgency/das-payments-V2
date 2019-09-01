using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure;
using SFA.DAS.Payments.Monitoring.Jobs.JobsService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsProxyService.Handlers
{
    public abstract class JobMessageStatusHandler<T> : IHandleMessages<T> where T: IJobMessageStatus
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger logger;

        protected JobMessageStatusHandler(IActorProxyFactory proxyFactory, IPaymentLogger logger)
        {
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(T message, IMessageHandlerContext context)
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
                    await HandleMessage(message, context, actor, cancellationTokenSource.Token).ConfigureAwait(false); 
                }
            }
            catch (AggregateException aggregateException)
            {
                if (aggregateException.InnerExceptions.Any(ex => ex is OperationCanceledException))
                {
                    await HandleOperationCancelledException(message, context);
                    return;
                }
                logger.LogWarning($"Failed to record job message status. Job id: {message.JobId}, message: {message.ToJson()}.  Errors: {aggregateException}");
                throw;
            }
            catch (OperationCanceledException)
            {
                await HandleOperationCancelledException(message, context);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to record job message status. Job id: {message.JobId}, message: {message.ToJson()}.  Error: {ex.Message}. {ex}");
                throw;
            }
        }

        protected abstract Task HandleMessage(T message, IMessageHandlerContext context, IJobsService actor, CancellationToken cancellationToken);
        private async Task<bool> HandleOperationCancelledException(T message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Couldn't get actor within time allocated time. Job id: {message.JobId}message: {message.ToJson()}.");
            if (!await context.Defer(message, TimeSpan.FromSeconds(1), "no_actor_available", 20))
                throw new InvalidOperationException($"Tried over 10 times to get an actor for job: message: {message.ToJson()}");
            return true;
        }

        private async Task DeleteActor(ActorId actorId)
        {
            var actor = ActorServiceProxy.Create(new Uri("fabric:/SFA.DAS.Payments.Monitoring.ServiceFabric/JobsServiceActorService"), actorId);
            await actor.DeleteActorAsync(actorId, CancellationToken.None);
        }
    }
}