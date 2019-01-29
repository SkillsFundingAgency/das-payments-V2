using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.LevyFundedService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class LevyFundedService : Actor, ILevyFundedService
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IDataCache<ApprenticeshipContractType1RequiredPaymentEvent> requiredPaymentsCache;
        private readonly IDataCache<List<string>> requiredPaymentKeys;
        private readonly ITelemetry telemetry;

        public LevyFundedService(
            ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger, 
            IDataCache<ApprenticeshipContractType1RequiredPaymentEvent> requiredPaymentsCache, 
            ITelemetry telemetry, 
            IDataCache<List<string>> requiredPaymentKeys) 
            : base(actorService, actorId)
        {
            this.paymentLogger = paymentLogger;
            this.requiredPaymentsCache = requiredPaymentsCache;
            this.telemetry = telemetry;
            this.requiredPaymentKeys = requiredPaymentKeys;
        }

        public async Task Reset()
        {
            paymentLogger.LogInfo($"Resetting actor for apprenticeship {Id}");
            using (var operation = telemetry.StartOperation())
            {
                var keysValue = await requiredPaymentKeys.TryGet("keys", CancellationToken.None).ConfigureAwait(false);
                if (keysValue.HasValue)
                {
                    foreach (var key in keysValue.Value)
                    {
                        await requiredPaymentsCache.Clear(key, CancellationToken.None).ConfigureAwait(false);
                    }

                    await requiredPaymentKeys.Clear("keys").ConfigureAwait(false);
                }
                telemetry.StopOperation(operation);
            }
        }

        public async Task HandleRequiredPayment(ApprenticeshipContractType1RequiredPaymentEvent message)
        {
            paymentLogger.LogVerbose($"Handling RequiredPayment for {Id}");

            using (var operation = telemetry.StartOperation())
            {
                await requiredPaymentsCache.Add(message.EventId.ToString(), message).ConfigureAwait(false);

                var keysValue = await requiredPaymentKeys.TryGet("keys", CancellationToken.None).ConfigureAwait(false);
                var keys = keysValue.HasValue ? keysValue.Value : new List<string>();
                keys.Add(message.EventId.ToString());
                await requiredPaymentKeys.AddOrReplace("keys", keys, CancellationToken.None).ConfigureAwait(false);

                telemetry.StopOperation(operation);
            }
        }

        public async Task<IReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(ProcessLevyPaymentsOnMonthEndCommand command)
        {
            paymentLogger.LogVerbose($"Handling ProcessLevyPaymentsOnMonthEndCommand for {Id}");

            using (var operation = telemetry.StartOperation())
            {
                var keysValue = await requiredPaymentKeys.TryGet("keys", CancellationToken.None).ConfigureAwait(false);
                if (keysValue.HasValue)
                {
                    var result = new List<ApprenticeshipContractType1RequiredPaymentEvent>();

                    foreach (var key in keysValue.Value)
                    {
                        var conditionalValue = await requiredPaymentsCache.TryGet(key, CancellationToken.None);
                        if (!conditionalValue.HasValue)
                            throw new InvalidOperationException("Corrupt key stored: " + key);

                        result.Add(conditionalValue.Value);
                    }

                    // TODO: process
                }
                telemetry.StopOperation(operation);
            }
        }
    }
}
