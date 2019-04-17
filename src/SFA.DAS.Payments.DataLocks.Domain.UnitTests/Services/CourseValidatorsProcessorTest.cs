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
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    [TestFixture]
    public class CourseValidatorsProcessorTest
    {
        private List<ICourseValidator> courseValidators;
        private List<ValidationResult>  courseValidationResults ;
        private DataLockValidation dataLockValidation;
        private Mock<ICourseValidator> startDateValidator;

        [SetUp]
        public void Prepare()
        {
            var earningPeriod = new EarningPeriod
            {
                Period = 1
            };

            dataLockValidation = new DataLockValidation
            {
                EarningPeriod = earningPeriod,
                Uln = 100,
                PriceEpisode = new PriceEpisode(),
                Apprenticeships = new List<ApprenticeshipModel> {new ApprenticeshipModel()}
            };

             courseValidationResults = new List<ValidationResult>
            {
                new ValidationResult()
            };

            startDateValidator = new Mock<ICourseValidator>(MockBehavior.Strict);
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
            var courseValidatorsProcessor = new CourseValidatorsProcessor(courseValidators);
            var actualResults = courseValidatorsProcessor.ValidateCourse(dataLockValidation);

            startDateValidator.Verify(o => o.Validate(It.IsAny<CourseValidation>()), Times.Once);

            actualResults.Should().NotBeNull();
            actualResults.Should().HaveCount(courseValidators.Count);
        }

    }
}
