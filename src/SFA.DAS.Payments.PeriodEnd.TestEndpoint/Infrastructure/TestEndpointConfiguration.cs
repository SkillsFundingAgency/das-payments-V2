using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.PeriodEnd.TestEndpoint.Infrastructure
{
    public interface ITestEndpointConfiguration
    {
         string EndpointName { get; }
        string StorageConnectionString { get; }
        string ServiceBusConnectionString { get; }
        string PaymentsConnectionString { get; }
        string ProviderPaymentsEndpointName { get;}
    }

    public class TestEndpointConfiguration: ITestEndpointConfiguration
    {
        public  string EndpointName { get; set; }
        public string StorageConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string PaymentsConnectionString { get; set; }
        public string ProviderPaymentsEndpointName { get; set; }
    }
}
