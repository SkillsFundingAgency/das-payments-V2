using Autofac;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Events;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Infrastructure
{
    [Binding]
    public class FeatureScopedBootstrapper : BindingBootstrapper
    {
        public FeatureScopedBootstrapper(FeatureContext context) : base(context)
        {
        }

        [BeforeTestRun(Order = 2)]
        public static void AddDcConfig()
        {
            DcHelper.AddDcConfig(Builder);
        }

        [BeforeFeature(Order = 0)]
        public static void SetUpFeature(FeatureContext context)
        {
            SetUpTestSession(context);
        }

        [BeforeFeature(Order = 99)]
        public static void CleanUpFeature(FeatureContext context)
        {
            CleanUpTestSession(context);
        }

        [BeforeTestRun(Order = 51)]
        public static void AddRoutingConfig()
        {
            var endpointConfiguration = Container.Resolve<EndpointConfiguration>();
            endpointConfiguration.Conventions().DefiningEventsAs(type => type.IsEvent<IPaymentsEvent>());
        }
    }
}