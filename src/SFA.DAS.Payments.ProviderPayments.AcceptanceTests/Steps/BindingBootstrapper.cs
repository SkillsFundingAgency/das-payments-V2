using Autofac;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Steps
{
    [Binding]
    public class BindingBootstrapper : StepsBase
    {
        public BindingBootstrapper(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }


        [BeforeTestRun(Order = 40)]
        public static void SetUpPaymentsDataContext()
        {
            Builder.Register((c, p) =>
            {
                var configHelper = c.Resolve<TestsConfiguration>();
                return new PaymentsDataContext(configHelper.GetConnectionString("PaymentsConnectionString"));
            }).As<IPaymentsDataContext>().InstancePerDependency();
        }


        [BeforeTestRun(Order = 51)]
        public static void AddRoutingConfig()
        {
            var endpointConfiguration = Container.Resolve<EndpointConfiguration>();
            endpointConfiguration.Conventions().DefiningMessagesAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false));

            var transportConfig = Container.Resolve<TransportExtensions<AzureServiceBusTransport>>();
            var routing = transportConfig.Routing();
            routing.RouteToEndpoint(typeof(SfaCoInvestedFundingSourcePaymentEvent), EndpointNames.ProviderPaymentEndPointName);
            routing.RouteToEndpoint(typeof(EmployerCoInvestedFundingSourcePaymentEvent), EndpointNames.ProviderPaymentEndPointName);
            routing.RouteToEndpoint(typeof(MonthEndEvent), EndpointNames.ProviderPaymentEndPointName);
            routing.RouteToEndpoint(typeof(IlrSubmittedEvent), EndpointNames.ProviderPaymentEndPointName);
        }

       
    }
}
