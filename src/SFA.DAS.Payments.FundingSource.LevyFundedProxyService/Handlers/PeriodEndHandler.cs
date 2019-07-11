using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class PeriodEndHandler: IHandleMessages<PeriodEndRunningEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IActorProxyFactory proxyFactory;

        public PeriodEndHandler(IPaymentLogger logger, IActorProxyFactory proxyFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
        }

        public Task Handle(PeriodEndRunningEvent message, IMessageHandlerContext context)
        {
            throw new NotImplementedException();
        }
    }
}