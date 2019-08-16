using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class FoundNotLevyPayerEmployerAccountEventHandler : IHandleMessages<FoundNotLevyPayerEmployerAccount>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger paymentLogger;
        private readonly ILifetimeScope scope;

        public FoundNotLevyPayerEmployerAccountEventHandler(IActorProxyFactory proxyFactory, IPaymentLogger paymentLogger, ILifetimeScope scope)
        {
            this.proxyFactory = proxyFactory;
            this.paymentLogger = paymentLogger;
            this.scope = scope;
        }

        public async Task Handle(FoundNotLevyPayerEmployerAccount message, IMessageHandlerContext context)
        {
            paymentLogger.LogDebug($"Handling Found NotLevyPayer Employer Account Id: {message.AccountId}");
            var processor = scope.Resolve<IApprenticeshipProcessor>();
            await processor.ProcessApprenticeshipForNonLevyPayerEmployer(message.AccountId).ConfigureAwait(false);
            paymentLogger.LogInfo($"Finished Handling Found NotLevyPayer Employer Account Id: {message.AccountId}");
        }
    }
}
