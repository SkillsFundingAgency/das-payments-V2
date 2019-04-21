using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public interface IDataLockLearnerCache
    {
        Task<bool> HasLearnerRecords();
        Task<List<ApprenticeshipModel>> GetLearnerApprenticeships(long uln);
    }
}