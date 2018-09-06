using NServiceBus;
using NServiceBus.Features;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    [Binding]
    public class BindingBootstrapper
    {
        public static IEndpointInstance EndpointInstance;

        [BeforeFeature]
        public static void FeatureSetup()
        {
            var endpointAddress = "sfa-das-payments-fundingsource-acceptancetests";
            var endpointConfiguration = new EndpointConfiguration(endpointAddress);

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningMessagesAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false));

            endpointConfiguration.UsePersistence<AzureStoragePersistence>()
                .ConnectionString(TestConfiguration.StorageConnectionString);

            endpointConfiguration.DisableFeature<TimeoutManager>();

            endpointConfiguration.UseTransport<AzureServiceBusTransport>()
                .UseForwardingTopology()
                .ConnectionString(TestConfiguration.ServiceBusConnectionString)
                .Routing();
//                .RouteToEndpoint(typeof(IEarningEvent).Assembly, endpointAddress);
            // Need correct interface to bind to

            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseContainer<AutofacBuilder>();

            EndpointInstance = Endpoint.Start(endpointConfiguration).Result;
        }
    }
}
