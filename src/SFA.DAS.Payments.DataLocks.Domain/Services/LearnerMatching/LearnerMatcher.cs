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

        public async Task<LearnerMatchResult> MatchLearner(long uln)
        {
            var dataLockErrorCode = await ukprnMatcher.MatchUkprn().ConfigureAwait(false);
            if (dataLockErrorCode.HasValue)
                return new LearnerMatchResult
                {
                    DataLockErrorCode = dataLockErrorCode
                };

           return await ulnLearnerMatcher.MatchUln(uln).ConfigureAwait(false);
        }
    }
}