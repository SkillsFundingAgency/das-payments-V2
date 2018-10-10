using System.Collections.Generic;
using Autofac.Extras.Moq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Core.Validation;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner;

namespace SFA.DAS.Payments.EarningEvents.Domain.UnitTests
{
    [TestFixture]
    public class LearnerEarningsTests
    {
        private AutoMock mocker;
        private FM36Learner learner;
        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mocker.Mock<ILearnerValidator>().Setup(x => x.Validate(It.IsAny<FM36Learner>()))
                .Returns(new ValidationResult(new List<ValidationRuleResult>()));
            learner = new FM36Learner();
        }

        protected LearnerEarnings GenerateEarnings(long ukprn = 12345, string jobId = "job-1234")
        {
            return mocker.Create<LearnerEarnings>();
        }

        [Test]
        public void Fails_If_Validation_Fails()
        {
            mocker.Mock<ILearnerValidator>().Setup(x => x.Validate(It.IsAny<FM36Learner>()))
                .Returns(new ValidationResult(new List<ValidationRuleResult> { ValidationRuleResult.Failure("some failure") }));
            var result = mocker.Create<LearnerEarnings>().GenerateEarnings(12345, "1819", "job-1234", new FM36Learner());
            Assert.IsTrue(result.Failed);
        }

        [Test]
        public void Generates_Apprenticeship_Contract_Earnings()
        {

        }
    }
}