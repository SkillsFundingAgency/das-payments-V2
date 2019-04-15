using System.Collections.Generic;
using NUnit.Framework;
using FluentAssertions;
using Moq;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    [TestFixture]
    public class ProcessCourseValidatorTest
    {
        private List<ICourseValidator> courseValidators;
        private List<ValidationResult>  courseValidationResults ;

        [SetUp]
        public void Prepare()
        {
             courseValidationResults = new List<ValidationResult>
            {
                new ValidationResult()
            };

            var startDateValidator = new Mock<ICourseValidator>();
            startDateValidator
                .Setup(o => o.Validate(It.IsAny<CourseValidation>()))
                .Returns(courseValidationResults);

            courseValidators = new List<ICourseValidator>
            {
                startDateValidator.Object
            };

        }

        [Test]
        public void ValidateCourseShouldReturnValidationResults()
        {
            var processCourseValidator = new ProcessCourseValidator(courseValidators);

            var validation = new DataLockValidation
            {
                EarningPeriod = new EarningPeriod {Period = 1}
            };

            var actualResults = processCourseValidator.ValidateCourse(validation);

            actualResults.Should().NotBeNull();
            actualResults.Should().HaveCount(courseValidators.Count);
        }

    }
}
