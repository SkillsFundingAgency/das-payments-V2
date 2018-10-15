using Autofac;
using ESFA.DC.JobContextManager.Model;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Steps
{
    [Binding]
    public class BindingBootstrapper: StepsBase
    {
        public BindingBootstrapper(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [BeforeTestRun(Order = 51)]
        public static void AddRoutingConfig()
        {
            var endpointConfiguration = Container.Resolve<EndpointConfiguration>();
            endpointConfiguration.Conventions().DefiningEventsAs(type => type.IsEvent<IEarningEvent>());
            var transportConfig = Container.Resolve<TransportExtensions<AzureServiceBusTransport>>();
            var routing = transportConfig.Routing();
            routing.RouteToEndpoint(typeof(JobContextMessage), EndpointNames.EarningEvents);
        }
    }
}