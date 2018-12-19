using System;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Domain
{
    public interface IFunctionalSkillsEarningProcessor
    {
        IncentivePaymentDueEvent[] HandleEarning(
            Submission submission,
            FunctionalSkillEarning earning,
            Learner learner,
            LearningAim learningAim
        );
    }
}