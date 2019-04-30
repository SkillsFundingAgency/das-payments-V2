using System.Collections.Generic;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class InvalidOnProgrammePeriod : OnProgrammePeriodValidationResult
    {
        public List<DataLockErrorCode> DataLockErrors { get; set; }
    }
}