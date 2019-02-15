
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
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;

namespace SFA.DAS.Payments.FundingSource.LevyFundedService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class LevyFundedService : Actor, ILevyFundedService
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly ITelemetry telemetry;
        private IContractType1RequiredPaymentEventFundingSourceService fundingSourceService;
        private IDataCache<ApprenticeshipContractType1RequiredPaymentEvent> requiredPaymentsCache;
        private IDataCache<List<string>> requiredPaymentKeys;
        private readonly ILifetimeScope lifetimeScope;

        public LevyFundedService(
            ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger,
            ITelemetry telemetry, 
            IContractType1RequiredPaymentEventFundingSourceService fundingSourceService, ILifetimeScope lifetimeScope) 
            : base(actorService, actorId)
        {
            this.paymentLogger = paymentLogger;
            this.telemetry = telemetry;
            this.fundingSourceService = fundingSourceService;
            this.lifetimeScope = lifetimeScope;
        }

        public async Task HandleRequiredPayment(ApprenticeshipContractType1RequiredPaymentEvent message)
        {
            paymentLogger.LogVerbose($"Handling RequiredPayment for {Id}, Job: {message.JobId}, UKPRN: {message.Ukprn}, Account: {message.EmployerAccountId}");

            using (var operation = telemetry.StartOperation())
            {
                await fundingSourceService.AddRequiredPayment(message).ConfigureAwait(false);
                telemetry.StopOperation(operation);
            }
        }

        public async Task<ReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(ProcessLevyPaymentsOnMonthEndCommand command)
        {
            paymentLogger.LogVerbose($"Handling ProcessLevyPaymentsOnMonthEndCommand for {Id}, Job: {command.JobId}, Account: {command.EmployerAccountId}");

            using (var operation = telemetry.StartOperation())
            {
                var fundingSourceEvents = await fundingSourceService.GetFundedPayments(command.EmployerAccountId, command.JobId);
                telemetry.StopOperation(operation);
                return fundingSourceEvents;
            }
        }

        protected override async Task OnActivateAsync()
        {   
            requiredPaymentsCache = new ReliableCollectionCache<ApprenticeshipContractType1RequiredPaymentEvent>(StateManager);
            requiredPaymentKeys = new ReliableCollectionCache<List<string>>(StateManager);
            fundingSourceService = new ContractType1RequiredPaymentEventFundingSourceService(
                lifetimeScope.Resolve<IPaymentProcessor>(),
                lifetimeScope.Resolve<IMapper>(),
                requiredPaymentsCache,
                requiredPaymentKeys,
                lifetimeScope.Resolve<ILevyAccountRepository>(),
                lifetimeScope.Resolve<ILevyBalanceService>(),
                lifetimeScope.Resolve<IPaymentLogger>(),
                lifetimeScope.Resolve<ISortableKeyGenerator>()
            );
            await base.OnActivateAsync().ConfigureAwait(false);
        }

    }
}
