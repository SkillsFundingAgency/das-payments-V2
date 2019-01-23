using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Model.Core;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Messages;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class PayableEarningEventMatcher : BaseMatcher<PayableEarning>
    {
        private readonly TestSession testSession;
        private readonly CollectionPeriod collectionPeriod;

        public PayableEarningEventMatcher(TestSession testSession, CollectionPeriod collectionPeriod)
        {
            this.testSession = testSession;
            this.collectionPeriod = collectionPeriod;
        }

        protected override IList<PayableEarning> GetActualEvents()
        {
            return PayableEarningEventHandler.ReceivedEvents.Where(e => e.JobId == testSession.JobId
                                                                                 && e.CollectionPeriod.Name == collectionPeriod.Name
                                                                                 && e.Ukprn == testSession.Ukprn).ToList();
        }

        protected override IList<PayableEarning> GetExpectedEvents()
        {
            return null;
        }

        protected override bool Match(PayableEarning expectedEvent, PayableEarning actualEvent)
        {
            return true;
        }
    }
}