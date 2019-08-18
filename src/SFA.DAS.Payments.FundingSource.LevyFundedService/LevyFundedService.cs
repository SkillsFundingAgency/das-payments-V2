using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private  IDataCache<List<string>> refundSortKeysCache;
        private  IDataCache<List<TransferPaymentSortKeyModel>> transferPaymentSortKeysCache;
        private  IDataCache<List<RequiredPaymentSortKeyModel>> requiredPaymentSortKeysCache;

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
                using (var operation = telemetry.StartOperation())
                {
                    paymentLogger.LogVerbose($"Handling RequiredPayment for {Id}, Job: {message.JobId}, UKPRN: {message.Ukprn}, Account: {message.AccountId}");
                    await fundingSourceService.AddRequiredPayment(message).ConfigureAwait(false);
                    paymentLogger.LogInfo($"Finished handling required payment for {Id}, Job: {message.JobId}, UKPRN: {message.Ukprn}, Account: {message.AccountId}");
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
                using (var operation = telemetry.StartOperation())
                {
                    paymentLogger.LogDebug($"Storing EmployerChangedProviderPriority event for {Id},  Account Id: {message.EmployerAccountId}");
                    await fundingSourceService.StoreEmployerProviderPriority(message).ConfigureAwait(false);
                    paymentLogger.LogInfo($"Finished Storing EmployerChangedProviderPriority event for {Id},  Account Id: {message.EmployerAccountId}");
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
                using (var operation = telemetry.StartOperation())
                {
                    paymentLogger.LogDebug($"Handling UnableToFundTransfer for {Id}, Job: {message.JobId}, UKPRN: {message.Ukprn}, Receiver Account: {message.AccountId}, Sender Account: {message.TransferSenderAccountId}");
                    var fundingSourcePayments = await fundingSourceService.ProcessReceiverTransferPayment(message).ConfigureAwait(false);
                    paymentLogger.LogInfo($"Finished handling required payment for {Id}, Job: {message.JobId}, UKPRN: {message.Ukprn}, Account: {message.AccountId}");
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

        public async Task RemovePreviousSubmissions(ProcessPreviousSubmissionDeletionCommand command)
        {
            paymentLogger.LogVerbose($"Handling ProcessPreviousSubmissionDeletionCommand for {Id}, Job: {command.JobId}, Account: {command.AccountId}");
            try
            {
                using (var operation = telemetry.StartOperation())
                {
                    await fundingSourceService.RemovePreviousSubmissions(command.AccountId, command.JobId, command.CollectionPeriod, command.SubmissionDate);
                    telemetry.StopOperation(operation);
                }
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Failed to remove previous submission required payments. Error: {ex.Message}", ex);
                throw;
            }
        }

        public async Task RemoveCurrentSubmission(ProcessCurrentSubmissionDeletionCommand command)
        {
            paymentLogger.LogVerbose($"Handling ProcessCurrentSubmissionDeletionCommand for {Id}, Job: {command.JobId}, Account: {command.AccountId}");
            try
            {
                using (var operation = telemetry.StartOperation())
                {
                    await fundingSourceService.RemovePreviousSubmissions(command.AccountId, command.JobId, command.CollectionPeriod, command.SubmissionDate);
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
        }

        private async Task Initialise()
        {
            if (await actorCache.IsInitialiseFlagIsSet().ConfigureAwait(false)) return;

            paymentLogger.LogInfo($"Initialising actor for employer {Id.GetLongId()}");

            var paymentPriorities = await levyFundingSourceRepository.GetPaymentPriorities(Id.GetLongId()).ConfigureAwait(false);
            await employerProviderPriorities
                    .AddOrReplace(CacheKeys.EmployerPaymentPriorities, paymentPriorities, default(CancellationToken))
                    .ConfigureAwait(false);
        
            paymentLogger.LogInfo($"Initialised actor for employer {Id.GetLongId()}");
            await actorCache.SetInitialiseFlag().ConfigureAwait(false);
        }

        public async Task Reset()
        {
            paymentLogger.LogInfo($"Resetting actor for employer {Id.GetLongId()}");
            await actorCache.ResetInitialiseFlag().ConfigureAwait(false);
        }

    }
}
