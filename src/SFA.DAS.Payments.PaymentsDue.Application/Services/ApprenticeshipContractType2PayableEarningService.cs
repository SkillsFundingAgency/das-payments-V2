using System.Linq;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Domain;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Application.Services
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
            var messages = message.OnProgrammeEarnings
                .SelectMany(earning => act2EarningProcessor.HandleOnProgrammeEarning(message.Ukprn, message.JobId, earning, message.CollectionPeriod, message.Learner, message.LearningAim, message.SfaContributionPercentage, message.IlrSubmissionDateTime))
                .ToList();
            messages.ForEach(msg => msg.EarningEventId = message.EventId);
            return messages.ToArray();
        }
    }
}
