using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Domain.Interfaces
{
    public interface IDataLockLearnerCache
    {
        Task<bool> HasLearnerRecords();
        Task<List<ApprenticeshipModel>> GetLearnerApprenticeships(long uln);
    }
}