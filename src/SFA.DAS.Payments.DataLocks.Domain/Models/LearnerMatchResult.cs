using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class LearnerMatchResult
    {
        public DataLockErrorCode? DataLockErrorCode { get; set; }

        public List<ApprenticeshipModel> Apprenticeships { get; set; }
    }
}
