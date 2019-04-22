using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class CourseValidatorsProcessorTest
    {
        private AutoMock mocker;
        private List<ICourseValidator> courseValidators;
        private List<ValidationResult> courseValidationResults;
        private DataLockValidationModel dataLockValidationModel;
        private Mock<ICourseValidator> startDateValidator;

        [SetUp]
        public void Prepare()
        {
            mocker = AutoMock.GetLoose();
            var earningPeriod = new EarningPeriod
            {
                Period = 1
            };

            dataLockValidationModel = new DataLockValidationModel
            {
                EarningPeriod = earningPeriod,
                Uln = 100,
                PriceEpisode = new PriceEpisode(),
                ApprenticeshipId = 1
            };


            //courseValidationResults = new List<ValidationResult>
            //{
            //    new ValidationResult()
            //};

            //startDateValidator = new Mock<ICourseValidator>(MockBehavior.Strict);
            //startDateValidator
            //    .Setup(o => o.Validate(It.IsAny<DataLockValidationModel>()))
            //    .Returns(courseValidationResults);

            //courseValidators = new List<ICourseValidator>
            //{
            //    startDateValidator.Object
            //};

        }

        //[Test]
        //public void ValidateCourseShouldReturnValidationResults()
        //{
        //    var courseValidatorsProcessor = new CourseValidationProcessor(courseValidators);
        //    var actualResults = courseValidatorsProcessor.ValidateCourse(dataLockValidationModel);

        //    startDateValidator.Verify(o => o.Validate(It.IsAny<DataLockValidationModel>()), Times.Once);

        //    actualResults.Should().NotBeNull();
        //    actualResults.Should().HaveCount(courseValidationResults.Count);
        //}

        [Test]
        public void CallsEachValidatorInExpectedOrder()
        {
            //TODO: find more elegant way to check this
            var expectedResult = new ValidationResult();
            var calledOrder = new List<string>();
            mocker.Mock<IStartDateValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => expectedResult)
                .Callback(() => calledOrder.Add("StartDate"));
            mocker.Mock<INegotiatedPriceValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => expectedResult)
                .Callback(() => calledOrder.Add("NegotiatedPrice"));
            mocker.Mock<IApprenticeshipPauseValidator>()
                .Setup(validator => validator.Validate(It.IsAny<DataLockValidationModel>()))
                .Returns(() => expectedResult)
                .Callback(() => calledOrder.Add("ApprenticeshipPause"));
            var courseValidator = mocker.Create<CourseValidationProcessor>();
            courseValidator.ValidateCourse(dataLockValidationModel);
            calledOrder.Should().NotBeEmpty();
            calledOrder.FirstOrDefault().Should().BeEquivalentTo("StartDate");
            calledOrder.Skip(1).FirstOrDefault().Should().BeEquivalentTo("NegotiatedPrice");
            calledOrder.Skip(2).FirstOrDefault().Should().BeEquivalentTo("ApprenticeshipPause");
        }
    }
}
