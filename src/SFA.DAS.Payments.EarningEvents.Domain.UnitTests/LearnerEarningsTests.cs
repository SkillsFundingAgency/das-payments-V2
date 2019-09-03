using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Core.Validation;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Incentives;

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
        private Mock<IConfigurationHelper> configurationHelper;
        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetStrict();
            validatorMock = mocker.Mock<ILearnerValidator>();
            actBuilder = mocker.Mock<IApprenticeshipContractTypeEarningsEventBuilder>();
            functionalSkillBuilder = mocker.Mock<IFunctionalSkillEarningsEventBuilder>();
            validatorMock = mocker.Mock<ILearnerValidator>();
            configurationHelper = mocker.Mock<IConfigurationHelper>();

            configurationHelper
                .Setup(x => x.HasSetting("Settings", "GenerateTransactionType4to16Payments"))
                .Returns(true);

            configurationHelper
                .Setup(x => x.GetSetting("Settings", "GenerateTransactionType4to16Payments"))
                .Returns("true");

            validatorMock.Setup(x => x.Validate(It.IsAny<FM36Learner>())).Returns(new ValidationResult(new List<ValidationRuleResult>()));
            learner = new FM36Learner();
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
                CollectionYear = 1819,
                CollectionPeriod = 1,
                Ukprn = 12345,
                JobId = 1
            };
            var result = mocker.Create<LearnerSubmissionProcessor>().GenerateEarnings(learnerSubmission);
            Assert.IsTrue(result.Validation.Failed);

            Mock.Verify(validatorMock, actBuilder, functionalSkillBuilder);
        }

        [Test]
        public void Generates_Apprenticeship_Contract_Earnings()
        {
            // arrange
            var learnerSubmission = new ProcessLearnerCommand
            {
                Learner = learner,
                CollectionYear = 1819,
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
                new Act2FunctionalSkillEarningsEvent(),
                new Act1FunctionalSkillEarningsEvent()
            };

            actBuilder.Setup(b => b.Build(learnerSubmission)).Returns(actEarnings).Verifiable();
            functionalSkillBuilder.Setup(b => b.Build(learnerSubmission)).Returns(functionalSkillEarnings).Verifiable();

            // act
            var result = mocker.Create<LearnerSubmissionProcessor>().GenerateEarnings(learnerSubmission);


            // assert
            Assert.IsFalse(result.Validation.Failed);
            Assert.AreEqual(4, result.EarningEvents.Count);

            Mock.Verify(validatorMock, actBuilder, functionalSkillBuilder);
        }

        [Test]
        public void When_GenerateTransactionType4to16Payments_Is_False_No_FunctionalSkillEarningsEvent_Are_Generates()
        {
            // arrange
            var learnerSubmission = new ProcessLearnerCommand
            {
                Learner = learner,
                CollectionYear = 1819,
                CollectionPeriod = 1,
                Ukprn = 12345,
                JobId = 1
            };

            var actEarnings = new List<ApprenticeshipContractTypeEarningsEvent>
            {
                new ApprenticeshipContractType1EarningEvent(),
                new ApprenticeshipContractType2EarningEvent(),
            };

            var functionalSkillEarnings = new List<FunctionalSkillEarningsEvent>
            {
                new Act2FunctionalSkillEarningsEvent(),
                new Act1FunctionalSkillEarningsEvent()
            };

            configurationHelper
                .Setup(x => x.GetSetting("Settings", "GenerateTransactionType4to16Payments"))
                .Returns("false");


            actBuilder.Setup(b => b.Build(learnerSubmission)).Returns(actEarnings).Verifiable();
            functionalSkillBuilder.Setup(b => b.Build(learnerSubmission)).Returns(functionalSkillEarnings).Verifiable();

            // act
            var result = mocker.Create<LearnerSubmissionProcessor>().GenerateEarnings(learnerSubmission);
            
            // assert
            Assert.IsFalse(result.Validation.Failed);
            Assert.AreEqual(2, result.EarningEvents.Count);
            Assert.AreEqual(2, result.EarningEvents.OfType<ApprenticeshipContractTypeEarningsEvent>().Count());


            actBuilder.Verify(b => b.Build(learnerSubmission));
            functionalSkillBuilder.Verify(b => b.Build(learnerSubmission), Times.Never);

        }
    }
}