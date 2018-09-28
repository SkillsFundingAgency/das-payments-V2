using System.Linq;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Domain;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Application
{
    public class ApprenticeshipContractType2PayableEarningService : IApprenticeshipContractType2PayableEarningService
    {
        private readonly IApprenticeshipContractType2EarningProcessor act2EarningProcessor;

        public ApprenticeshipContractType2PayableEarningService(IApprenticeshipContractType2EarningProcessor act2EarningProcessor)
        {
            this.act2EarningProcessor = act2EarningProcessor;
        }

        public ApprenticeshipContractType2PaymentDueEvent[] CreatePaymentsDue(ApprenticeshipContractType2EarningEvent message)
        {
            return message.OnProgrammeEarnings
                .SelectMany(e => act2EarningProcessor.HandleOnProgrammeEarning(e, message.CollectionPeriod, message.Learner, message.LearningAim, message.SfaContributionPercentage))
                .ToArray();
        }
    }
}
