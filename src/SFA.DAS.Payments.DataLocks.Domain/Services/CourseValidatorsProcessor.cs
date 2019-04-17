using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public class CourseValidatorsProcessor : ICourseValidatorsProcessor
    {
        private readonly List<ICourseValidator> courseValidators;

        public CourseValidatorsProcessor(IEnumerable<ICourseValidator> courseValidators)
        {
            this.courseValidators = new List<ICourseValidator>(courseValidators);
        }
        
        public List<ValidationResult> ValidateCourse(DataLockValidationModel validationModel)
        {
            var validationResult = new List<ValidationResult>();
            var courseValidationModel =  new CourseValidationModel
            {
                Period = validationModel.EarningPeriod.Period,
                PriceEpisode = validationModel.PriceEpisode,
                Apprenticeships = validationModel.Apprenticeships
            };

            foreach (var courseValidator in courseValidators)
            {
                var results = courseValidator.Validate(courseValidationModel);

                if (results != null && results.Any())
                {
                    validationResult.AddRange(results);
                }
            }

            return validationResult;
        }


    }
}