using Autofac;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Audit.AcceptanceTests.Data;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.Audit.AcceptanceTests.Infrastructure
{
    [Binding]
    public class BindingBootstrapper : BindingsBase
    {
        [BeforeTestRun(Order = 40)]
        public static void SetUpPaymentsDataContext()
        {
            Builder.Register((c, p) =>
            {
                var configHelper = c.Resolve<TestsConfiguration>();
                return new AuditDataContext(configHelper.PaymentsConnectionString);
            })
                .AsSelf()
                .InstancePerLifetimeScope()
                .AutoActivate();

            Builder.Register((c, p) =>
            {
                var configHelper = c.Resolve<TestsConfiguration>();
                return new PaymentsDataContext(configHelper.PaymentsConnectionString);
            })
                .As<IPaymentsDataContext>()
                .InstancePerLifetimeScope()
                .AutoActivate();
        }

        [BeforeTestRun(Order = 51)]
        public static void AddRoutingConfig()
        {
            var endpointConfiguration = Container.Resolve<EndpointConfiguration>();
            endpointConfiguration.Conventions().DefiningEventsAs(type => false);
            var transportConfig = Container.Resolve<TransportExtensions<AzureServiceBusTransport>>();
            var routing = transportConfig.Routing();
            routing.RouteToEndpoint(typeof(SfaCoInvestedFundingSourcePaymentEvent), EndpointNames.AuditServiceEndpoint);
            routing.RouteToEndpoint(typeof(EmployerCoInvestedFundingSourcePaymentEvent), EndpointNames.AuditServiceEndpoint);
            routing.RouteToEndpoint(typeof(SfaFullyFundedFundingSourcePaymentEvent), EndpointNames.AuditServiceEndpoint);
            routing.RouteToEndpoint(typeof(LevyFundingSourcePaymentEvent), EndpointNames.AuditServiceEndpoint);
        }
    }
}
