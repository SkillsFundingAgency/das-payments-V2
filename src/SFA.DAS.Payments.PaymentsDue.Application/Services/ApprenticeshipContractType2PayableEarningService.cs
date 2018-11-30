using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Domain;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Application.Services
{
    public class ApprenticeshipContractType2PayableEarningService : IApprenticeshipContractType2PayableEarningService
    {
        private readonly IApprenticeshipContractType2EarningProcessor act2EarningProcessor;
        private readonly IApprenticeshipContractTypeIncentiveProcessor incentiveEarningProcessor;

        public ApprenticeshipContractType2PayableEarningService(
            IApprenticeshipContractType2EarningProcessor act2EarningProcessor,
            IApprenticeshipContractTypeIncentiveProcessor incentiveEarningProcessor)
        {
            this.act2EarningProcessor = act2EarningProcessor;
            this.incentiveEarningProcessor = incentiveEarningProcessor;
        }

        public PaymentDueEvent[] CreatePaymentsDue(ApprenticeshipContractType2EarningEvent message)
        {
            var onProgEvents = message.OnProgrammeEarnings
                .SelectMany(earning => act2EarningProcessor.HandleOnProgrammeEarning(message.Ukprn, message.JobId,
                    earning, message.CollectionPeriod, message.Learner, message.LearningAim,
                    message.SfaContributionPercentage, message.IlrSubmissionDateTime)).ToList();

            var incentiveEvents = message.IncentiveEarnings
                .SelectMany(earning => incentiveEarningProcessor.HandleIncentiveEarnings(message.Ukprn, message.JobId,
                    earning, message.CollectionPeriod, message.Learner, message.LearningAim,
                    message.SfaContributionPercentage, message.IlrSubmissionDateTime)).ToList();

            var result = new List<PaymentDueEvent>();
            result.AddRange(onProgEvents);
            result.AddRange(incentiveEvents);

            return result.ToArray();
        }
    }
}
