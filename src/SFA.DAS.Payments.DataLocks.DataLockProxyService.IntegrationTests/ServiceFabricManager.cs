using System;
using System.Fabric;
using System.Fabric.Health;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.IntegrationTests
{
    class ServiceFabricManager
    {
        public static async Task<bool> IsApprovalsServiceRunning()
        {
            var client = new FabricClient();
            var result = await client.HealthManager.GetApplicationHealthAsync(
                new Uri("fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric"));
            var service = result.ServiceHealthStates.FirstOrDefault(x =>
                x.ServiceName.AbsoluteUri.Equals(
                    "fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric/SFA.DAS.Payments.DataLocks.ApprovalsService"));

            if (service?.AggregatedHealthState == HealthState.Ok)
            {
                return true;
            }

            return false;
        }
    }
}
