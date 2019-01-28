using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.LevyFundedService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class LevyFundedService : Actor, ILevyFundedService
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IDataCache<ApprenticeshipContractType1RequiredPaymentEvent> requiredPaymentsCache;
        private readonly ITelemetry telemetry;

        public LevyFundedService(
            ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger, 
            IDataCache<ApprenticeshipContractType1RequiredPaymentEvent> requiredPaymentsCache, ITelemetry telemetry) 
            : base(actorService, actorId)
        {
            this.paymentLogger = paymentLogger;
            this.requiredPaymentsCache = requiredPaymentsCache;
            this.telemetry = telemetry;
        }

        public async Task Reset()
        {
            paymentLogger.LogInfo($"Resetting actor for apprenticeship {Id}");
            using (var operation = telemetry.StartOperation())
            {
                await StateManager.ClearCacheAsync().ConfigureAwait(false);
                telemetry.StopOperation(operation);
            }
        }

        public async Task HandleRequiredPayment(ApprenticeshipContractType1RequiredPaymentEvent message)
        {
            paymentLogger.LogVerbose($"Handling RequiredPayment for {Id}");

            using (var operation = telemetry.StartOperation())
            {
                await requiredPaymentsCache.Add(message.EventId.ToString(), message).ConfigureAwait(false);
                telemetry.StopOperation(operation);
            }
        }
    }
}
