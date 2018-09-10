using Autofac;
using NServiceBus;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Core
{
    [Binding]
    public abstract class StepsBase
    {
        public static ContainerBuilder Builder { get; protected set; }
        public static IContainer Container { get; protected set; }
        public static IMessageSession MessageSession { get; protected set; }
    }
}
