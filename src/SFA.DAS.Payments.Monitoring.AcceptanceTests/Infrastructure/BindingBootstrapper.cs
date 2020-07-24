using Autofac;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
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
            endpointConfiguration.Conventions().DefiningEventsAs(type => type.Namespace == "SFA.DAS.Payments.Monitoring.Jobs.Messages.Events");
            var transportConfig = Container.Resolve<TransportExtensions<AzureServiceBusTransport>>();
            var routing = transportConfig.Routing();
            routing.RouteToEndpoint(typeof(JobsMessage).Assembly, "SFA.DAS.Payments.Monitoring.Jobs.Messages", EndpointNames.JobsService);
        }

        [BeforeTestRun(Order = 40)]
        public static void SetUpJobsDataContext()
        {
            Builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<TestsConfiguration>();
                    return new JobsDataContext(configHelper.PaymentsConnectionString);
                })
                .InstancePerLifetimeScope();

            Builder.RegisterBuildCallback(c => c.Resolve<JobsDataContext>());
        }
    }
}
