using NServiceBus;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class RequiredPaymentsCacheCleaner
    {
        private readonly IApprenticeshipKeyService apprenticeshipKeyService;
        private readonly IMessageSession messageSession;

        public RequiredPaymentsCacheCleaner(IApprenticeshipKeyService apprenticeshipKeyService, IMessageSession messageSession)
        {
            this.apprenticeshipKeyService = apprenticeshipKeyService;
            this.messageSession = messageSession;
        }

        public async Task ClearCaches(TestSession testSession)
        {
            var startedEvent = new CollectionStartedEvent
            {
                ApprenticeshipKeys = testSession.Learners.Select(learner =>
                {
                    return apprenticeshipKeyService.GenerateApprenticeshipKey(
                        testSession.Ukprn,
                        testSession.GetLearner(learner.LearnerIdentifier).LearnRefNumber,
                        learner.Course.FrameworkCode,
                        learner.Course.PathwayCode,
                        learner.Course.ProgrammeType,
                        learner.Course.StandardCode,
                        learner.Course.LearnAimRef);
                }).ToList().AsReadOnly()
            };

            await messageSession.Send(startedEvent).ConfigureAwait(false);
        }
    }
}
