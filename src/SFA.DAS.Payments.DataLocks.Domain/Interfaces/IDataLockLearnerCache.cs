using SFA.DAS.Payments.Model.Core.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Domain.Interfaces
{
    public interface IDataLockLearnerCache
    {
        Task<bool> HasLearnerRecords();
    }
}