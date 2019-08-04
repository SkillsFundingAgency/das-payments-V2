using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching
{
    public interface IUkprnMatcher
    {
        Task<DataLockErrorCode?> MatchUkprn(long ukprn);
    }
}