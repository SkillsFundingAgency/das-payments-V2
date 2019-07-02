using System.Collections.Generic;
using NServiceBus;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

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

        public async Task ClearCaches(Provider provider, TestSession testSession, short academicYear)
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
                        var priceEpisodes = aim.PriceEpisodes;
                        var contractTypes = EnumHelper.GetContractTypes(null, priceEpisodes);

                        contractTypes.ForEach(c =>
                        {

                            keys.Add(apprenticeshipKeyService.GenerateApprenticeshipKey(
                                ukprn,
                                testSession.GetLearner(ukprn, learner.LearnerIdentifier).LearnRefNumber,
                                aim.FrameworkCode,
                                aim.PathwayCode,
                                aim.ProgrammeType,
                                aim.StandardCode,
                                aim.AimReference,
                                academicYear, c));
                        });
                    }
                }
            }
            else
            {
                foreach (var providerLearner in providerLearners)
                {
                    var learner = testSession.GetLearner(ukprn, providerLearner.LearnerIdentifier);

                    var aims = learner.Aims;
                    var priceEpisodes = aims.SelectMany(x => x.PriceEpisodes).ToList();

                    var contractTypes = EnumHelper.GetContractTypes(null, priceEpisodes);

                    contractTypes.ForEach(c =>
                    {
                        keys.Add(apprenticeshipKeyService.GenerateApprenticeshipKey(
                            ukprn,
                            learner.LearnRefNumber,
                            learner.Course.FrameworkCode,
                            learner.Course.PathwayCode,
                            learner.Course.ProgrammeType,
                            learner.Course.StandardCode,
                            learner.Course.LearnAimRef,
                            academicYear, c));
                    });
                }
            }

            var startedEvent = new CollectionStartedEvent
            {
                ApprenticeshipKeys = keys,
                JobId = provider.JobId > 0 ? provider.JobId : testSession.JobId
            };

            await messageSession.Request<int>(startedEvent).ConfigureAwait(false);
        }
    }
}
