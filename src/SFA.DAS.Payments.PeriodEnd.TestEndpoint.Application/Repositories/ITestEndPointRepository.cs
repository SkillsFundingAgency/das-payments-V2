using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.PeriodEnd.TestEndpoint.Application.Repositories
{
    public interface ITestEndPointRepository
    {
        Task<List<SubmittedLearnerAimModel>> GetProviderLearnerAims( long ukprn, CancellationToken cancellationToken = default(CancellationToken));
    }
}
