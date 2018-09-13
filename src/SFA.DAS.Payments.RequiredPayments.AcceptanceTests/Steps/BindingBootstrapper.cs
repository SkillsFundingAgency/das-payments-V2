using Autofac;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class BindingBootstrapper: StepsBase
    {
        //public static IEndpointInstance EndpointInstance;

        [BeforeFeature]
        public static void FeatureSetup()
        {
            //var endpointAddress = "sfa-das-payments-requiredpayments-acceptancetests";
            //var endpointConfiguration = new EndpointConfiguration(endpointAddress);

            //var conventions = endpointConfiguration.Conventions();
            //conventions.DefiningMessagesAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false));

            //endpointConfiguration.UsePersistence<AzureStoragePersistence>()
            //    .ConnectionString(TestConfiguration.StorageConnectionString);

            //endpointConfiguration.DisableFeature<TimeoutManager>();

            //endpointConfiguration.UseTransport<AzureServiceBusTransport>()
            //    .UseForwardingTopology()
            //    .ConnectionString(TestConfiguration.ServiceBusConnectionString)
            //    .Routing()
            //    .RouteToEndpoint(typeof(IEarningEvent).Assembly, endpointAddress);
            //    //.RouteToEndpoint(typeof(IEarningEvent).Assembly, EndpointNames.RequiredPayments);

            //endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            //endpointConfiguration.EnableInstallers();
            //endpointConfiguration.UseContainer<AutofacBuilder>();

            //EndpointInstance = Endpoint.Start(endpointConfiguration).Result;
        }

        [BeforeTestRun(Order = 51)]
        public static void AddRoutingConfig()
        {
            var endpointConfiguration = Container.Resolve<EndpointConfiguration>();
            endpointConfiguration.Conventions().DefiningEventsAs(type => type.IsEvent<IRequiredPayment>());
            var transportConfig = Container.Resolve<TransportExtensions<AzureServiceBusTransport>>();
            var routing = transportConfig.Routing();
            routing.RouteToEndpoint(typeof(IEarningEvent), EndpointNames.RequiredPayments);
            routing.RouteToEndpoint(typeof(ApprenticeshipContractType2EarningEvent), EndpointNames.RequiredPayments);
        }
    }
}