using System.Linq;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Steps
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
                return new TestPaymentsDataContext(configHelper.PaymentsConnectionString);
            }).As<IPaymentsDataContext>().As<TestPaymentsDataContext>()
                .InstancePerDependency();
        }
        
        [BeforeTestRun(Order = 51)]
        public static void AddRoutingConfig()
        {
            var endpointConfiguration = Container.Resolve<EndpointConfiguration>();
            endpointConfiguration.Conventions().DefiningEventsAs(type => type.IsEvent<ProviderPaymentEvent>());
            var transportConfig = Container.Resolve<TransportExtensions<AzureServiceBusTransport>>();
            var routing = transportConfig.Routing();
            routing.RouteToEndpoint(typeof(SfaFullyFundedFundingSourcePaymentEvent), EndpointNames.ProviderPaymentEndPointName);
            routing.RouteToEndpoint(typeof(SfaCoInvestedFundingSourcePaymentEvent), EndpointNames.ProviderPaymentEndPointName);
            routing.RouteToEndpoint(typeof(EmployerCoInvestedFundingSourcePaymentEvent), EndpointNames.ProviderPaymentEndPointName);
            routing.RouteToEndpoint(typeof(ProcessProviderMonthEndAct1CompletionPaymentCommand), EndpointNames.ProviderPaymentEndPointName);
            routing.RouteToEndpoint(typeof(SubmissionJobSucceeded), EndpointNames.ProviderPaymentEndPointName);
            routing.RouteToEndpoint(typeof(PeriodEndStartedEvent).Assembly, EndpointNames.ProviderPaymentEndPointName);
            
            var autoSubscribe = endpointConfiguration.AutoSubscribe();
            autoSubscribe.DisableFor<RecordedAct1CompletionPayment>();
        }

        [BeforeTestRun(Order = 52)]
        public static async Task AddDasEndPoint()
        {
            var endpointConfiguration = new EndpointConfiguration("sfa-das-payments-providerpayments");

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(t => t.IsAssignableTo<RecordedAct1CompletionPayment>());

            var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
            persistence.ConnectionString(TestConfiguration.StorageConnectionString);

            endpointConfiguration.DisableFeature<TimeoutManager>();
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport
                .ConnectionString(TestConfiguration.DasServiceBusConnectionString)
                .UseForwardingTopology()
                .Transactions(TransportTransactionMode.ReceiveOnly);

            var strategy = transport.Sanitization().UseStrategy<ValidateAndHashIfNeeded>();

            strategy.RuleNameSanitization(ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);

            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();

            DasEndpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            await DasEndpointInstance.Subscribe<RecordedAct1CompletionPayment>()
                                     .ConfigureAwait(false);
        }

        public static IEndpointInstance DasEndpointInstance { get; set; }
    }
}