using System;
using System.Fabric;
using System.Fabric.Description;
using System.Fabric.Health;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Infrastructure
{
    public class ApprovalsServiceHelper
    {
        private static readonly string ServiceName =
            "fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric/SFA.DAS.Payments.DataLocks.ApprovalsService";
        public static async Task<bool> IsApprovalsServiceRunning()
        {
            var client = new FabricClient();
            var result = await client.HealthManager.GetApplicationHealthAsync(
                new Uri("fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric"));
            var service = result.ServiceHealthStates.FirstOrDefault(x =>
                x.ServiceName.AbsoluteUri.Equals(ServiceName));

            return service?.AggregatedHealthState == HealthState.Ok;
        }

        public static async Task StopService()
        {

            var fabricClient = new FabricClient();
            var serviceDescription = new DeleteServiceDescription(new Uri(ServiceName))
            {
                ForceDelete = true,
            };

            await fabricClient.ServiceManager.DeleteServiceAsync(serviceDescription);
        }

        public static async Task StartService()
        {
            var fabricClient = new FabricClient();
            var serviceDescription = new StatelessServiceDescription
            {
                ApplicationName = new Uri("fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric"),
                PartitionSchemeDescription = new SingletonPartitionSchemeDescription(),
                ServiceName = new Uri(ServiceName),
                ServiceTypeName = "SFA.DAS.Payments.DataLocks.ApprovalsServiceType",
                InstanceCount = -1,
            };

            await fabricClient.ServiceManager.CreateServiceAsync(serviceDescription);
        }
    }
}
