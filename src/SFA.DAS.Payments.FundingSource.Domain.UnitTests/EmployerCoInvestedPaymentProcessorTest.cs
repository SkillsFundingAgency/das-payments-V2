using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Exceptions;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using System.Collections.Generic;

namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests
{
    [TestFixture]
    public class EmployerCoInvestedPaymentProcessorTest
    {
        private EmployerCoInvestedPaymentProcessor processor;
        private Mock<IValidateRequiredPaymentEvent> validator;

        [Test]
        public void ShouldThrowExceptionIfValidationResultIsNotEmpty()
        {
            var message = new RequiredCoInvestedPayment
            {
                SfaContributionPercentage = 0
            };

            var validationResults = new List<RequiredCoInvestedPaymentValidationResult>
            {
                new RequiredCoInvestedPaymentValidationResult
                {
                    RequiredCoInvestedPayment = message,
                    Rule = RequiredPaymentEventValidationRules.ZeroSfaContributionPercentage
                }
            };

            validator = new Mock<IValidateRequiredPaymentEvent>();
            validator.Setup(o => o.Validate(message)).Returns(validationResults);


            processor = new EmployerCoInvestedPaymentProcessor(validator.Object);

            Assert.That(() => processor.Process(message), Throws.InstanceOf<FundingSourceRequiredPaymentValidationException>());
        }

        [TestCase(0.9, 2000.00, 200.00, 0, 200.00)]
        [TestCase(0.9, 552580.20, 55258.02, 0, 55258.02)]
        [TestCase(1, 552580.20, 0, 0, 0)]
        [TestCase(0.9, 0.66667, 0.06667, 0, 0.06667)]
        [TestCase(0.9, 0.666667, 0.06667, 0, 0.06667)]
        [TestCase(0.9, 100, 0, 100, 100)]
        [TestCase(0.9, 100, 5, 95, 100)]
        [TestCase(1.0, 100, 0, 0, 0)]
        public void GivenValidSfaContributionAndAmountDueShouldReturnValidPayment(
            decimal sfaContribution,
            decimal amountDue,
            decimal expectedAmount,
            decimal amountFunded,
            decimal expectedAmountFunded)
        {
            var message = new RequiredCoInvestedPayment
            {
                SfaContributionPercentage = sfaContribution,
                AmountDue = amountDue,
                AmountFunded = amountFunded
            };
            validator = new Mock<IValidateRequiredPaymentEvent>();
            validator.Setup(o => o.Validate(message)).Returns(new List<RequiredCoInvestedPaymentValidationResult>());
            processor = new EmployerCoInvestedPaymentProcessor(validator.Object);
            var payment = processor.Process(message);
            Assert.AreEqual(expectedAmount, payment.AmountDue);
            Assert.AreEqual(expectedAmountFunded, message.AmountFunded);
        }
    }
}