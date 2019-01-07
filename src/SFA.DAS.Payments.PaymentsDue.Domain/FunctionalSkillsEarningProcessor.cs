using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Domain
{
    public class FunctionalSkillsEarningProcessor : IFunctionalSkillsEarningProcessor
    {
        public IncentivePaymentDueEvent[] HandleEarning(
            Submission submission,
            FunctionalSkillEarning functionalSkillEarning,
            Learner learner,
            LearningAim learningAim,
            ContractType contractType
        )
        {
            var paymentsDue = new List<IncentivePaymentDueEvent>();

            foreach (var period in functionalSkillEarning.Periods.Where(earning => earning.Period <= submission.CollectionPeriod.Period))
            {
                paymentsDue.Add(new IncentivePaymentDueEvent
                {
                    JobId = submission.JobId,
                    Ukprn = submission.Ukprn,
                    DeliveryPeriod = DeliveryPeriod.CreateFromAcademicYearAndPeriod(submission.CollectionPeriod.AcademicYear, period.Period),
                    LearningAim = learningAim.Clone(),
                    Learner = learner.Clone(),
                    Type = (IncentivePaymentType) functionalSkillEarning.Type,
                    AmountDue = period.Amount,
                    CollectionPeriod = submission.CollectionPeriod,
                    EventTime = DateTimeOffset.UtcNow,
                    PriceEpisodeIdentifier = period.PriceEpisodeIdentifier,
                    IlrSubmissionDateTime = submission.IlrSubmissionDate,
                    ContractType = contractType
                });
            }

            return paymentsDue.ToArray();
        }
    }
}