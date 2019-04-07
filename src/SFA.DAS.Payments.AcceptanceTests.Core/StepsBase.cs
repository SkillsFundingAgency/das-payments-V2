using NUnit.Framework;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Tests.Core.Builders;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Core
{
    public abstract class StepsBase : BindingsBase
    {
        public SpecFlowContext Context { get; }
        public TestSession TestSession { get => Get<TestSession>(); set => Set(value); }
        public ILifetimeScope Scope => Get<ILifetimeScope>("container_scope");
        protected short AcademicYear { get => Get<short>("academic_year"); set => Set(value, "academic_year"); }
        protected byte CollectionPeriod { get => Get<byte>("collection_period"); set => Set(value, "collection_period"); }
        public static bool IsDevEnvironment => (Environment?.Equals("DEVELOPMENT", StringComparison.OrdinalIgnoreCase) ?? false) ||
                                        (Environment?.Equals("LOCAL", StringComparison.OrdinalIgnoreCase) ?? false);
        protected decimal SfaContributionPercentage { get => Get<decimal>("sfa_contribution_percentage"); set => Set(value, "sfa_contribution_percentage"); }
        protected ContractType ContractType { get => Get<ContractType>("contract_type"); set => Set(value, "contract_type"); }

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
                else
                {
                    if (lastRun) break;
                }

                await Task.Delay(Config.TimeToPause);
            }
            Assert.Fail($"{failText}  Time: {DateTime.Now:G}.  Ukprn: {TestSession.Ukprn}. Job Id: {TestSession.JobId}");
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
                else
                {
                    if (lastRun) break;
                }

                await Task.Delay(Config.TimeToPause);
            }
            Assert.Fail($"{failText}  Time: {DateTime.Now:G}.  Ukprn: {TestSession.Ukprn}. Job Id: {TestSession.JobId}");
        }

        protected async Task WaitForIt(Func<(bool pass, string reason, bool final)> lookForIt, string failText)
        {
            var endTime = DateTime.Now.Add(Config.TimeToWait);
            var reason = string.Empty;
            var lastRun = false;

            while (DateTime.Now < endTime || lastRun)
            {
                bool pass, final;
                (pass, reason, final) = lookForIt();
                if (pass)
                {
                    if (lastRun) return;
                    lastRun = true;
                }
                else
                {
                    if (lastRun || final) break;
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
            AcademicYear = new CollectionPeriodBuilder().WithDate(DateTime.Today).Build().AcademicYear;
        }

        public async Task CreateTestEarningsJob(DateTimeOffset startTime, List<IPaymentsEvent> payments,
               JobType jobType = JobType.ComponentAcceptanceTestEarningsJob)
        {
            await CreateJob(startTime,
                payments.Select(payment => new GeneratedMessage
                {
                    StartTime = payment.EventTime,
                    MessageName = payment.GetType().FullName,
                    MessageId = payment.EventId
                }).ToList());
        }

        public async Task CreateJob(DateTimeOffset startTime, List<GeneratedMessage> generatedMessages, JobType jobType = JobType.ComponentAcceptanceTestEarningsJob)
        {
            var job = new JobModel
            {
                CollectionPeriod = CollectionPeriod,
                AcademicYear = AcademicYear,
                StartTime = startTime,
                Ukprn = TestSession.Ukprn,
                DcJobId = TestSession.JobId,
                IlrSubmissionTime = TestSession.IlrSubmissionTime,
                JobType = JobType.ComponentAcceptanceTestEarningsJob,
                LearnerCount = generatedMessages.Count,
                Status = JobStatus.InProgress
            };
            var dataContext = Scope.Resolve<JobsDataContext>();
            dataContext.Jobs.Add(job);
            await dataContext.SaveChangesAsync();
            Console.WriteLine($"Saved new test job to database. Job Id: {job.Id}");
            dataContext.JobSteps.AddRange(generatedMessages.Select(msg => new JobStepModel
            {
                JobId = job.Id,
                StartTime = msg.StartTime,
                MessageName = msg.MessageName,
                MessageId = msg.MessageId,
                Status = JobStepStatus.Queued
            }));
            await dataContext.SaveChangesAsync();
            Console.WriteLine($"Finished creating job and generated messages. Job id: {job.Id}, Test DC Job id: {job.DcJobId}");
        }
    }
}