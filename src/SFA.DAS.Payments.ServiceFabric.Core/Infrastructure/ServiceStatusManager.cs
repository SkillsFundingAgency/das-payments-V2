using SFA.DAS.Payments.ServiceFabric.Core.Constants;
using System.Fabric.Description;
using System.Fabric.Health;
using System.Fabric;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure
{
    public interface IServiceStatusManager
    {
        Task<bool> IsServiceRunning(string applicationName, string serviceName);
        Task StopService(string applicationName, string serviceName);
        Task StartService(string applicationName, string serviceName, int instanceCount);
    }

    public class ServiceStatusManager : IServiceStatusManager
    {
        private string GetApplicationUri(string applicationName) => $"fabric:/{applicationName}";
        private string GetServiceUri(string applicationName, string serviceName) =>
            $"{GetApplicationUri(applicationName)}/{serviceName}";


        public async Task<bool> IsServiceRunning(string applicationName, string serviceName)
        {
            var client = new FabricClient();
            var result = await client.HealthManager.GetApplicationHealthAsync(
                new Uri(GetApplicationUri(applicationName)));
            var service = result.ServiceHealthStates.FirstOrDefault(x =>
                x.ServiceName.AbsoluteUri.Equals(GetServiceUri(applicationName, serviceName)));
            return service?.AggregatedHealthState == HealthState.Ok;
        }

        public async Task StopService(string applicationName, string serviceName)
        {

            var fabricClient = new FabricClient();
            var serviceDescription = new DeleteServiceDescription(new Uri(GetServiceUri(applicationName, serviceName)))
            {
                ForceDelete = true,
            };

            await fabricClient.ServiceManager.DeleteServiceAsync(serviceDescription);
        }

        public async Task StartService(string applicationName, string serviceName, int instanceCount)
        {
            var fabricClient = new FabricClient();
            var serviceDescription = new StatelessServiceDescription
            {
                ApplicationName = new Uri(GetApplicationUri(applicationName)),
                PartitionSchemeDescription = new SingletonPartitionSchemeDescription(),
                ServiceName = new Uri(GetServiceUri(applicationName, serviceName)),
                ServiceTypeName = $"{serviceName}Type",
                InstanceCount = instanceCount,
            };

            await fabricClient.ServiceManager.CreateServiceAsync(serviceDescription);
        }

    }
}