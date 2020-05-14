using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;

namespace SFA.DAS.Payments.FundingSource.LevyFundedService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class LevyFundedService : Actor, ILevyFundedService
    {
        private IActorDataCache<bool> actorCache;
        private IFundingSourceEventGenerationService fundingSourceEventGenerationService;
        private ITransferFundingSourceEventGenerationService transferFundingSourceEventGenerationService;

        private IDataCache<bool> monthEndCache;
        private IDataCache<LevyAccountModel> levyAccountCache;

        private readonly IPaymentLogger paymentLogger;
        private readonly ITelemetry telemetry;
        private readonly ILifetimeScope lifetimeScope;
        private readonly ISubmissionCleanUpService submissionCleanUpService;
        private readonly IEmployerProviderPriorityStorageService employerProviderPriorityStorageService;


        public LevyFundedService(
            ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger,
            ITelemetry telemetry,
            ILifetimeScope lifetimeScope,
            ISubmissionCleanUpService submissionCleanUpService,
            IEmployerProviderPriorityStorageService employerProviderPriorityStorageService)
            : base(actorService, actorId)
        {
            this.paymentLogger = paymentLogger;
            this.telemetry = telemetry;
            this.lifetimeScope = lifetimeScope;
            this.submissionCleanUpService = submissionCleanUpService;
            this.employerProviderPriorityStorageService = employerProviderPriorityStorageService;
        }

        public async Task HandleEmployerProviderPriorityChange(EmployerChangedProviderPriority message)
        {
            try
            {
                using (var operation =
                    telemetry.StartOperation("LevyFundedService.HandleEmployerProviderPriorityChange",
                        message.EventId.ToString()))
                {
                    var stopwatch = Stopwatch.StartNew();
                    paymentLogger.LogDebug(
                        $"Storing EmployerChangedProviderPriority event for {Id},  Account Id: {message.EmployerAccountId}");
                    await employerProviderPriorityStorageService.StoreEmployerProviderPriority(message)
                        .ConfigureAwait(false);
                    paymentLogger.LogInfo(
                        $"Finished Storing EmployerChangedProviderPriority event for {Id},  Account Id: {message.EmployerAccountId}");
                    TrackInfrastructureEvent("LevyFundedService.HandleEmployerProviderPriorityChange", stopwatch);
                    telemetry.StopOperation(operation);
                }
            }
            catch (Exception ex)
            {
                paymentLogger.LogError(
                    $"Error while handling EmployerChangedProviderPriority event for {Id},  Account Id: {message.EmployerAccountId} Error:{ex.Message}",
                    ex);
                throw;
            }
        }

        public async Task<ReadOnlyCollection<FundingSourcePaymentEvent>> UnableToFundTransfer(
            ProcessUnableToFundTransferFundingSourcePayment message)
        {
            try
            {
                using (var operation = telemetry.StartOperation("LevyFundedService.UnableToFundTransfer",
                    message.EventId.ToString()))
                {
                    var stopwatch = Stopwatch.StartNew();
                    paymentLogger.LogDebug(
                        $"Handling UnableToFundTransfer for {Id}, Job: {message.JobId}, UKPRN: {message.Ukprn}, Receiver Account: {message.AccountId}, Sender Account: {message.TransferSenderAccountId}");
                    var fundingSourcePayments = await transferFundingSourceEventGenerationService
                        .ProcessReceiverTransferPayment(message).ConfigureAwait(false);
                    
                   
                    paymentLogger.LogInfo(
                        $"Finished handling required payment for {Id}, Job: {message.JobId}, UKPRN: {message.Ukprn}, Account: {message.AccountId}");
                    telemetry.TrackDuration("LevyFundedService.UnableToFundTransfer", stopwatch, message);
                    telemetry.StopOperation(operation);
                    return fundingSourcePayments;
                }
            }
            catch (Exception e)
            {
                paymentLogger.LogError($"Error handling unable to fund transfer. Error: {e.Message}", e);
                throw;
            }
        }

        public async Task<ReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(
            ProcessLevyPaymentsOnMonthEndCommand command)
        {
            paymentLogger.LogVerbose(
                $"Handling ProcessLevyPaymentsOnMonthEndCommand for {Id}, Job: {command.JobId}, Account: {command.AccountId}");
            try
            {
                using (var operation =
                    telemetry.StartOperation("LevyFundedService.HandleMonthEnd", command.CommandId.ToString()))
                {
                    var stopwatch = Stopwatch.StartNew();


                    var fundingSourceEvents =
                        await fundingSourceEventGenerationService.HandleMonthEnd(command.AccountId, command.JobId, command.CollectionPeriod);

                    await monthEndCache.AddOrReplace(CacheKeys.MonthEndCacheKey, true, CancellationToken.None);

                    telemetry.TrackDurationWithMetrics("LevyFundedService.HandleMonthEnd",
                        stopwatch,
                        command,
                        command.AccountId,
                        new Dictionary<string, double>
                        {
                            {TelemetryKeys.Count, fundingSourceEvents.Count}
                        });

                    telemetry.StopOperation(operation);
                    return fundingSourceEvents;
                }
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Failed to get levy or co-invested month end payments. Error: {ex.Message}",
                    ex);
                throw;
            }
        }

        public async Task RemovePreviousSubmissions(SubmissionJobSucceeded message)
        {
            paymentLogger.LogVerbose($"Handling ProcessSubmissionDeletion for {Id}, Job: {message.JobId}");
            try
            {
                using (var operation = telemetry.StartOperation())
                {
                    await submissionCleanUpService.RemovePreviousSubmissions(message.JobId, message.CollectionPeriod,
                        message.AcademicYear, message.IlrSubmissionDateTime, message.Ukprn);
                    telemetry.StopOperation(operation);
                }
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Failed to remove previous submission required payments. Error: {ex.Message}",
                    ex);
                throw;
            }
        }

        public async Task RemoveCurrentSubmission(SubmissionJobFailed message)
        {
            paymentLogger.LogVerbose(
                $"Handling ProcessCurrentSubmissionDeletionCommand for {Id}, Job: {message.JobId}");
            try
            {
                using (var operation = telemetry.StartOperation())
                {
                    await submissionCleanUpService.RemoveCurrentSubmission(message.JobId, message.CollectionPeriod,
                        message.AcademicYear, message.Ukprn);
                    telemetry.StopOperation(operation);
                }
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Failed to remove current submission required payments. Error: {ex.Message}",
                    ex);
                throw;
            }
        }

        protected override async Task OnActivateAsync()
        {
            using (var operation =
                telemetry.StartOperation("LevyFundedService.OnActivateAsync", $"{Id}_{Guid.NewGuid():N}"))
            {
                var stopwatch = Stopwatch.StartNew();
                //TODO: Use DI
                actorCache = new ActorReliableCollectionCache<bool>(StateManager);
               
                monthEndCache = new ReliableCollectionCache<bool>(StateManager);
                levyAccountCache = new ReliableCollectionCache<LevyAccountModel>(StateManager);

                var logger = lifetimeScope.Resolve<IPaymentLogger>();
                var levyBalanceService = lifetimeScope.Resolve<ILevyBalanceService>();

                fundingSourceEventGenerationService = new FundingSourceEventGenerationService(
                    logger,
                    lifetimeScope.Resolve<IFundingSourceDataContext>(),
                    levyBalanceService,
                    lifetimeScope.Resolve<ILevyFundingSourceRepository>(),
                    levyAccountCache,
                    lifetimeScope.Resolve<ICalculatedRequiredLevyAmountPrioritisationService>(),
                    lifetimeScope.Resolve<IFundingSourcePaymentEventBuilder>()
                );
                transferFundingSourceEventGenerationService = new TransferFundingSourceEventGenerationService(
                    logger,
                    lifetimeScope.Resolve<IMapper>(),
                    monthEndCache,
                    levyAccountCache,
                    levyBalanceService,
                    lifetimeScope.Resolve<IFundingSourcePaymentEventBuilder>(),
                    lifetimeScope.Resolve<ILevyTransactionBatchStorageService>()
                );

                await Initialise().ConfigureAwait(false);
                await base.OnActivateAsync().ConfigureAwait(false);
                TrackInfrastructureEvent("LevyFundedService.OnActivateAsync", stopwatch);
                telemetry.StopOperation(operation);
            }
        }

        private async Task Initialise()
        {
            if (!long.TryParse(Id.ToString(), out _))
                throw new InvalidCastException($"Unable to cast Actor Id {Id} to valid account Id ");

            if (await actorCache.IsInitialiseFlagIsSet().ConfigureAwait(false))
            {
                paymentLogger.LogVerbose($"Actor already initialised for employer {Id}");
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            paymentLogger.LogInfo($"Initialising actor for employer {Id}");

            await actorCache.SetInitialiseFlag().ConfigureAwait(false);
            paymentLogger.LogInfo($"Initialised actor for employer {Id}");
            TrackInfrastructureEvent("LevyFundedService.Initialise", stopwatch);
        }

        public async Task Reset()
        {
            paymentLogger.LogInfo($"Resetting actor for employer {Id}");
            await actorCache.ResetInitialiseFlag().ConfigureAwait(false);
        }

        private void TrackInfrastructureEvent(string eventName, Stopwatch stopwatch)
        {
            stopwatch.Stop();
            telemetry.TrackEvent(eventName,
                new Dictionary<string, string>
                {
                    {"ActorId", Id.ToString()},
                    {"Employer", Id.ToString()}
                },
                new Dictionary<string, double>
                {
                    {TelemetryKeys.Duration, stopwatch.ElapsedMilliseconds}
                });
        }
    }
}