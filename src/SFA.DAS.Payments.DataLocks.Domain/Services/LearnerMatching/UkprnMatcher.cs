using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching
{
    public class UkprnMatcher: IUkprnMatcher
    {
        private readonly IDataLockLearnerCache dataLockLearnerCache;

        public UkprnMatcher(IDataLockLearnerCache dataLockLearnerCache)
        {
            this.dataLockLearnerCache = dataLockLearnerCache;
        }
        public async Task<DataLockErrorCode?> MatchUkprn()
        {
            var hasLearnerData = await dataLockLearnerCache.HasLearnerRecords().ConfigureAwait(false);
            if (!hasLearnerData) return DataLockErrorCode.DLOCK_01;

            return default(DataLockErrorCode?);
        }
    }
}
