using System;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Enums;

namespace SFA.DAS.Payments.AcceptanceTests.ServiceFabric.Core
{
    public class ActorHelper
    {
        private readonly IApprenticeshipKeyService apprenticeshipKeyService;
        private readonly IActorProxyFactory proxyFactory;
        private readonly string url;

        public ActorHelper(IApprenticeshipKeyService apprenticeshipKeyService, IActorProxyFactory proxyFactory, string url)
        {
            this.apprenticeshipKeyService = apprenticeshipKeyService;
            this.proxyFactory = proxyFactory;
            this.url = url;
            //url = "fabric:/SFA.DAS.Payments.RequiredPayments.ServiceFabric/RequiredPaymentsServiceActorService";
        }

        public void ClearCaches()
        {
            foreach (var learner in testSession.Learners)
            {
                var key = apprenticeshipKeyService.GenerateApprenticeshipKey(
                    testSession.Ukprn,
                    testSession.GetLearner(learner.LearnerIdentifier).LearnerIdentifier,
                    learner.Course.FrameworkCode,
                    learner.Course.PathwayCode,
                    (ProgrammeType)learner.Course.ProgrammeType,
                    learner.Course.StandardCode,
                    learner.Course.LearnAimRef
                );

                var actorId = new ActorId(key);
                var actor = proxyFactory.CreateActorProxy<IRequiredPaymentsService>(new Uri(url), actorId);

            }
        }

        //public T GetActor(string key)
        //{
        //        var actorId = new ActorId(key);
        //        return proxyFactory.CreateActorProxy<T>(new Uri(url), actorId);

        //    foreach (var learner in testSession.Learners)
        //    {
        //        //var key = apprenticeshipKeyService.GenerateApprenticeshipKey(
        //        //    testSession.Ukprn,
        //        //    testSession.GetLearner(learner.LearnerIdentifier).LearnerIdentifier,
        //        //    learner.Course.FrameworkCode,
        //        //    learner.Course.PathwayCode,
        //        //    (ProgrammeType)learner.Course.ProgrammeType,
        //        //    learner.Course.StandardCode,
        //        //    learner.Course.LearnAimRef
        //        //);

        //        //var actorId = new ActorId(key);
        //        //var actor = proxyFactory.CreateActorProxy<T>(new Uri(url), actorId);

        //        //actors
        //    }
        //}
    }
}
