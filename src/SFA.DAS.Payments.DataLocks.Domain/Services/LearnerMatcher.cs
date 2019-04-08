using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Interfaces
{
    public class LearnerMatcher : ILearnerMatcher
    {
        public async Task<LearnerMatchResult> MatchLearner(long ukprn, long uln)
        {
            throw new System.NotImplementedException();
        }
    }
}