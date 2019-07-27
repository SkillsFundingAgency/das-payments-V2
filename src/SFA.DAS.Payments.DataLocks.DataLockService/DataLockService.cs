using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.DataLockService.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Infrastructure;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.DataLockService
{
    [StatePersistence(StatePersistence.Volatile)]
    public class DataLockService : Actor, IDataLockService
    {
        private readonly ActorService actorService;
        private readonly ActorId actorId;
        private readonly IPaymentLogger paymentLogger;
        private readonly IActorDataCache<List<ApprenticeshipModel>> apprenticeships;
        private readonly IDataLockProcessor dataLockProcessor;
        private readonly IApprenticeshipUpdatedProcessor apprenticeshipUpdatedProcessor;
        private readonly ITelemetry telemetry;
        private readonly IApprenticeshipRepository apprenticeshipRepository;

        public DataLockService(
            ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger,
            IApprenticeshipRepository apprenticeshipRepository,
            IActorDataCache<List<ApprenticeshipModel>> apprenticeships,
            IDataLockProcessor dataLockProcessor,
            IApprenticeshipUpdatedProcessor apprenticeshipUpdatedProcessor,
            ITelemetry telemetry
            )
            : base(actorService, actorId)
        {
            this.actorService = actorService;
            this.actorId = actorId;
            this.paymentLogger = paymentLogger;
            this.apprenticeshipRepository = apprenticeshipRepository;
            this.apprenticeships = apprenticeships;
            this.dataLockProcessor = dataLockProcessor;
            this.apprenticeshipUpdatedProcessor = apprenticeshipUpdatedProcessor ?? throw new ArgumentNullException(nameof(apprenticeshipUpdatedProcessor));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task<List<DataLockEvent>> HandleEarning(ApprenticeshipContractType1EarningEvent message, CancellationToken cancellationToken)
        {
            using (var operation = telemetry.StartOperation("DataLockService.HandleEarning", message.EventId.ToString()))
            {
                var stopwatch = Stopwatch.StartNew();
                await Initialise().ConfigureAwait(false);
                var dataLockEvents = await dataLockProcessor.GetPaymentEvents(message, cancellationToken);
                telemetry.TrackDuration("DataLockService.HandleEarning", stopwatch, message);
                telemetry.StopOperation(operation);
                return dataLockEvents;
            }
        }

        public async Task HandleApprenticeshipUpdated(ApprenticeshipUpdated message, CancellationToken none)
        {
            using (var operation = telemetry.StartOperation("DataLockService.HandleApprenticeshipUpdated", message.EventId.ToString()))
            {
                var stopwatch = Stopwatch.StartNew();
                await Initialise().ConfigureAwait(false);
                await apprenticeshipUpdatedProcessor.ProcessApprenticeshipUpdate(message);
                TrackInfrastructureEvent("DataLockService.HandleApprenticeshipUpdated", stopwatch);
                telemetry.StopOperation(operation);
            }
        }

        public async Task Reset()
        {
            paymentLogger.LogDebug($"Resetting actor for provider {Id}");
            await apprenticeships.ResetInitialiseFlag().ConfigureAwait(false);
            paymentLogger.LogInfo($"Reset actor for provider {Id}");
        }

        protected override async Task OnActivateAsync()
        {
            using (var operation = telemetry.StartOperation("DataLockService.OnActivateAsync", $"{Id}_{Guid.NewGuid():N}"))
            {
                var stopwatch = Stopwatch.StartNew();
                await Initialise().ConfigureAwait(false);
                await base.OnActivateAsync().ConfigureAwait(false);
                TrackInfrastructureEvent("DataLockService.HandleEarning", stopwatch);
                telemetry.StopOperation(operation);
            }
        }

        private async Task Initialise()
        {
            if (await apprenticeships.IsInitialiseFlagIsSet().ConfigureAwait(false))
            {
                paymentLogger.LogVerbose($"Actor already initialised for apprenticeship {Id}");
                return;
            }
            var stopwatch = Stopwatch.StartNew();

            paymentLogger.LogInfo($"Initialising actor for provider {Id}");

            var providerApprenticeships = await apprenticeshipRepository.ApprenticeshipsForProvider(long.Parse(Id.ToString())).ConfigureAwait(false);

            if (providerApprenticeships.Any())
            {
                var groupedApprenticeships = providerApprenticeships.ToLookup(x => x.Uln);

                foreach (var group in groupedApprenticeships)
                {
                    await this.apprenticeships.AddOrReplace(group.Key.ToString(), group.ToList()).ConfigureAwait(false);
                }

                var providerDuplicateApprenticeships = await apprenticeshipRepository
                    .DuplicateApprenticeshipsForProvider(long.Parse(Id.ToString()))
                    .ConfigureAwait(false);

                await this.apprenticeships.AddOrReplace(CacheKeys.DuplicateApprenticeshipsKey, providerDuplicateApprenticeships).ConfigureAwait(false);
            }
            await apprenticeships.SetInitialiseFlag().ConfigureAwait(false);
            paymentLogger.LogInfo($"Initialised actor for provider {Id}");
            stopwatch.Stop();
            TrackInfrastructureEvent("DataLockService.Initialise", stopwatch);
        }


        private void TrackInfrastructureEvent(string eventName, Stopwatch stopwatch)
        {
            telemetry.TrackEvent(eventName,
                new Dictionary<string, string>
                {
                    { "ActorId",Id.ToString()},
                    { TelemetryKeys.Ukprn, Id.ToString()},
                },
                new Dictionary<string, double>
                {
                    { TelemetryKeys.Duration, stopwatch.ElapsedMilliseconds }
                });
        }
    }
}
