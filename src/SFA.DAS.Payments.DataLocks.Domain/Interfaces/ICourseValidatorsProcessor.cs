using SFA.DAS.Payments.DataLocks.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.Payments.DataLocks.Domain.Interfaces
{
    public interface ICourseValidatorsProcessor
    {
        List<ValidationResult> ValidateCourse(DataLockValidationModel validationModel);
    }
}