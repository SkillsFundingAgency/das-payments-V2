using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Commands;

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

        public async Task Handle(ProcessLevyPaymentsOnMonthEndCommand message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing ProcessLevyPaymentsOnMonthEndCommand. Message Id : {context.MessageId}");

            foreach (var employerId in message.EmployerIds)
            {
                using (var operation = telemetry.StartOperation())
                {
                    var actorId = new ActorId(employerId);
                    var actor = proxyFactory.CreateActorProxy<ILevyFundedService>(new Uri("fabric:/SFA.DAS.Payments.FundingSource.ServiceFabric/LevyFundedServiceActorService"), actorId);
                    await actor.HandleMonthEnd(message).ConfigureAwait(false);
                    telemetry.StopOperation(operation);
                }
            }
        }
    }
}