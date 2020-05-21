using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.EarningEvents.ProcessLearnerService.Interfaces;

namespace SFA.DAS.Payments.EarningEvents.ProcessLearnerProxyService.Handlers
{
    public class ProcessLearnerHandler : IHandleMessages<ProcessLearnerCommand>
    {
        private readonly IPaymentLogger logger;
        private readonly IActorProxyFactory proxyFactory;

        public ProcessLearnerHandler(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.proxyFactory = proxyFactory ?? new ActorProxyFactory();
        }

        public async Task Handle(ProcessLearnerCommand message, IMessageHandlerContext context)
        {
            logger.LogDebug(
                $"Handling ILR learner submission. Job: {message.JobId}, Ukprn: {message.Ukprn}, Collection year: {message.CollectionYear}, Learner: {message.Learner.LearnRefNumber}");

            string key = $"{message.Ukprn}-{message.Learner.LearnRefNumber}";
            var actorId = new ActorId(key);
            var actor = proxyFactory.CreateActorProxy<IProcessLearnerService>(
                new Uri("fabric:/SFA.DAS.Payments.EarningEvents.ServiceFabric/ProcessLearnerServiceActorService"),
                actorId);

            var learnerEarningEvents = actor.CreateLearnerEarningEvents(message);

            foreach (var earningEvent in learnerEarningEvents)
            {
                await context.Publish(earningEvent).ConfigureAwait(false);
            }

            var summary =  learnerEarningEvents.Any() ? string.Join(", ",
                learnerEarningEvents.GroupBy(e => e.GetType().Name).Select(g => $"{g.Key}: {g.Count()}")): "none";
            logger.LogInfo(
                $"Finished handling ILR learner submission.Job: {message.JobId}, Ukprn: {message.Ukprn}, Collection year: {message.CollectionYear}, Learner: {message.Learner.LearnRefNumber}. Published events: {summary}");
        }
    }
}