using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.LevyFundedService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class LevyFundedService : Actor, ILevyFundedService
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IPaymentKeyService paymentKeyService;
        private readonly ILifetimeScope lifetimeScope;
        private readonly ApprenticeshipKey apprenticeshipKey;
        private const string InitialisedKey = "Initialised";

        public LevyFundedService(ActorService actorService, ActorId actorId,
            IPaymentLogger paymentLogger,
            IApprenticeshipKeyService apprenticeshipKeyService,
            IPaymentKeyService paymentKeyService,
        ILifetimeScope lifetimeScope) : base(actorService, actorId)
        {
            this.paymentLogger = paymentLogger;
            this.paymentKeyService = paymentKeyService ?? throw new ArgumentNullException(nameof(paymentKeyService));
            this.lifetimeScope = lifetimeScope;
            apprenticeshipKey = apprenticeshipKeyService.ParseApprenticeshipKey(actorId.GetStringId());
        }

        public async Task Initialise()
        {
            if (await StateManager.ContainsStateAsync(InitialisedKey).ConfigureAwait(false)) return;

            paymentLogger.LogInfo($"Initialising actor for apprenticeship {apprenticeshipKey}");
        }

        public async Task Reset()
        {
            paymentLogger.LogInfo($"Resetting actor for apprenticeship {apprenticeshipKey}");
            await StateManager.TryRemoveStateAsync(InitialisedKey, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<ReadOnlyCollection<RequiredPaymentEvent>> HandleEarningEvent(EarningEvent message, CancellationToken none)
        {
            paymentLogger.LogVerbose($"Handling EarningEvent for {apprenticeshipKey}");

            await Initialise().ConfigureAwait(false);

            return null;
        }
    }
}
