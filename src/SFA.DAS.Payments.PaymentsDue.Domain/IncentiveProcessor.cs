using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Domain
{
    public class IncentiveProcessor : IIncentiveProcessor
    {
        public IncentivePaymentDueEvent[] HandleIncentiveEarnings(
            Submission submission,
            IncentiveEarning incentiveEarning,
            Learner learner,
            LearningAim learningAim,
            decimal sfaContributionPercentage, ContractType contractType)
        {
            if (incentiveEarning == null)
                throw new ArgumentNullException(nameof(incentiveEarning));

            if (submission.CollectionPeriod == null)
                throw new ArgumentNullException(nameof(submission.CollectionPeriod));

            if (learner == null)
                throw new ArgumentNullException(nameof(learner));

            if (learningAim == null)
                throw new ArgumentNullException(nameof(learningAim));

            var paymentsDue = new List<IncentivePaymentDueEvent>();

            foreach (var period in incentiveEarning.Periods.Where(earning => earning.Period <= submission.CollectionPeriod.Period))
            {
                paymentsDue.Add(new IncentivePaymentDueEvent
                {
                    JobId = submission.JobId,
                    Ukprn = submission.Ukprn,
                    DeliveryPeriod = new CalendarPeriod(submission.CollectionPeriod.AcademicYear, period.Period),
                    LearningAim = learningAim.Clone(),
                    Learner = learner.Clone(),
                    AmountDue = period.Amount,
                    CollectionPeriod = submission.CollectionPeriod,
                    EventTime = DateTimeOffset.UtcNow,
                    PriceEpisodeIdentifier = period.PriceEpisodeIdentifier,
                    IlrSubmissionDateTime = submission.IlrSubmissionDate,
                    Type = (IncentivePaymentType) incentiveEarning.Type
                    ContractType = contractType
                });
            }

            return paymentsDue.ToArray();
        }
    }
}