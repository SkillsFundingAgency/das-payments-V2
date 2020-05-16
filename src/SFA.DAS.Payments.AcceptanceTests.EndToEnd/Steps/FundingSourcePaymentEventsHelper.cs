using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    public static class FundingSourcePaymentEventsHelper
    {
        public static IEnumerable<FundingSourcePaymentEvent> FundingSourcePaymentsReceivedForLearner(string priceEpisodeIdentifier, short academicYear, TestSession session)
        {
            return FundingSourcePaymentEventHandler.ReceivedEvents.Where(fspEvent =>
                fspEvent.Ukprn == session.Provider.Ukprn
                && fspEvent.Learner.Uln == session.Learner.Uln
                && fspEvent.Learner.ReferenceNumber == session.Learner.LearnRefNumber
                && fspEvent.CollectionPeriod.AcademicYear == academicYear
                && fspEvent.CollectionPeriod.Period == session.CollectionPeriod.Period
                && fspEvent.PriceEpisodeIdentifier == priceEpisodeIdentifier);
        }
    }
}