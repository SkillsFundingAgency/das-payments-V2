using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using IMapper = AutoMapper.IMapper;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public interface IApprenticeshipUpdatedProcessor
    {
        Task ProcessApprenticeshipUpdate(ApprenticeshipUpdated updatedApprenticeship);
        Task<List<DataLockEvent>> GetApprenticeshipUpdatePayments(ApprenticeshipUpdated updatedApprenticeship);
        Task<List<FunctionalSkillDataLockEvent>> GetApprenticeshipUpdateFunctionalSkillPayments(ApprenticeshipUpdated updatedApprenticeship);
    }

    public class ApprenticeshipUpdatedProcessor : IApprenticeshipUpdatedProcessor
    {
        private readonly IPaymentLogger logger;
        private readonly IActorDataCache<List<ApprenticeshipModel>> cache;
        private readonly IMapper mapper;
        private readonly IApprenticeshipRepository repository;
        private readonly IDataLockProcessor dataLockProcessor;
        private readonly IActorDataCache<Act1FunctionalSkillEarningsEvent> act1FunctionalSkillEarningsEventCache;
        private readonly IActorDataCache<ApprenticeshipContractType1EarningEvent> apprenticeshipContractType1EarningEventCache;
        private readonly IGenerateApprenticeshipEarningCacheKey generateApprenticeshipEarningCacheKey;

        public ApprenticeshipUpdatedProcessor(IPaymentLogger logger,
            IActorDataCache<List<ApprenticeshipModel>> cache,
            IMapper mapper,
            IApprenticeshipRepository repository,
            IDataLockProcessor dataLockProcessor,
            IActorDataCache<Act1FunctionalSkillEarningsEvent> act1FunctionalSkillEarningsEventCache,
            IActorDataCache<ApprenticeshipContractType1EarningEvent> apprenticeshipContractType1EarningEventCache,
            IGenerateApprenticeshipEarningCacheKey generateApprenticeshipEarningCacheKey) 
        {

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.dataLockProcessor = dataLockProcessor ?? throw new ArgumentNullException(nameof(dataLockProcessor));
            this.act1FunctionalSkillEarningsEventCache = act1FunctionalSkillEarningsEventCache;
            this.apprenticeshipContractType1EarningEventCache = apprenticeshipContractType1EarningEventCache;
            this.generateApprenticeshipEarningCacheKey = generateApprenticeshipEarningCacheKey;
        }

        public async Task ProcessApprenticeshipUpdate(ApprenticeshipUpdated updatedApprenticeship)
        {
            logger.LogDebug(
                $"Getting apprenticeships cache item using uln for apprenticeship id: {updatedApprenticeship.Id}");
            var cacheItem = await cache.TryGet(updatedApprenticeship.Uln.ToString(), CancellationToken.None);
            logger.LogDebug(cacheItem.HasValue
                ? "Item found in the cache."
                : "No cache item found. Will now create new apprenticeships list.");
            var apprenticeships = cacheItem.HasValue ? cacheItem.Value : new List<ApprenticeshipModel>();
            logger.LogDebug("Removing old version of the apprenticeship.");
            apprenticeships.RemoveAll(apprenticeship => apprenticeship.Id == updatedApprenticeship.Id);
            logger.LogDebug("Now mapping the ApprenticeshipUpdated event to the ApprenticeshipModel.");
            var model = mapper.Map<ApprenticeshipModel>(updatedApprenticeship);
            logger.LogDebug("Finished mapping the apprenticeship model, now adding to the cache.");
            apprenticeships.Add(model);
            await cache.AddOrReplace(model.Uln.ToString(), apprenticeships, CancellationToken.None);
            logger.LogInfo(
                $"Finished storing the apprenticeship details in the cache. Apprenticeship id: {model.Id}, Account: {model.AccountId}, Provider: {model.Ukprn}");
        }

        public async Task<List<DataLockEvent>> GetApprenticeshipUpdatePayments(ApprenticeshipUpdated updatedApprenticeship)
        {
            var earningCacheKey = generateApprenticeshipEarningCacheKey
                .GenerateAct1EarningsKey(updatedApprenticeship.Ukprn, updatedApprenticeship.Uln);

            var apprenticeshipAct1Earning = await apprenticeshipContractType1EarningEventCache
                .TryGet(earningCacheKey)
                .ConfigureAwait(false);

            if (!apprenticeshipAct1Earning.HasValue || apprenticeshipAct1Earning.Value == null) return null;

            var payments = await dataLockProcessor.GetPaymentEvents(apprenticeshipAct1Earning.Value, default(CancellationToken))
                .ConfigureAwait(false);

            return payments;
        }
        
        public async Task<List<FunctionalSkillDataLockEvent>> GetApprenticeshipUpdateFunctionalSkillPayments(ApprenticeshipUpdated updatedApprenticeship)
        {
            var earningCacheKey = generateApprenticeshipEarningCacheKey
                .GenerateAct1FunctionalSkillEarningsKey(updatedApprenticeship.Ukprn, updatedApprenticeship.Uln);

            var apprenticeshipAct1FunctionalSkillEarning = await act1FunctionalSkillEarningsEventCache
                .TryGet(earningCacheKey)
                .ConfigureAwait(false);
            
            if (!apprenticeshipAct1FunctionalSkillEarning.HasValue || apprenticeshipAct1FunctionalSkillEarning.Value == null) return null;
            
            var functionalSkillPayments = await dataLockProcessor.GetFunctionalSkillPaymentEvents(apprenticeshipAct1FunctionalSkillEarning.Value, default(CancellationToken))
                .ConfigureAwait(false);

            return functionalSkillPayments;

        }

        private void AddFunctionalSkillEarnings(EarningEventModel functionalSkillEarning, Act1FunctionalSkillEarningsEvent functionalSkillEarningEvent)
        {
            var allFunctionalSkillTypes = Enum.GetValues(typeof(FunctionalSkillType)).Cast<FunctionalSkillType>().ToList();

            var functionalSkillTypes = functionalSkillEarning.Periods
                .Where(x => allFunctionalSkillTypes.Cast<int>().Contains((int)x.TransactionType))
                .Select(x => (int)x.TransactionType)
                .Cast<FunctionalSkillType>()
                .ToList();

            var functionalSkillEarnings = new List<FunctionalSkillEarning>();

            foreach (var functionalSkillType in functionalSkillTypes)
            {
                var earningPeriodData = GetEarningsPeriodsAndCensusDate(functionalSkillEarning, (int)functionalSkillType);
                var newFunctionalSkillEarning = new FunctionalSkillEarning
                {
                    Type = functionalSkillType,
                    Periods = new ReadOnlyCollection<EarningPeriod>(earningPeriodData.periods)
                };

                functionalSkillEarnings.Add(newFunctionalSkillEarning);
            }

            functionalSkillEarningEvent.Earnings = new ReadOnlyCollection<FunctionalSkillEarning>(functionalSkillEarnings);
        }


        private void AddIncentiveEarnings(EarningEventModel onProgAndIncentiveEarning, ApprenticeshipContractType1EarningEvent act1EarningEvent)
        {
            var allIncentiveEarningTypes = Enum.GetValues(typeof(IncentiveEarningType)).Cast<IncentiveEarningType>().ToList();

            var incentiveEarningTypes = onProgAndIncentiveEarning.Periods
                .Where(x => allIncentiveEarningTypes.Cast<int>().Contains((int)x.TransactionType))
                .Select(x => (int)x.TransactionType)
                .Cast<IncentiveEarningType>()
                .ToList();

            foreach (var incentiveEarningType in incentiveEarningTypes)
            {
                var earningPeriodData = GetEarningsPeriodsAndCensusDate(onProgAndIncentiveEarning, (int)incentiveEarningType);
                var incentiveEarning = new IncentiveEarning
                {
                    Type = incentiveEarningType,
                    CensusDate = earningPeriodData.censusDate,
                    Periods = new ReadOnlyCollection<EarningPeriod>(earningPeriodData.periods)
                };

                act1EarningEvent.IncentiveEarnings.Add(incentiveEarning);
            }
        }

        private void AddOnProgEarnings( EarningEventModel onProgAndIncentiveEarning, ApprenticeshipContractType1EarningEvent act1EarningEvent)
        {
            var onProgEarningTypes = Enum.GetValues(typeof(OnProgrammeEarningType)).Cast<OnProgrammeEarningType>().ToList();
            foreach (var onProgrammeEarningType in onProgEarningTypes)
            {
                var earningPeriodData = GetEarningsPeriodsAndCensusDate(onProgAndIncentiveEarning, (int)onProgrammeEarningType);
                var onProgrammeEarning = new OnProgrammeEarning
                {
                    Type = onProgrammeEarningType,
                    CensusDate = earningPeriodData.censusDate,
                    Periods = new ReadOnlyCollection<EarningPeriod>(earningPeriodData.periods)
                };
                act1EarningEvent.OnProgrammeEarnings.Add(onProgrammeEarning);
            }
        }

        private (List<EarningPeriod> periods, DateTime censusDate) GetEarningsPeriodsAndCensusDate(EarningEventModel earning, int earningType)
        {
            var earningPeriods = earning.Periods.Where(x => (int)x.TransactionType == (int)earningType).ToList();
            if (earningPeriods == null || !earningPeriods.Any())
            {
                throw new ArgumentNullException($"Unable to find On Programme Earning Type {earningType.GetType().ToString()}");
            }

            return (earningPeriods.Select(x => mapper.Map<EarningPeriod>(x)).ToList(), earning.Periods.First().CensusDate ?? DateTime.MaxValue);
        }
    }
}