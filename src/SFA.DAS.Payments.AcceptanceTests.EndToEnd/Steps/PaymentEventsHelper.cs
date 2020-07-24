using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    public static class PaymentEventsHelper
    {
        public static IEnumerable<ProviderPaymentEvent> ProviderPaymentsReceivedForLearner(string priceEpisodeIdentifier, short academicYear, TestSession session)
        {
            return ProviderPaymentEventHandler.ReceivedEvents.Where(ppEvent =>
                ppEvent.Ukprn == session.Provider.Ukprn
                && ppEvent.Learner.Uln == session.Learner.Uln
                && ppEvent.Learner.ReferenceNumber == session.Learner.LearnRefNumber
                && ppEvent.CollectionPeriod.AcademicYear == academicYear
                && ppEvent.CollectionPeriod.Period == session.CollectionPeriod.Period
                && ppEvent.PriceEpisodeIdentifier == priceEpisodeIdentifier);
        }
    }
}