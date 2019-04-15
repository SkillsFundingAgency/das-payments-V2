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
    public class ProcessCourseValidatorTest
    {
        private List<ICourseValidator> courseValidators;
        private List<ValidationResult>  courseValidationResults ;
        private DataLockValidation dataLockValidation;


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

            var courseValidation = new CourseValidation
            {
                Period = dataLockValidation.EarningPeriod.Period,
                PriceEpisode = dataLockValidation.PriceEpisode,
                Apprenticeships = dataLockValidation.Apprenticeships
            };

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
            var actualResults = processCourseValidator.ValidateCourse(dataLockValidation);

            actualResults.Should().NotBeNull();
            actualResults.Should().HaveCount(courseValidators.Count);
        }

    }
}
