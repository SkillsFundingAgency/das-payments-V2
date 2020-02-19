using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class MonthEndCommandHandler : IHandleMessages<ProcessLevyPaymentsOnMonthEndCommand>
    {
        private readonly ITelemetry telemetry;
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger paymentLogger;

        public MonthEndCommandHandler(ITelemetry telemetry, IActorProxyFactory proxyFactory, IPaymentLogger paymentLogger)
        {
            this.telemetry = telemetry;
            this.proxyFactory = proxyFactory;
            this.paymentLogger = paymentLogger;
        }

        public async Task Handle(ProcessLevyPaymentsOnMonthEndCommand command, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing ProcessLevyPaymentsOnMonthEndCommand. Message Id : {context.MessageId}, Job: {command.JobId}");

            using (var operation = telemetry.StartOperation())
            {
                var actorId = new ActorId(command.AccountId.ToString());
                var actor = proxyFactory.CreateActorProxy<ILevyFundedService>(new Uri("fabric:/SFA.DAS.Payments.FundingSource.ServiceFabric/LevyFundedServiceActorService"), actorId);
                var fundingSourceEvents = await actor.HandleMonthEnd(command).ConfigureAwait(false);
                foreach (var fundingSourcePaymentEvent in fundingSourceEvents)
                {
                    if (fundingSourcePaymentEvent is ProcessUnableToFundTransferFundingSourcePayment)
                        await context.SendLocal(fundingSourcePaymentEvent).ConfigureAwait(false);
                    else
                        await context.Publish(fundingSourcePaymentEvent).ConfigureAwait(false);
                }

                //await Task.WhenAll(fundingSourceEvents.Select(context.Publish));
                telemetry.StopOperation(operation);
            }
        }
    }
}