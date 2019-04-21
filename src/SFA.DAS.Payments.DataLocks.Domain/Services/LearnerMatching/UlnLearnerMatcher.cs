using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching
{
    public class UlnLearnerMatcher : IUlnLearnerMatcher
    {
        private readonly IDataLockLearnerCache dataLockLearnerCache;

        public UlnLearnerMatcher(IDataLockLearnerCache dataLockLearnerCache)
        {
            this.dataLockLearnerCache = dataLockLearnerCache;
        }
        
        public async Task<LearnerMatchResult> MatchUln(long uln)
        {
            var apprenticeships = await dataLockLearnerCache.GetLearnerApprenticeships(uln).ConfigureAwait(false);

            if (!apprenticeships.Any())
            {
                return new LearnerMatchResult
                {
                    DataLockErrorCode = DataLockErrorCode.DLOCK_02
                };
            }

            return new LearnerMatchResult
            {
                Apprenticeships =  apprenticeships
            };
        }
    }
}