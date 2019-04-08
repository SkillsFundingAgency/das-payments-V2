using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
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
            var dataLockErrorCode = await ukprnMatcher.MatchUkprn();
            if (dataLockErrorCode.HasValue)
                return new LearnerMatchResult
                {
                    DataLockErrorCode = dataLockErrorCode
                };

            var ulnMatchResult = await ulnLearnerMatcher.MatchUln(uln);
            if (ulnMatchResult.DataLockErrorCode.HasValue)
                return new LearnerMatchResult
                {
                    DataLockErrorCode = ulnMatchResult.DataLockErrorCode
                };

            return ulnMatchResult;
        }
    }
}