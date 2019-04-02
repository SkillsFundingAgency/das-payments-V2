using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService
{
    public class ActorApprenticeshipKeyProvider : IApprenticeshipKeyProvider
    {
        private ApprenticeshipKey apprenticeshipKey;
        private readonly IActorIdProvider actorIdProvider;
        private readonly IApprenticeshipKeyService apprenticeshipKeyService;

        public ActorApprenticeshipKeyProvider(IApprenticeshipKeyService apprenticeshipKeyService, IActorIdProvider actorIdProvider)
        {
            this.apprenticeshipKeyService = apprenticeshipKeyService;
            this.actorIdProvider = actorIdProvider;
        }

        public ApprenticeshipKey GetCurrentKey()
        {
            if (apprenticeshipKey == null)
                apprenticeshipKey = apprenticeshipKeyService.ParseApprenticeshipKey(actorIdProvider.Current.GetStringId());

            return apprenticeshipKey;
        }
    }
}
