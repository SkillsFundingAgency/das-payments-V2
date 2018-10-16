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

        protected LearnerSubmissionProcessor GenerateEarnings(long ukprn = 12345, string jobId = "job-1234")
        {
            return mocker.Create<LearnerSubmissionProcessor>();
        }

        [Test]
        public void Fails_If_Validation_Fails()
        {
            mocker.Mock<ILearnerValidator>().Setup(x => x.Validate(It.IsAny<FM36Learner>()))
                .Returns(new ValidationResult(new List<ValidationRuleResult> { ValidationRuleResult.Failure("some failure") }));
            var learnerSubmission = mocker.Mock<IIlrLearnerSubmission>()
                .SetupProperty(x => x.Learner, learner)
                .SetupProperty(x => x.CollectionYear, "1819")
                .SetupProperty(x => x.JobId, "job-1")
                .SetupProperty(x => x.Ukprn, 12345);
            var result = mocker.Create<LearnerSubmissionProcessor>().GenerateEarnings(learnerSubmission.Object);
            Assert.IsTrue(result.Validation.Failed);
        }

        [Test]
        public void Generates_Apprenticeship_Contract_Earnings()
        {

        }
    }
}