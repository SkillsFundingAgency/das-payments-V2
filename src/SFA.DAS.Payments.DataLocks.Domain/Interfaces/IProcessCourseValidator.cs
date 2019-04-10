using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Interfaces
{
    public interface IProcessCourseValidator
    {
        Task<List<ValidationResult>> ValidateCourse(DataLockValidation validation,
            List<ApprenticeshipModel> apprenticeships);
    }
}