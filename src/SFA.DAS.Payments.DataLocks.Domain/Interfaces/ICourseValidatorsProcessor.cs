using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Interfaces
{
    public interface ICourseValidatorsProcessor
    {
        List<ValidationResult> ValidateCourse(DataLockValidationModel validationModel);
    }
}