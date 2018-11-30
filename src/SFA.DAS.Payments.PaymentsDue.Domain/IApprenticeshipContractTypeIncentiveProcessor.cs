using System;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Domain
{
    public interface IApprenticeshipContractTypeIncentiveProcessor
    {
        PaymentDueEvent[] HandleIncentiveEarnings(long ukprn, long jobId,
            IncentiveEarning incentiveEarning, CalendarPeriod collectionPeriod, Learner learner,
            LearningAim learningAim, decimal sfaContributionPercentage, DateTime ilrSubmissionDate);
    }
}