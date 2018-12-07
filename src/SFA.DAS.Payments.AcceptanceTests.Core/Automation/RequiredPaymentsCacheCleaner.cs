using System.Collections.Generic;
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
            var keys = new List<string>();

            if (testSession.Learners.Any(x => x.Aims.Count > 0))
            {
                foreach (var learner in testSession.Learners)
                {
                    foreach (var aim in learner.Aims)
                    {
                        keys.Add(apprenticeshipKeyService.GenerateApprenticeshipKey(
                            testSession.Ukprn,
                            testSession.GetLearner(learner.LearnerIdentifier).LearnRefNumber,
                            aim.FrameworkCode,
                            aim.PathwayCode,
                            aim.ProgrammeType,
                            aim.StandardCode,
                            aim.AimReference));
                    }
                }    
            }
            else
            {
                keys.AddRange(testSession.Learners.Select(learner =>
                    apprenticeshipKeyService.GenerateApprenticeshipKey(
                        testSession.Ukprn,
                        testSession.GetLearner(learner.LearnerIdentifier).LearnRefNumber,
                        learner.Course.FrameworkCode,
                        learner.Course.PathwayCode,
                        learner.Course.ProgrammeType,
                        learner.Course.StandardCode,
                        learner.Course.LearnAimRef))
                    .ToList());
            }
            
            var startedEvent = new CollectionStartedEvent
            {
                ApprenticeshipKeys = keys,
            };

            await messageSession.Send(startedEvent).ConfigureAwait(false);
        }
    }
}
