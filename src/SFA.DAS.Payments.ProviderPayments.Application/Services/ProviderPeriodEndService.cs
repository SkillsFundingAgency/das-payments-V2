using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IProviderPeriodEndService
    {
        Task<bool> IsMonthEndStarted(long ukprn, short academicYear, byte collectionPeriod);
        Task<long> GetMonthEndJobId(long ukprn, short academicYear, byte collectionPeriod);
    }

    public class ProviderPeriodEndService : IProviderPeriodEndService
    {
        private readonly IMonthEndCache monthEndCache;

        public ProviderPeriodEndService(IMonthEndCache monthEndCache)
        {
            this.monthEndCache = monthEndCache ?? throw new ArgumentNullException(nameof(monthEndCache));
        }

        public async Task<bool> IsMonthEndStarted(long ukprn, short academicYear, byte collectionPeriod)
        {
            return await monthEndCache.Exists( ukprn,  academicYear,  collectionPeriod).ConfigureAwait(false);
        }

        public async Task<long> GetMonthEndJobId(long ukprn, short academicYear, byte collectionPeriod)
        {
            var monthEndData =  await monthEndCache.GetMonthEndDetails(ukprn, academicYear, collectionPeriod).ConfigureAwait(false);
            return monthEndData.JobId;
        }
    }
}