using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Domain
{
    public class ApprenticeshipContractType2EarningProcessor : IApprenticeshipContractType2EarningProcessor
    {
        public ApprenticeshipContractTypePaymentDueEvent[] HandleOnProgrammeEarning(long ukprn, long jobId,
            OnProgrammeEarning onProgEarning,
            CalendarPeriod collectionPeriod, Learner learner, LearningAim learningAim,
            decimal sfaContributionPercentage, DateTime ilrSubmissionDate)
        {
            if (onProgEarning == null)
                throw new ArgumentNullException(nameof(onProgEarning));

            if (collectionPeriod == null)
                throw new ArgumentNullException(nameof(collectionPeriod));

            if (learner == null)
                throw new ArgumentNullException(nameof(learner));

            if (learningAim == null)
                throw new ArgumentNullException(nameof(learningAim));

            var paymentsDue = new List<ApprenticeshipContractType2PaymentDueEvent>();

            foreach (var period in onProgEarning.Periods.Where(earning => earning.Period <= collectionPeriod.Period))
            {
                paymentsDue.Add(new ApprenticeshipContractType2PaymentDueEvent
                {
                    JobId = jobId,
                    Ukprn = ukprn,
                    DeliveryPeriod = new CalendarPeriod(collectionPeriod.Name.Split('-').FirstOrDefault(), period.Period),  //TODO: yuck!! fix when the CalendarPeriod class is reworked.
                    LearningAim = learningAim.Clone(),
                    Learner = learner.Clone(),
                    Type = onProgEarning.Type,
                    SfaContributionPercentage = sfaContributionPercentage,
                    AmountDue = period.Amount,
                    CollectionPeriod = collectionPeriod,
                    EventTime = DateTimeOffset.UtcNow,
                    PriceEpisodeIdentifier = period.PriceEpisodeIdentifier,
                    IlrSubmissionDateTime = ilrSubmissionDate
                });
            }

            return paymentsDue.Cast<ApprenticeshipContractTypePaymentDueEvent>().ToArray();
        }
    }
}
