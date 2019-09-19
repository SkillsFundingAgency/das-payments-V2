using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Audit.AcceptanceTests.Data;
using SFA.DAS.Payments.Audit.AcceptanceTests.Data.Entities;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.Audit.AcceptanceTests.DataLock
{
    [Binding]
    public class DataLockSteps : StepsBase
    {
        public DataLockSteps(ScenarioContext context) : base(context)
        {
        }

        [Given(@"the data lock service has generated the following events")]
        public void GivenTheDataLockServiceHasGeneratedTheFollowingEvents(Table table)
        {
            var dataContext = Container.Resolve<AuditDataContext>();
            var submissions = table.CreateSet<SubmissionData>();

            foreach (var submission in submissions)
            {
                var dataLockEvent = new DataLockEvent
                {
                    EventId = Guid.NewGuid(),
                    EarningEventId = Guid.NewGuid(),
                    IlrSubmissionDateTime = submission.SubmissionTime,
                    LearnerReferenceNumber = submission.Learner,
                    LearningAimReference = "1",
                    LearningAimFundingLineType = "2",                    
                    Ukprn = TestSession.Ukprn,
                    CollectionPeriod = 1,
                    AcademicYear = 1
                };

                var dataLockPayablePeriod = new DataLockPayablePeriod
                {
                    DataLockEventId = dataLockEvent.EventId
                };

                dataContext.DataLockEvents.Add(dataLockEvent);
                dataContext.DataLockPayablePeriods.Add(dataLockPayablePeriod);
            }

            dataContext.SaveChanges();
        }

        [When(@"submission failed event for ILR submitted at '(.*)' arrives")]
        public async Task WhenSubmissionFailedEventForILRSubmittedAtArrives(string p0)
        {
            var submissionFailedEvent = new SubmissionFailedEvent
            {
                AcademicYear = 1,
                CollectionPeriod = 1,
                Ukprn = TestSession.Ukprn,
                IlrSubmissionDateTime = DateTime.Parse(p0)
            };

            await MessageSession.Send(submissionFailedEvent).ConfigureAwait(false);
        }

        [When(@"submission succeeded event for ILR submitted at '(.*)' arrives")]
        public async Task WhenSubmissionSucceededEventForILRSubmittedAtArrives(string p0)
        {
            var submissionSucceededEvent = new SubmissionSucceededEvent
            {
                AcademicYear = 1,
                CollectionPeriod = 1,
                Ukprn = TestSession.Ukprn,
                IlrSubmissionDateTime = DateTime.Parse(p0)
            };

            await MessageSession.Send(submissionSucceededEvent).ConfigureAwait(false);
        }

        [Then(@"only the following events stay in database")]
        public async Task ThenOnlyTheFollowingEventsStayInDatabase(Table table)
        {
            var dataContext = Container.Resolve<AuditDataContext>();
            var submissions = table.CreateSet<SubmissionData>()
                .OrderBy(e => e.SubmissionTime)
                .ThenBy(e => e.Learner)
                .ToList();

            await WaitForIt(() =>
            {
                var allEvents = dataContext.DataLockEvents
                    .Where(e => e.Ukprn == TestSession.Ukprn)
                    .OrderBy(e => e.IlrSubmissionDateTime)
                    .ThenBy(e => e.LearnerReferenceNumber)
                    .AsNoTracking()
                    .ToList();

                if (allEvents.Count != submissions.Count)
                    return false;

                for (var i = 0; i < submissions.Count; i++)
                {
                    if (submissions[i].SubmissionTime != allEvents[i].IlrSubmissionDateTime)
                        return false;
                    if (submissions[i].Learner != allEvents[i].LearnerReferenceNumber)
                        return false;
                }

                return true;
            }, "Data Lock Event check failure").ConfigureAwait(false);
        }

    }
}
