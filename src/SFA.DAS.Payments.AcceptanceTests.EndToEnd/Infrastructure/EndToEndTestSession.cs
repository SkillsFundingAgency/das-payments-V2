using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.Application.Repositories;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Infrastructure
{


    [Binding]
    public class EndToEndTestSession : TestSessionBase
    {
        public EndToEndTestSession(FeatureContext context) : base(context)
        {
        }

        [BeforeTestRun(Order = 2)]
        public static void SetUpContainer()
        {
            Builder.Register((c, p) =>
            {
                var configHelper = c.Resolve<TestsConfiguration>();
                return new PaymentsDataContext(configHelper.PaymentsConnectionString);
            }).As<IPaymentsDataContext>().InstancePerDependency();
            DcHelper.AddDcConfig(Builder);
        }

        [BeforeFeature(Order = 0)]
        public static void SetUpFeature(FeatureContext context)
        {
            SetUpTestSession(context);
            var dcHelper = new DcHelper(Container);
            context.Set(dcHelper);
        }


        [BeforeFeature(Order = 99)]
        public static void CleanUpFeature(FeatureContext context)
        {
            CleanUpTestSession(context);
        }
    }
}