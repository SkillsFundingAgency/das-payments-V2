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
    public class SfaCoInvestedPaymentProcessorTest
    {
        private SfaCoInvestedPaymentProcessor processor;
        private Mock<IValidateRequiredPaymentEvent> validator;

     
        [Test]
        public void ShouldThrowExceptionIfValidationResultIsNotNull()
        {
            var message = new RequiredCoInvestedPayment
            {
                SfaContributionPercentage = 0
            };

            var validationResults = new List<RequiredCoInvestedPaymentValidationResult>
            {
                new RequiredCoInvestedPaymentValidationResult
                {
                    RequiredCoInvestedPayment =message,
                    Rule = RequiredPaymentEventValidationRules.ZeroSfaContributionPercentage
                }
            };

            validator = new Mock<IValidateRequiredPaymentEvent>();
            validator.Setup(o => o.Validate(message)).Returns(validationResults);

            processor = new SfaCoInvestedPaymentProcessor(validator.Object);
            
            Assert.That(() => processor.Process(message), Throws.InstanceOf<FundingSourceRequiredPaymentValidationException>());
        }

        [TestCase(0.9, 100, 90)]
        [TestCase(0.9, 200, 180)]
        [TestCase(1, 500, 500)]
        [TestCase(0.9, 0.66667, 0.6)]
        [TestCase(0.9, 0.666667, 0.6)]
        [TestCase(0.9, -600, -540)]
        public void GivenValidSfaContributionAndAmountDueShouldReturnValidPayment(decimal sfaContribution, 
                                                                                    decimal amountDue,
                                                                                    decimal expectedAmount)
        {
            var message = new RequiredCoInvestedPayment
            {
                SfaContributionPercentage = sfaContribution,
                AmountDue = amountDue
            };

            validator = new Mock<IValidateRequiredPaymentEvent>();
            validator.Setup(o => o.Validate(message)).Returns(new List<RequiredCoInvestedPaymentValidationResult>());
            processor = new SfaCoInvestedPaymentProcessor(validator.Object);

            var payment = processor.Process(message);

            Assert.AreEqual(expectedAmount, payment.AmountDue);
        }


    }
}