using System.Linq;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Domain;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Application.Services
{
    public class FunctionalSkillsEarningService : IFunctionalSkillsEarningService
    {
        private readonly IFunctionalSkillsEarningProcessor earningProcessor;

        public FunctionalSkillsEarningService(IFunctionalSkillsEarningProcessor earningProcessor)
        {
            this.earningProcessor = earningProcessor;
        }

        public IncentivePaymentDueEvent[] CreatePaymentsDue(FunctionalSkillEarningsEvent message)
        {
            var submission = new Submission
            {
                JobId = message.JobId,
                Ukprn = message.Ukprn,
                CollectionPeriod = message.CollectionPeriod,
                IlrSubmissionDate = message.IlrSubmissionDateTime
            };

            return message.Earnings.SelectMany(earning => earningProcessor.HandleEarning(
                submission,
                earning,
                message.Learner,
                message.LearningAim
            )).ToArray();
        }
    }
}