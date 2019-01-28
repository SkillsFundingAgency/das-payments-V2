using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Model.Core;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Messages;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class PayableEarningEventMatcher : BaseMatcher<PayableEarningEvent>
    {
        private readonly TestSession testSession;
        private readonly CollectionPeriod collectionPeriod;

        public PayableEarningEventMatcher(TestSession testSession, CollectionPeriod collectionPeriod)
        {
            this.testSession = testSession;
            this.collectionPeriod = collectionPeriod;
        }

        protected override IList<PayableEarningEvent> GetActualEvents()
        {
            return PayableEarningEventHandler.ReceivedEvents.Where(e => e.JobId == testSession.JobId
                                                                        && e.CollectionPeriod.Period == collectionPeriod.Period
                                                                        && e.CollectionYear == collectionPeriod.AcademicYear
                                                                                 && e.Ukprn == testSession.Ukprn).ToList();
        }

        protected override IList<PayableEarningEvent> GetExpectedEvents()
        {
            return null;
        }

        protected override bool Match(PayableEarningEvent expectedEvent, PayableEarningEvent actualEvent)
        {
            if (expectedEvent.GetType() != actualEvent.GetType())
                return false;

            if (expectedEvent.CollectionPeriod.Period != actualEvent.CollectionPeriod.Period ||
                expectedEvent.CollectionPeriod.AcademicYear != actualEvent.CollectionPeriod.AcademicYear ||
                expectedEvent.Learner.ReferenceNumber != actualEvent.Learner.ReferenceNumber ||
                expectedEvent.LearningAim.Reference != actualEvent.LearningAim.Reference ||
                expectedEvent.LearningAim.FundingLineType != actualEvent.LearningAim.FundingLineType ||
                expectedEvent.LearningAim.FrameworkCode != actualEvent.LearningAim.FrameworkCode ||
                expectedEvent.LearningAim.PathwayCode != actualEvent.LearningAim.PathwayCode ||
                expectedEvent.LearningAim.ProgrammeType != actualEvent.LearningAim.ProgrammeType ||
                expectedEvent.LearningAim.StandardCode != actualEvent.LearningAim.StandardCode ||
                expectedEvent.Priority != actualEvent.Priority ||
                expectedEvent.EmployerAccountId != actualEvent.EmployerAccountId ||
                expectedEvent.CommitmentId != actualEvent.CommitmentId ||
                expectedEvent.CommitmentVersion != actualEvent.CommitmentVersion)
                return false;

            return true;
        }
    }
}