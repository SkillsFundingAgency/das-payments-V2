using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace SFA.DAS.Payments.FundingSource.LevyAccountBalanceService
{

    public class LevyAccountBalanceService : StatefulService
    {
        public LevyAccountBalanceService(StatefulServiceContext context) : base(context) { }
        
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[0];
        }
        
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();







                await Task.Delay(TimeSpan.FromHours(23), cancellationToken);
            }
        }
    }
}
