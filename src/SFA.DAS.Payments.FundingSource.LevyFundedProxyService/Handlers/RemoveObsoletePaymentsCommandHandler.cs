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
    public class RemoveObsoletePaymentsCommandHandler : IHandleMessages<RemoveObsoletePayments>
    {
        private readonly ITelemetry telemetry;
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger paymentLogger;

        public RemoveObsoletePaymentsCommandHandler(ITelemetry telemetry, IActorProxyFactory proxyFactory, IPaymentLogger paymentLogger)
        {
            this.telemetry = telemetry;
            this.proxyFactory = proxyFactory;
            this.paymentLogger = paymentLogger;
        }

        public async Task Handle(RemoveObsoletePayments command, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing RemoveObsoletePayments. Message Id : {context.MessageId}, Account Id : {command.AccountId}");

            using (var operation = telemetry.StartOperation())
            {
                try
                {
                    var actorId = new ActorId(command.AccountId);
                    var actor = proxyFactory.CreateActorProxy<ILevyFundedService>(new Uri("fabric:/SFA.DAS.Payments.FundingSource.ServiceFabric/LevyFundedServiceActorService"), actorId);
                    await actor.RemoveObsoletePayments(command).ConfigureAwait(false);
                    telemetry.StopOperation(operation);
                }
                catch (Exception ex)
                {
                    paymentLogger.LogError($"Error while Removing Obsolete Payments for account: {command.AccountId}. Error: {ex}", ex);
                    throw;
                }
            }
        }
    }
}