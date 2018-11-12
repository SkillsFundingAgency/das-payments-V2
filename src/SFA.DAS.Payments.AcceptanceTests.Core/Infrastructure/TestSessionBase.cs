using System;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure
{
    public abstract class TestSessionBase : StepsBase
    {
        protected TestSessionBase(SpecFlowContext context) : base(context)
        {
        }
        protected static void SetUpTestSession(SpecFlowContext context)
        {
            var scope = Container.BeginLifetimeScope();
            context.Set(scope, "container_scope");
            var testSession = new TestSession();
            context.Set(testSession);
            Console.WriteLine($"Created test session: Ukprn: {testSession.Ukprn}, Job Id: {testSession.JobId}");
        }

        protected static void CleanUpTestSession(SpecFlowContext context)
        {
            if (!context.ContainsKey("container_scope"))
                return;
            var scope = context.Get<ILifetimeScope>("container_scope");
            context.Remove("container_scope");
            scope.Dispose();
        }
    }
}