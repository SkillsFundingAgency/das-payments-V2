using Autofac;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class PaymentsDueBootstrapper : StepsBase
    {
        public PaymentsDueBootstrapper(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [BeforeTestRun(Order = 51)]
        public static void AddRoutingConfig()
        {
            var endpointConfiguration = Container.Resolve<EndpointConfiguration>();
            endpointConfiguration.Conventions().DefiningEventsAs(type => type.IsEvent<IPaymentDueEvent>());
            endpointConfiguration.Conventions().DefiningMessagesAs(type => typeof(IEarningEvent).IsAssignableFrom(type));
            var transportConfig = Container.Resolve<TransportExtensions<AzureServiceBusTransport>>();
            var routing = transportConfig.Routing();
            routing.RouteToEndpoint(typeof(ApprenticeshipContractType2EarningEvent), EndpointNames.PaymentsDue);
        }
    }
}
