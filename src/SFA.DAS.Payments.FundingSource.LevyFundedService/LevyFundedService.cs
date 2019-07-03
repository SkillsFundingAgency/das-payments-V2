
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;
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
        private IDataCache<CalculatedRequiredLevyAmount> requiredPaymentsCache;
        private IDataCache<List<string>> requiredPaymentKeys;
        private IDataCache<bool> monthEndCache;
        private IDataCache<LevyAccountModel> levyAccountCache;
        private readonly ILifetimeScope lifetimeScope;

        public LevyFundedService(
            ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger,
            ITelemetry telemetry,
            ILifetimeScope lifetimeScope)
            : base(actorService, actorId)
        {
            this.paymentLogger = paymentLogger;
            this.telemetry = telemetry;
            this.lifetimeScope = lifetimeScope;
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

        protected override async Task OnActivateAsync()
        {
            //TODO: Use DI
            requiredPaymentsCache = new ReliableCollectionCache<CalculatedRequiredLevyAmount>(StateManager);
            requiredPaymentKeys = new ReliableCollectionCache<List<string>>(StateManager);
            monthEndCache = new ReliableCollectionCache<bool>(StateManager);
            levyAccountCache = new ReliableCollectionCache<LevyAccountModel>(StateManager);
            fundingSourceService = new RequiredLevyAmountFundingSourceService(
                lifetimeScope.Resolve<IPaymentProcessor>(),
                lifetimeScope.Resolve<IMapper>(),
                requiredPaymentsCache,
                requiredPaymentKeys,
                lifetimeScope.Resolve<ILevyAccountRepository>(),
                lifetimeScope.Resolve<ILevyBalanceService>(),
                lifetimeScope.Resolve<IPaymentLogger>(),
                lifetimeScope.Resolve<ISortableKeyGenerator>(),
                monthEndCache,
                levyAccountCache
            );
            await base.OnActivateAsync().ConfigureAwait(false);
        }

    }
}
