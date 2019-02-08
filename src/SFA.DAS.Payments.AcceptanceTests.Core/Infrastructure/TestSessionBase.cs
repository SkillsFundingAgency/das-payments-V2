using System;
using System.Diagnostics;
using Autofac;
using NServiceBus;
using NUnit.Framework;
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
            var scope = Container.BeginLifetimeScope(builder => builder.RegisterInstance<IMessageSession>(MessageSession));
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

        protected static void LogTestSession(SpecFlowContext context)
        {
            var testSession = context.Get<TestSession>();
            Trace.WriteLine("Finished scenario with:\n" +
                              $"Ukprn: {testSession.Ukprn}\n" +
                              $"Job ID: {testSession.JobId}\n" +
                              $"Learners:\n" +
                              $"\t{string.Join("\n\t", testSession.Learners)}");
        }
    }
}