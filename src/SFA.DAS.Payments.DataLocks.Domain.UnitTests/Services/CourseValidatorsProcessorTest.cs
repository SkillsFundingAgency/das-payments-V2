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
        private DataLockValidationModel dataLockValidationModel;
        private Mock<ICourseValidator> startDateValidator;

        [SetUp]
        public void Prepare()
        {
            var earningPeriod = new EarningPeriod
            {
                Period = 1
            };

            dataLockValidationModel = new DataLockValidationModel
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
                .Setup(o => o.Validate(It.IsAny<CourseValidationModel>()))
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
            var actualResults = courseValidatorsProcessor.ValidateCourse(dataLockValidationModel);

            startDateValidator.Verify(o => o.Validate(It.IsAny<CourseValidationModel>()), Times.Once);

            actualResults.Should().NotBeNull();
            actualResults.Should().HaveCount(courseValidationResults.Count);
        }

    }
}
