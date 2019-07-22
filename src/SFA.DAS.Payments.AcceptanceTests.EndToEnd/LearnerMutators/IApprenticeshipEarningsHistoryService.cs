using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public interface IApprenticeshipEarningsHistoryService
    {
        Task AddHistoryAsync(int collectionYear, byte collectionPeriod, IEnumerable<Learner> learners);

        Task DeleteHistoryAsync(long ukprn);
    }
}
