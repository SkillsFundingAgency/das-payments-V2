using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RemovedLearnerService.Interfaces;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService.Handlers
{
    public class ReceivedProviderEarningsEventHandler : IHandleMessages<ReceivedProviderEarningsEvent>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger paymentLogger;

        public ReceivedProviderEarningsEventHandler(IActorProxyFactory proxyFactory, IPaymentLogger paymentLogger)
        {
            this.proxyFactory = proxyFactory;
            this.paymentLogger = paymentLogger;
        }

        public async Task Handle(ReceivedProviderEarningsEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing ReceivedProviderEarningsEvent, UKPRN: {message.Ukprn}, JobId: {message.JobId}, Period: {message.CollectionPeriod}, ILR: {message.IlrSubmissionDateTime}");

            var actorId = new ActorId(message.Ukprn);
            var actor = proxyFactory.CreateActorProxy<IRemovedLearnerService>(new Uri("fabric:/SFA.DAS.Payments.RequiredPayments.ServiceFabric/RemovedLearnerServiceActorService"), actorId);
            var removedAims = await actor.HandleReceivedProviderEarningsEvent(message.CollectionPeriod.AcademicYear, message.CollectionPeriod.Period, message.IlrSubmissionDateTime, CancellationToken.None).ConfigureAwait(false);

            foreach (var removedAim in removedAims)
            {
                removedAim.JobId = message.JobId;
                await context.Publish(removedAim).ConfigureAwait(false);
            }

            paymentLogger.LogInfo($"Finished processing ReceivedProviderEarningsEvent, published {removedAims.Count} aims. UKPRN: {message.Ukprn}, JobId: {message.JobId}, Period: {message.CollectionPeriod}, ILR: {message.IlrSubmissionDateTime}");
        }
    }
}