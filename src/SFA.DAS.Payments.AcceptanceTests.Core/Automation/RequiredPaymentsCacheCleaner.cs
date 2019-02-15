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

        public async Task ClearCaches(long ukprn,TestSession testSession)
        {
            var keys = new List<string>();
            var providerLearners = testSession.Learners.Where(x => x.Ukprn == ukprn).ToList();

            if (providerLearners.Any(x => x.Aims.Count > 0))
            {
                foreach (var learner in providerLearners)
                {
                    foreach (var aim in learner.Aims)
                    {
                        keys.Add(apprenticeshipKeyService.GenerateApprenticeshipKey(
                            ukprn,
                            testSession.GetLearner(ukprn,learner.LearnerIdentifier).LearnRefNumber,
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
                keys.AddRange(providerLearners.Select(learner =>
                    apprenticeshipKeyService.GenerateApprenticeshipKey(
                        ukprn,
                        testSession.GetLearner(ukprn,learner.LearnerIdentifier).LearnRefNumber,
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
