using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Interfaces
{
    public interface IUlnLearnerMatcher
    {
        Task<LearnerMatchResult> MatchUln(long uln);
    }
}