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
        public ApprenticeshipContractTypePaymentDueEvent[] HandleOnProgrammeEarning(
            Submission submission,
            OnProgrammeEarning onProgEarning,
            Learner learner,
            LearningAim learningAim,
            decimal sfaContributionPercentage)
        {
            if (onProgEarning == null)
                throw new ArgumentNullException(nameof(onProgEarning));

            if (submission == null)
                throw new ArgumentNullException(nameof(submission));

            if (submission.CollectionPeriod == null)
                throw new ArgumentNullException(nameof(submission.CollectionPeriod));

            if (learner == null)
                throw new ArgumentNullException(nameof(learner));

            if (learningAim == null)
                throw new ArgumentNullException(nameof(learningAim));

            var paymentsDue = new List<ApprenticeshipContractType2PaymentDueEvent>();

            foreach (var period in onProgEarning.Periods.Where(earning => earning.Period <= submission.CollectionPeriod.Period))
            {
                paymentsDue.Add(new ApprenticeshipContractType2PaymentDueEvent
                {
                    JobId = submission.JobId,
                    Ukprn = submission.Ukprn,
                    DeliveryPeriod = DeliveryPeriod.CreateFromAcademicYearAndPeriod(submission.CollectionPeriod.AcademicYear, period.Period),
                    LearningAim = learningAim.Clone(),
                    Learner = learner.Clone(),
                    Type = onProgEarning.Type,
                    SfaContributionPercentage = period.SfaContributionPercentage ?? sfaContributionPercentage,
                    AmountDue = period.Amount,
                    CollectionPeriod = submission.CollectionPeriod,
                    EventTime = DateTimeOffset.UtcNow,
                    PriceEpisodeIdentifier = period.PriceEpisodeIdentifier,
                    IlrSubmissionDateTime = submission.IlrSubmissionDate
                });
            }

            return paymentsDue.Cast<ApprenticeshipContractTypePaymentDueEvent>().ToArray();
        }
    }
}
