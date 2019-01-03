using System.Collections.Generic;
using Autofac.Extras.Moq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Core.Validation;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Domain.UnitTests
{
    [TestFixture]
    public class LearnerEarningsTests
    {
        private AutoMock mocker;
        private FM36Learner learner;
        private Mock<ILearnerValidator> validatorMock;
        private Mock<IApprenticeshipContractTypeEarningsEventBuilder> actBuilder;
        private Mock<IFunctionalSkillEarningsEventBuilder> functionalSkillBuilder;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetStrict();
            validatorMock = mocker.Mock<ILearnerValidator>();
            actBuilder = mocker.Mock<IApprenticeshipContractTypeEarningsEventBuilder>();
            functionalSkillBuilder = mocker.Mock<IFunctionalSkillEarningsEventBuilder>();
            validatorMock = mocker.Mock<ILearnerValidator>();
            validatorMock.Setup(x => x.Validate(It.IsAny<FM36Learner>())).Returns(new ValidationResult(new List<ValidationRuleResult>()));
            learner = new FM36Learner();
        }

        [TearDown]
        public void TearDown()
        {
            Mock.Verify(validatorMock, actBuilder, functionalSkillBuilder);
        }

        [Test]
        public void Fails_If_Validation_Fails()
        {
            mocker.Mock<ILearnerValidator>()
                .Setup(x => x.Validate(It.IsAny<FM36Learner>()))
                .Returns(new ValidationResult(new List<ValidationRuleResult> { ValidationRuleResult.Failure("some failure") }))
                .Verifiable();

            var learnerSubmission =new ProcessLearnerCommand
            {
                Learner = learner,
                CollectionYear = "1819",
                CollectionPeriod = 1,
                Ukprn = 12345,
                JobId = 1
            };
            var result = mocker.Create<LearnerSubmissionProcessor>().GenerateEarnings(learnerSubmission);
            Assert.IsTrue(result.Validation.Failed);
        }

        [Test]
        public void Generates_Apprenticeship_Contract_Earnings()
        {
            // arrange
            var learnerSubmission = new ProcessLearnerCommand
            {
                Learner = learner,
                CollectionYear = "1819",
                CollectionPeriod = 1,
                Ukprn = 12345,
                JobId = 1
            };

            var actEarnings = new List<ApprenticeshipContractTypeEarningsEvent>
            {
                new ApprenticeshipContractType2EarningEvent(),
                new ApprenticeshipContractType2EarningEvent()
            };

            var functionalSkillEarnings = new List<FunctionalSkillEarningsEvent>
            {
                new FunctionalSkillEarningsEvent()
            };

            actBuilder.Setup(b => b.Build(learnerSubmission)).Returns(actEarnings).Verifiable();
            functionalSkillBuilder.Setup(b => b.Build(learnerSubmission)).Returns(functionalSkillEarnings).Verifiable();

            // act
            var result = mocker.Create<LearnerSubmissionProcessor>().GenerateEarnings(learnerSubmission);


            // assert
            Assert.IsFalse(result.Validation.Failed);
            Assert.AreEqual(3, result.EarningEvents.Count);
        }
    }
}