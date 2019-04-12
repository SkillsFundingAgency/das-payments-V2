using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using FluentAssertions;
using Moq;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services;

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
            var actualResults = processCourseValidator.ValidateCourse(new DataLockValidation());

            actualResults.Should().NotBeNull();
            actualResults.Should().HaveCount(courseValidators.Count);
        }

    }
}
