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
    }

    public class ApprenticeshipUpdatedProcessor : IApprenticeshipUpdatedProcessor
    {
        private readonly IPaymentLogger logger;
        private readonly IActorDataCache<List<ApprenticeshipModel>> cache;
        private readonly IMapper mapper;
        private readonly IApprenticeshipRepository repository;
        private readonly IDataLockProcessor dataLockProcessor;

        public ApprenticeshipUpdatedProcessor(IPaymentLogger logger,
            IActorDataCache<List<ApprenticeshipModel>> cache,
            IMapper mapper,
            IApprenticeshipRepository repository,
            IDataLockProcessor dataLockProcessor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.dataLockProcessor = dataLockProcessor ?? throw new ArgumentNullException(nameof(dataLockProcessor));
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
            var apprenticeshipEarning = await GetApprenticeshipEarningsToReProcess(updatedApprenticeship.Uln, updatedApprenticeship.Ukprn)
                .ConfigureAwait(false);

            var payments = await dataLockProcessor.GetPaymentEvents(apprenticeshipEarning, default(CancellationToken))
                .ConfigureAwait(false);

            return payments;
        }

        private async Task<ApprenticeshipContractType1EarningEvent> GetApprenticeshipEarningsToReProcess(long uln, long ukprn)
        {
            var apprenticeshipEarnings = await repository
                        .GetProviderApprenticeshipEarnings(uln, ukprn, default(CancellationToken))
                        .ConfigureAwait(false);

            var onProgEarningTypes = Enum.GetValues(typeof(OnProgrammeEarningType)).Cast<OnProgrammeEarningType>();
            var incentiveEarningTypes = Enum.GetValues(typeof(IncentiveEarningType)).Cast<IncentiveEarningType>();

            var onProgAndIncentiveEarning = apprenticeshipEarnings.FirstOrDefault(x => x.Periods.Any(p => onProgEarningTypes.Cast<int>().Contains((int)p.TransactionType)));

            var act1EarningEvent = mapper.Map<ApprenticeshipContractType1EarningEvent>(onProgAndIncentiveEarning);
            act1EarningEvent.OnProgrammeEarnings = new List<OnProgrammeEarning>();
            act1EarningEvent.IncentiveEarnings = new List<IncentiveEarning>();
            
            if (onProgAndIncentiveEarning == null) throw new ArgumentNullException($"Unable to find On Programme Earnings from Event Id {apprenticeshipEarnings[0].EventId}");

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

            if (onProgAndIncentiveEarning == null) throw new ArgumentNullException($"Unable to find On Programme Earnings from Event Id {apprenticeshipEarnings[0].EventId}");

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

            act1EarningEvent.Learner = mapper.Map<Learner>(onProgAndIncentiveEarning);
            act1EarningEvent.LearningAim = mapper.Map<LearningAim>(onProgAndIncentiveEarning);

            return act1EarningEvent;
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