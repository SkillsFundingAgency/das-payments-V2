using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public interface ICourseValidationProcessor
    {
        //List<ValidationResult> ValidateCourse(DataLockValidationModel validationModel);
        CourseValidationResult ValidateCourse(DataLockValidationModel validationModel);
    }
}