using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching
{
    public class UkprnMatcher: IUkprnMatcher
    {
        private readonly IDataLockLearnerCache dataLockLearnerCache;

        public UkprnMatcher(IDataLockLearnerCache dataLockLearnerCache)
        {
            this.dataLockLearnerCache = dataLockLearnerCache;
        }
        public async Task<DataLockErrorCode?> MatchUkprn(long ukprn)
        {
            var ukprnExists = await dataLockLearnerCache.UkprnExists(ukprn).ConfigureAwait(false);
            if (!ukprnExists) return DataLockErrorCode.DLOCK_01;

            return default(DataLockErrorCode?);
        }
    }
}
