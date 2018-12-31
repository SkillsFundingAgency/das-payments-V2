using Autofac;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Messages;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Infrastructure
{
    [Binding]
    public class BindingBootstrapper : BindingsBase
    {
        [BeforeTestRun(Order = 51)]
        public static void AddRoutingConfig()
        {
            var endpointConfiguration = Container.Resolve<EndpointConfiguration>();
            endpointConfiguration.Conventions().DefiningCommandsAs(type => type.IsCommand<JobsCommand>());
            var transportConfig = Container.Resolve<TransportExtensions<AzureServiceBusTransport>>();
            var routing = transportConfig.Routing();
            routing.RouteToEndpoint(typeof(JobsMessage).Assembly, EndpointNames.JobsService);
        }
    }
}
