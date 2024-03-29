﻿using System;
using System.Fabric;
using System.Fabric.Description;
using System.Fabric.Health;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Infrastructure
{
    public class ApprovalsServiceHelper
    {
        public static async Task<bool> IsApprovalsServiceRunning()
        {
            var client = new FabricClient();
            var result = await client.HealthManager.GetApplicationHealthAsync(
                new Uri(ServiceNames.DataLocksApprovals.ApplicationUri));
            var service = result.ServiceHealthStates.FirstOrDefault(x =>
                x.ServiceName.AbsoluteUri.Equals(ServiceNames.DataLocksApprovals.ServiceUri));

            return service?.AggregatedHealthState == HealthState.Ok;
        }

        public static async Task StopService()
        {

            var fabricClient = new FabricClient();
            var serviceDescription = new DeleteServiceDescription(new Uri(ServiceNames.DataLocksApprovals.ServiceUri))
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
                ApplicationName = new Uri(ServiceNames.DataLocksApprovals.ApplicationUri),
                PartitionSchemeDescription = new SingletonPartitionSchemeDescription(),
                ServiceName = new Uri(ServiceNames.DataLocksApprovals.ServiceUri),
                ServiceTypeName = ServiceNames.DataLocksApprovals.ServiceTypeName,
                InstanceCount = -1,
            };

            await fabricClient.ServiceManager.CreateServiceAsync(serviceDescription);
        }
    }
}
