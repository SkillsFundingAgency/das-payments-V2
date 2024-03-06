using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Model
{
    public class ServiceNames
    {
        public class Service
        {
            public string ApplicationName { get; }
            public string ServiceName { get; }
            public string ServiceTypeName { get; }

            public string ApplicationUri => $"fabric:/{ApplicationName}";
            public string ServiceUri => $"{ApplicationUri}/{ServiceName}";

            internal Service(string applicationName, string serviceName)
            {
                ApplicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
                ServiceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
                ServiceTypeName = $"{serviceName}Type";
            }

            internal Service(string applicationName, string serviceName, string serviceTypeName)
            {
                ApplicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
                ServiceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
                ServiceTypeName = serviceTypeName ?? throw new ArgumentNullException(nameof(serviceTypeName));
            }
        }

        public static Service DataLocksApprovals => new Service("SFA.DAS.Payments.DataLocks.ServiceFabric",
            "SFA.DAS.Payments.DataLocks.ApprovalsService");
    }
}