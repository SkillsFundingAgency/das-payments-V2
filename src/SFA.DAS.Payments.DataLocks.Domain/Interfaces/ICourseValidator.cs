﻿using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Interfaces
{
    public interface ICourseValidator
    {
        List<ValidationResult> Validate(CourseValidation courseValidation);
    }
}
