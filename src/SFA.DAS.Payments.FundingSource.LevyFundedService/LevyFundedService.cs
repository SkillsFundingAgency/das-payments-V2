using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Autofac;
using AutoMapper;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;
using SFA.DAS.Payments.FundingSource.Model;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;

namespace SFA.DAS.Payments.FundingSource.LevyFundedService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class LevyFundedService : Actor, ILevyFundedService
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly ITelemetry telemetry;
        private IRequiredLevyAmountFundingSourceService fundingSourceService;
        private IGenerateSortedPaymentKeys generateSortedPaymentKeys;

        private IDataCache<CalculatedRequiredLevyAmount> requiredPaymentsCache;
        private IDataCache<bool> monthEndCache;
        private IDataCache<LevyAccountModel> levyAccountCache;
        private IDataCache<List<EmployerProviderPriorityModel>> employerProviderPriorities;
        private IDataCache<List<string>> refundSortKeysCache;
        private IDataCache<List<TransferPaymentSortKeyModel>> transferPaymentSortKeysCache;
        private IDataCache<List<RequiredPaymentSortKeyModel>> requiredPaymentSortKeysCache;

        private IActorDataCache<bool> actorCache;
        private readonly ILifetimeScope lifetimeScope;
        private readonly ILevyFundingSourceRepository levyFundingSourceRepository;

        public LevyFundedService(
            ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger,
            ITelemetry telemetry,
            ILifetimeScope lifetimeScope,
            ILevyFundingSourceRepository levyFundingSourceRepository)
            : base(actorService, actorId)
        {
            this.paymentLogger = paymentLogger;
            this.telemetry = telemetry;
            this.lifetimeScope = lifetimeScope;
            this.levyFundingSourceRepository = levyFundingSourceRepository;
        }

        public async Task HandleRequiredPayment(CalculatedRequiredLevyAmount message)
        {
            try
            {
                using (var operation = telemetry.StartOperation("LevyFundedService.HandleRequiredPayment", message.EventId.ToString()))
                {
                    var stopwatch = Stopwatch.StartNew();
                    paymentLogger.LogVerbose($"Handling RequiredPayment for {Id}, Job: {message.JobId}, UKPRN: {message.Ukprn}, Account: {message.AccountId}");
                    await fundingSourceService.AddRequiredPayment(message).ConfigureAwait(false);
                    paymentLogger.LogInfo($"Finished handling required payment for {Id}, Job: {message.JobId}, UKPRN: {message.Ukprn}, Account: {message.AccountId}");
                    telemetry.TrackDuration("LevyFundedService.HandleRequiredPayment", stopwatch, message);
                    telemetry.StopOperation(operation);
                }
            }
            catch (Exception e)
            {
                paymentLogger.LogError($"Error handling required levy payment. Error:{e.Message}", e);
                throw;
            }
        }

        public async Task HandleEmployerProviderPriorityChange(EmployerChangedProviderPriority message)
        {
            try
            {
                using (var operation = telemetry.StartOperation("LevyFundedService.HandleEmployerProviderPriorityChange", message.EventId.ToString()))
                {
                    var stopwatch = Stopwatch.StartNew();
                    paymentLogger.LogDebug($"Storing EmployerChangedProviderPriority event for {Id},  Account Id: {message.EmployerAccountId}");
                    await fundingSourceService.StoreEmployerProviderPriority(message).ConfigureAwait(false);
                    paymentLogger.LogInfo($"Finished Storing EmployerChangedProviderPriority event for {Id},  Account Id: {message.EmployerAccountId}");
                    TrackInfrastructureEvent("LevyFundedService.HandleEmployerProviderPriorityChange", stopwatch);
                    telemetry.StopOperation(operation);
                }
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while handling EmployerChangedProviderPriority event for {Id},  Account Id: {message.EmployerAccountId} Error:{ex.Message}", ex);
                throw;
            }
        }

        public async Task<ReadOnlyCollection<FundingSourcePaymentEvent>> UnableToFundTransfer(ProcessUnableToFundTransferFundingSourcePayment message)
        {
            try
            {
                using (var operation = telemetry.StartOperation("LevyFundedService.UnableToFundTransfer", message.EventId.ToString()))
                {
                    var stopwatch = Stopwatch.StartNew();
                    paymentLogger.LogDebug($"Handling UnableToFundTransfer for {Id}, Job: {message.JobId}, UKPRN: {message.Ukprn}, Receiver Account: {message.AccountId}, Sender Account: {message.TransferSenderAccountId}");
                    var fundingSourcePayments = await fundingSourceService.ProcessReceiverTransferPayment(message).ConfigureAwait(false);
                    paymentLogger.LogInfo($"Finished handling required payment for {Id}, Job: {message.JobId}, UKPRN: {message.Ukprn}, Account: {message.AccountId}");
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

        public async Task<ReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(ProcessLevyPaymentsOnMonthEndCommand command)
        {
            paymentLogger.LogVerbose($"Handling ProcessLevyPaymentsOnMonthEndCommand for {Id}, Job: {command.JobId}, Account: {command.AccountId}");
            try
            {
                using (var operation = telemetry.StartOperation())
                {
                    var stopwatch = Stopwatch.StartNew();
                    var fundingSourceEvents = await fundingSourceService.HandleMonthEnd(command.AccountId, command.JobId);
                    telemetry.StopOperation(operation);
                    return fundingSourceEvents;
                }
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Failed to get levy or co-invested month end payments. Error: {ex.Message}", ex);
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
                    await fundingSourceService.RemovePreviousSubmissions(message.JobId, message.CollectionPeriod, message.AcademicYear, message.IlrSubmissionDateTime, message.Ukprn);
                    telemetry.StopOperation(operation);
                }
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Failed to remove previous submission required payments. Error: {ex.Message}", ex);
                throw;
            }
        }
        public async Task RemoveCurrentSubmission(SubmissionJobFailed message)
        {
            paymentLogger.LogVerbose($"Handling ProcessCurrentSubmissionDeletionCommand for {Id}, Job: {message.JobId}");
            try
            {
                using (var operation = telemetry.StartOperation())
                {
                    await fundingSourceService.RemoveCurrentSubmission(message.JobId, message.CollectionPeriod, message.AcademicYear, message.IlrSubmissionDateTime, message.Ukprn);
                    telemetry.StopOperation(operation);
                }
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Failed to remove current submission required payments. Error: {ex.Message}", ex);
                throw;
            }
        }

        protected override async Task OnActivateAsync()
        {
            using (var operation = telemetry.StartOperation("LevyFundedService.OnActivateAsync", $"{Id}_{Guid.NewGuid():N}"))
            {
                var stopwatch = Stopwatch.StartNew();
                //TODO: Use DI
                actorCache = new ActorReliableCollectionCache<bool>(StateManager);
                employerProviderPriorities = new ReliableCollectionCache<List<EmployerProviderPriorityModel>>(StateManager);
                requiredPaymentsCache = new ReliableCollectionCache<CalculatedRequiredLevyAmount>(StateManager);
                monthEndCache = new ReliableCollectionCache<bool>(StateManager);
                levyAccountCache = new ReliableCollectionCache<LevyAccountModel>(StateManager);
                refundSortKeysCache = new ReliableCollectionCache<List<string>>(StateManager);
                transferPaymentSortKeysCache = new ReliableCollectionCache<List<TransferPaymentSortKeyModel>>(StateManager);
                requiredPaymentSortKeysCache = new ReliableCollectionCache<List<RequiredPaymentSortKeyModel>>(StateManager);

                generateSortedPaymentKeys = new GenerateSortedPaymentKeys(
                    employerProviderPriorities,
                    refundSortKeysCache,
                    transferPaymentSortKeysCache,
                    requiredPaymentSortKeysCache
                );

                fundingSourceService = new RequiredLevyAmountFundingSourceService(
                    lifetimeScope.Resolve<IPaymentProcessor>(),
                    lifetimeScope.Resolve<IMapper>(),
                    requiredPaymentsCache,
                    lifetimeScope.Resolve<ILevyFundingSourceRepository>(),
                    lifetimeScope.Resolve<ILevyBalanceService>(),
                    lifetimeScope.Resolve<IPaymentLogger>(),
                    monthEndCache,
                    levyAccountCache,
                    employerProviderPriorities,
                    refundSortKeysCache,
                    transferPaymentSortKeysCache,
                    requiredPaymentSortKeysCache,
                    generateSortedPaymentKeys
                );

                await Initialise().ConfigureAwait(false);
                await base.OnActivateAsync().ConfigureAwait(false);
                TrackInfrastructureEvent("LevyFundedService.OnActivateAsync", stopwatch);
                telemetry.StopOperation(operation);
            }
        }

        private async Task Initialise()
        {
            if (await actorCache.IsInitialiseFlagIsSet().ConfigureAwait(false))
            {
                paymentLogger.LogVerbose($"Actor already initialised for employer {Id}");
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            paymentLogger.LogInfo($"Initialising actor for employer {Id.GetLongId()}");

            var paymentPriorities = await levyFundingSourceRepository.GetPaymentPriorities(Id.GetLongId()).ConfigureAwait(false);
            await employerProviderPriorities
                    .AddOrReplace(CacheKeys.EmployerPaymentPriorities, paymentPriorities, default(CancellationToken))
                    .ConfigureAwait(false);

            await actorCache.SetInitialiseFlag().ConfigureAwait(false);
            paymentLogger.LogInfo($"Initialised actor for employer {Id.GetLongId()}");
            TrackInfrastructureEvent("LevyFundedService.Initialise", stopwatch);
        }

        public async Task Reset()
        {
            paymentLogger.LogInfo($"Resetting actor for employer {Id.GetLongId()}");
            await actorCache.ResetInitialiseFlag().ConfigureAwait(false);
        }


        private void TrackInfrastructureEvent(string eventName, Stopwatch stopwatch)
        {
            stopwatch.Stop();
            telemetry.TrackEvent(eventName,
                new Dictionary<string, string>
                {
                    { "ActorId",Id.ToString()},
                    { "Employer", Id.ToString()},
                },
                new Dictionary<string, double>
                {
                    { TelemetryKeys.Duration, stopwatch.ElapsedMilliseconds }
                });
        }

    }
}
