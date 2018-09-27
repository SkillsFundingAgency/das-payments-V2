using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Domain.Entities;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Domain
{
    public interface IApprenticeshipContractType2EarningProcessor
    {
        ApprenticeshipContractType2PaymentDueEvent[] HandleOnProgrammeEarning(OnProgrammeEarning onProgEarning, CalendarPeriod collectionPeriod, Learner learner, LearningAim learningAim, decimal sfaContributionPercentage);
    }
}