using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public interface ICourseValidator
    {
        List<ValidationResult> Validate(DataLockValidationModel courseValidationModel);
    }
}
