using System.Collections.ObjectModel;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class UlnLearnerMatchResult
    {
        public DataLockErrorCode? ErrorCode { get; set; }
        public ReadOnlyCollection<ApprenticeshipModel> Apprenticeships { get; set; }
    }
}
