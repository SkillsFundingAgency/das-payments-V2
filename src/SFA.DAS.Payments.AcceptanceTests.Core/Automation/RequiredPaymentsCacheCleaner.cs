using System.Collections.Generic;
using NServiceBus;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    using Infrastructure;

    public class RequiredPaymentsCacheCleaner
    {
        private readonly IApprenticeshipKeyService apprenticeshipKeyService;
        private readonly IMessageSession messageSession;
        private readonly TestsConfiguration _configuration;

        public RequiredPaymentsCacheCleaner(IApprenticeshipKeyService apprenticeshipKeyService, IMessageSession messageSession, TestsConfiguration configuration)
        {
            this.apprenticeshipKeyService = apprenticeshipKeyService;
            this.messageSession = messageSession;
            _configuration = configuration;
        }

        public async Task ClearCaches(Provider provider, TestSession testSession)
        {
            var ukprn = provider.Ukprn;
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
                            testSession.GetLearner(ukprn, learner.LearnerIdentifier).LearnRefNumber,
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
                        testSession.GetLearner(ukprn, learner.LearnerIdentifier).LearnRefNumber,
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
                JobId = provider.JobId > 0 ? provider.JobId : testSession.JobId
            };

            //if (_configuration.ValidateDcAndDasServices)
            //{
            //    await messageSession.Send(startedEvent).ConfigureAwait(false);
            //}
            //else
            //{
                await messageSession.Request<int>(startedEvent).ConfigureAwait(false);
            //}
        }
    }
}
