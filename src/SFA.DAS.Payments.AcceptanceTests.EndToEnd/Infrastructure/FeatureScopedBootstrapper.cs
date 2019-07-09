using System;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.AcceptanceTests.Core.Services;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Infrastructure.IoC;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Infrastructure
{

    [Binding]
    public class FeatureScopedBootstrapper : TestSessionBase
    {
        public FeatureScopedBootstrapper(FeatureContext context) : base(context)
        {
        }

        [BeforeTestRun(Order = 2)]
        public static void SetUpContainer()
        {
            Builder.Register((c, p) =>
            {
                var configHelper = c.Resolve<TestsConfiguration>();
                return new PaymentsDataContext(configHelper.PaymentsConnectionString);
            }).As<IPaymentsDataContext>().InstancePerLifetimeScope();
            DcHelper.AddDcConfig(Builder);

            Builder.RegisterType<IlrDcService>()
                   .As<IIlrService>()
                   .InstancePerLifetimeScope()
                   .IfNotRegistered(typeof(IIlrService));

            Builder.RegisterType<ApprenticeshipKeyService>().AsImplementedInterfaces();

            Builder.RegisterModule<AutoMapperModule>();
        }

        [BeforeFeature(Order = 0)]
        public static void SetUpFeature(FeatureContext context)
        {
            SetUpTestSession(context);
            var dcHelper = Container.Resolve<IDcHelper>();
            context.Set(dcHelper);
        }

        [AfterFeature(Order = 100)]
        public static void CleanUpFeature(FeatureContext context)
        {
            CleanUpTestSession(context);
        }

        [BeforeTestRun(Order = 51)]
        public static void AddRoutingConfig()
        {
            var endpointConfiguration = Container.Resolve<EndpointConfiguration>();
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(type => type.IsEvent<IPaymentsEvent>())
                .DefiningCommandsAs(t => t.IsAssignableTo<ProcessLearnerCommand>() || t.IsAssignableTo<ProcessProviderMonthEndCommand>());
            var transportConfig = Container.Resolve<TransportExtensions<AzureServiceBusTransport>>();
            var routing = transportConfig.Routing();
            routing.RouteToEndpoint(typeof(ProcessLearnerCommand), EndpointNames.EarningEvents);
            routing.RouteToEndpoint(typeof(ProcessProviderMonthEndCommand), EndpointNames.ProviderPayments);
            routing.RouteToEndpoint(typeof(RecordStartedProcessingMonthEndJob).Assembly, EndpointNames.JobMonitoring);
            routing.RouteToEndpoint(typeof(ProcessLevyPaymentsOnMonthEndCommand).Assembly, EndpointNames.FundingSource);
            routing.RouteToEndpoint(typeof(EmployerChangedProviderPriority).Assembly, EndpointNames.FundingSource);
            transportConfig.Queues().LockDuration(TimeSpan.FromMinutes(5));
            endpointConfiguration.MakeInstanceUniquelyAddressable("reply");
            endpointConfiguration.EnableCallbacks();
        }

        [AfterScenario]
        public static void MarkScenarioCompleteWithSession(FeatureContext context)
        {
            var testSession = context.Get<TestSession>();
            testSession.CompleteScenario();
        }

        [AfterScenario]
        public static void AfterScenarioLogger(FeatureContext context)
        {
            LogTestSession(context);
        }
    }
}