using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class RequiredPaymentsCacheCleaner
    {
        private readonly IApprenticeshipKeyService apprenticeshipKeyService;
        private readonly IActorProxyFactory proxyFactory = new ActorProxyFactory();
        private const string Url = "fabric:/SFA.DAS.Payments.RequiredPayments.ServiceFabric/RequiredPaymentsServiceActorService";

        public RequiredPaymentsCacheCleaner(IApprenticeshipKeyService apprenticeshipKeyService)
        {
            this.apprenticeshipKeyService = apprenticeshipKeyService;
        }

        public async Task ClearCaches(TestSession testSession)
        {
            var tasks = new List<Task>();

            foreach (var learner in testSession.Learners)
            {
                var key = apprenticeshipKeyService.GenerateApprenticeshipKey(
                    testSession.Ukprn,
                    testSession.GetLearner(learner.LearnerIdentifier).LearnRefNumber,
                    learner.Course.FrameworkCode,
                    learner.Course.PathwayCode,
                    learner.Course.ProgrammeType,
                    learner.Course.StandardCode,
                    learner.Course.LearnAimRef
                );

                var actorId = new ActorId(key);
                var actor = proxyFactory.CreateActorProxy<IRequiredPaymentsService>(new Uri(Url), actorId);
                tasks.Add(actor.Reset());
            }

            await Task.WhenAll(tasks);
        }
    }
}
