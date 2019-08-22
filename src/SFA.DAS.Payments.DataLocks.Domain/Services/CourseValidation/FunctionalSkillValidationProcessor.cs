using System.Collections.Generic;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class FunctionalSkillValidationProcessor: CourseValidationProcessor, IFunctionalSkillValidationProcessor
    {
        public FunctionalSkillValidationProcessor(IEnumerable<ICourseValidator> courseValidators) : base(courseValidators)
        {
        }
    }
}