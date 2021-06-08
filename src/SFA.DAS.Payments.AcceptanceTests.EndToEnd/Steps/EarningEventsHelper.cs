using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    public static class EarningEventsHelper
    {
        public static IEnumerable<PayableEarningEvent> PayableEarningsReceivedForLearner(TestSession session)
        {
            return PayableEarningEventHandler.ReceivedEvents.Where(earningEvent =>
                earningEvent.Ukprn == session.Provider.Ukprn
                && earningEvent.Learner.Uln == session.Learner.Uln
                && earningEvent.Learner.ReferenceNumber == session.Learner.LearnRefNumber
            );
        }

        public static IEnumerable<EarningEvent> EarningEventsReceivedForLearner(TestSession session)
        {
            return EarningEventHandler.ReceivedEvents.Where(earningEvent =>
                earningEvent.Ukprn == session.Provider.Ukprn
                && earningEvent.Learner.Uln == session.Learner.Uln
                && earningEvent.Learner.ReferenceNumber == session.Learner.LearnRefNumber
            );
        }

        public static IEnumerable<DataLockErrorCode> GetOnProgrammeDataLockErrorsForLearnerAndPriceEpisode(string priceEpisodeIdentifier, short academicYear, TestSession session)
        {
            return EarningFailedDataLockMatchingHandler
                .ReceivedEvents
                .Where(dataLockEvent =>
                    dataLockEvent.Ukprn == session.Provider.Ukprn
                    && dataLockEvent.Learner.Uln == session.Learner.Uln
                    && dataLockEvent.Learner.ReferenceNumber == session.Learner.LearnRefNumber
                    && dataLockEvent.CollectionYear == academicYear
                    && dataLockEvent.CollectionPeriod.Period == session.CollectionPeriod.Period
                    && dataLockEvent.PriceEpisodes.Any(episode => episode.Identifier == priceEpisodeIdentifier))
                .SelectMany(dataLockEvent =>
                    dataLockEvent.OnProgrammeEarnings.SelectMany(earning => earning.Periods.SelectMany(period => period.DataLockFailures.Select(a => a.DataLockError))));
        }

        public static IEnumerable<DataLockErrorCode> GetOnProgrammeDataLockErrorsForLearnerAndPriceEpisodeAndDeliveryPeriod(string priceEpisodeIdentifier, short academicYear, TestSession session, byte deliveryPeriod)
        {
            return EarningFailedDataLockMatchingHandler
                .ReceivedEvents
                .Where(dataLockEvent =>
                    dataLockEvent.Ukprn == session.Provider.Ukprn
                    && dataLockEvent.Learner.Uln == session.Learner.Uln
                    && dataLockEvent.Learner.ReferenceNumber == session.Learner.LearnRefNumber
                    && dataLockEvent.CollectionYear == academicYear
                    && dataLockEvent.CollectionPeriod.Period == session.CollectionPeriod.Period
                    && dataLockEvent.PriceEpisodes.Any(episode => episode.Identifier == priceEpisodeIdentifier))
                .SelectMany(dataLockEvent =>
                    dataLockEvent.OnProgrammeEarnings.SelectMany(earning => earning.Periods.Where(period => period.Period == deliveryPeriod).SelectMany(period => period.DataLockFailures.Select(a => a.DataLockError))));
        }
    }
}