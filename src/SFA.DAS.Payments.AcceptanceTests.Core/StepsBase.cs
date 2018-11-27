using NUnit.Framework;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Core
{
    public abstract class StepsBase : BindingsBase
    {
        public SpecFlowContext Context { get; }
        public TestSession TestSession { get => Get<TestSession>(); set => Set(value); }
        protected string CollectionYear { get => Get<string>("collection_year"); set => Set(value, "collection_year"); }
        protected byte CollectionPeriod { get => Get<byte>("collection_period"); set => Set(value, "collection_period"); }
        public static bool IsDevEnvironment => (Environment?.Equals("DEVELOPMENT", StringComparison.OrdinalIgnoreCase) ?? false) ||
                                        (Environment?.Equals("LOCAL", StringComparison.OrdinalIgnoreCase) ?? false);
        protected decimal SfaContributionPercentage { get => Get<decimal>("sfa_contribution_percentage"); set => Set(value, "sfa_contribution_percentage"); }
        protected byte ContractType { get => Get<byte>("contract_type"); set => Set(value, "contract_type"); }

        protected StepsBase(SpecFlowContext context)
        {
            Context = context;
        }

        public T Get<T>(string key = null)// where T : class
        {
            return key == null ? Context.Get<T>() : Context.Get<T>(key);
        }

        public void Set<T>(T item, string key = null)
        {
            if (key == null)
                Context.Set(item);
            else
                Context.Set(item, key);
        }

        protected async Task WaitForIt(Func<Task<bool>> lookForIt, string failText)
        {
            var endTime = DateTime.Now.Add(Config.TimeToWait);
            var lastRun = false;

            while (DateTime.Now < endTime || lastRun)
            {
                if (await lookForIt())
                {
                    if (lastRun) return;
                    lastRun = true;
                }

                await Task.Delay(Config.TimeToPause);
            }
            Assert.Fail(failText);
        }

        protected async Task WaitForIt(Func<bool> lookForIt, string failText)
        {
            var endTime = DateTime.Now.Add(Config.TimeToWait);
            var lastRun = false;

            while (DateTime.Now < endTime || lastRun)
            {
                if (lookForIt())
                {
                    if (lastRun) return;
                    lastRun = true;
                }

                await Task.Delay(Config.TimeToPause);
            }
            Assert.Fail($"{failText}  Time: {DateTime.Now:G}.  Ukprn: {TestSession.Ukprn}. Job Id: {TestSession.JobId}");
        }

        protected async Task WaitForIt(Func<Tuple<bool, string>> lookForIt, string failText)
        {
            var endTime = DateTime.Now.Add(Config.TimeToWait);
            var reason = string.Empty;
            var lastRun = false;

            while (DateTime.Now < endTime || lastRun)
            {
                bool pass;
                (pass, reason) = lookForIt();
                if (pass)
                {
                    if (lastRun) return;
                    lastRun = true;
                }
                await Task.Delay(Config.TimeToPause);
            }

            Assert.Fail($"{failText} - {reason}  Time: {DateTime.Now:G}.  Ukprn: {TestSession.Ukprn}. Job Id: {TestSession.JobId}");
        }

        protected async Task WaitForUnexpected(Func<(bool pass, string reason)> findUnexpected, string failText)
        {
            var endTime = DateTime.Now.Add(Config.TimeToWaitForUnexpected);
            while (DateTime.Now < endTime)
            {
                var (pass, reason) = findUnexpected();
                if (!pass)
                {
                    Assert.Fail($"{failText} - {reason}   Time: {DateTime.Now:G}.  Ukprn: {TestSession.Ukprn}. Job Id: {TestSession.JobId}");
                }

                await Task.Delay(Config.TimeToPause);
            }
        }

        protected byte GetMonth(byte period)
        {
            return (byte)(period >= 5 ? period - 4 : period + 8);
        }

        protected short GetYear(byte period, string year)
        {
            var part = year.Substring(period < 5 ? 0 : 2, 2);
            return (short)(short.Parse(part) + 2000);
        }

        protected void SetCurrentCollectionYear()
        {
            var year = DateTime.Today.Year - 2000;
            CollectionYear = DateTime.Today.Month < 9 ? $"{year - 1}{year}" : $"{year}{year + 1}";
        }
    }
}