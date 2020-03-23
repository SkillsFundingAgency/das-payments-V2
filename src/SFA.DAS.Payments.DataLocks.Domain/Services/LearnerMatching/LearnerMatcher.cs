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
            return result.DataLockErrorCode.HasValue ? result : ukprnMatcher.MatchUkprn(ukprn, result.Apprenticeships);
        }
    }
}