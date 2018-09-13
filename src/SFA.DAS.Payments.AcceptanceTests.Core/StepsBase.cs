using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Core
{
    [Binding]
    public abstract class StepsBase
    {
        public static ContainerBuilder Builder { get; protected set; }
        public static IContainer Container { get; protected set; }
        public static IMessageSession MessageSession { get; protected set; }
        public TestsConfiguration Config => Container.Resolve<TestsConfiguration>();
        public string Environment => Config.GetAppSetting("Environment");

        public bool IsDevEnvironment => (Environment?.Equals("DEVELOPMENT", StringComparison.OrdinalIgnoreCase) ?? false) ||
                                        (Environment?.Equals("LOCAL", StringComparison.OrdinalIgnoreCase) ?? false);

        protected List<IEarningEvent> EarningEvents { get => Get<List<IEarningEvent>>(); set => Set(value); }
        public T Get<T>(string key = null)// where T : class
        {
            return key == null ? ScenarioContext.Current.Get<T>() : ScenarioContext.Current.Get<T>(key);
        }

        public void Set<T>(T item, string key = null)
        {
            if (key == null)
                ScenarioContext.Current.Set(item);
            else
                ScenarioContext.Current.Set(item, key);
        }

        protected void WaitForIt(Func<bool> lookForIt, string failText)
        {
            var endTime = DateTime.Now.Add(Config.TimeToWait);
            while (DateTime.Now < endTime)
            {
                if (lookForIt())
                    return;
                Thread.Sleep(Config.TimeToPause);
            }
            Assert.Fail(failText);
        }

        protected void WaitForIt(Func<Tuple<bool, string>> lookForIt, string failText)
        {
            var endTime = DateTime.Now.Add(Config.TimeToWait);
            var reason = "";
            var pass = false;
            while (DateTime.Now < endTime)
            {
                (pass, reason) = lookForIt();
                if (pass)
                    return;
                Thread.Sleep(Config.TimeToPause);
            }
            Assert.Fail(failText + " - " + reason);
        }

        protected bool WaitForIt(Func<bool> lookForIt)
        {
            var endTime = DateTime.Now.Add(Config.TimeToWait);
            while (DateTime.Now < endTime)
            {
                if (lookForIt())
                    return true;
                Thread.Sleep(Config.TimeToPause);
            }
            return false;
        }
    }
}
