using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching
{
    public class LearnerMatcher : ILearnerMatcher
    {
        private readonly IUlnLearnerMatcher ulnLearnerMatcher;
        private readonly IUkprnMatcher ukprnMatcher;

        public LearnerMatcher(IUkprnMatcher ukprnMatcher, IUlnLearnerMatcher ulnLearnerMatcher)
        {
            this.ulnLearnerMatcher = ulnLearnerMatcher;
            this.ukprnMatcher = ukprnMatcher;
        }

        public async Task<LearnerMatchResult> MatchLearner(long ukprn, long uln)
        {
            var result = await ulnLearnerMatcher.MatchUln(uln).ConfigureAwait(false);
            if (result.DataLockErrorCode.HasValue)
            {
                return result;
            }

            var dataLockErrorCode = await ukprnMatcher.MatchUkprn(ukprn).ConfigureAwait(false);
            if (dataLockErrorCode.HasValue)
                return new LearnerMatchResult
                {
                    DataLockErrorCode = dataLockErrorCode
                };

            return result;
        }
    }
}