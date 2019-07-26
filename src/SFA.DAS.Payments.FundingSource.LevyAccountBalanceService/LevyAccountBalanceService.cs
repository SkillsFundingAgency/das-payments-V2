using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.FundingSource.Integrations.Services;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.FundingSource.LevyAccountBalanceService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class LevyAccountBalanceService : StatefulService
    {
        private readonly IProcessLevyAccountBalanceService processLevyAccountBalanceService;
        private readonly IPaymentLogger logger;
        private readonly ILifetimeScope lifetimeScope;
        private IStatefulEndpointCommunicationListener listener;
        private readonly TimeSpan refreshInterval;

        protected LevyAccountBalanceService(StatefulServiceContext context,
            IPaymentLogger logger,
            ILifetimeScope lifetimeScope,
            IProcessLevyAccountBalanceService processLevyAccountBalanceService,
            IConfigurationHelper configurationHelper) : base(context)
        {
            this.processLevyAccountBalanceService = processLevyAccountBalanceService ?? throw new ArgumentNullException(nameof(processLevyAccountBalanceService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            this.processLevyAccountBalanceService = processLevyAccountBalanceService;
            var intervalInHours = double.Parse(configurationHelper.GetSetting("LevyAccountRefreshIntervalInHours"));
            refreshInterval = TimeSpan.FromHours(intervalInHours);
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            logger.LogInfo("Creating Service Replica Listeners For Levy Account Balance Service");
            return new List<ServiceReplicaListener>
            {
                new ServiceReplicaListener(context => listener = lifetimeScope.Resolve<IStatefulEndpointCommunicationListener>())
            };
        }

        protected override  Task RunAsync(CancellationToken cancellationToken)
        {
            logger.LogInfo("Running Levy Account Balance Service");
            return  Task.WhenAll(listener.RunAsync(), processLevyAccountBalanceService.RunAsync(refreshInterval, cancellationToken));
        }
    }
}
