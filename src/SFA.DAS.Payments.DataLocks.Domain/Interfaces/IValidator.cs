using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Interfaces
{
    public interface IValidator
    {
        CourseValidationResult Validate(CourseValidation courseValidation, List<ApprenticeshipModel> apprenticeships);
    }
}
