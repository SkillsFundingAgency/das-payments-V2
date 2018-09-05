using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;


namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests
{
    [TestFixture]
    public class EmployerCoInvestedPaymentProcessorTest
    {
        private EmployerCoInvestedPaymentProcessor processor;

        [OneTimeSetUp]
        public void Setup()
        {
            processor = new EmployerCoInvestedPaymentProcessor();
        }

        [Test]
        public void ShouldThrowExceptionForZeroSfaContributionPercentage()
        {
            var message = new ApprenticeshipContractType2RequiredPaymentEvent
            {
               SfaContributionPercentage = 0
            };

            var processor = new EmployerCoInvestedPaymentProcessor();

            Assert.That(() => processor.Process(message), Throws.ArgumentException);
        }

        [TestCase(0.9, 20600.87, 2060.087)]
        [TestCase(0.9, 552580.20, 55258.02)]
        [TestCase(1, 552580.20, 0)]
        public void GivenValidSfaContributionAndAmountDueSholudReturnValidPayment(decimal sfaContribution,
                                                                                   decimal amountDue,
                                                                                   decimal expectedAmount)
        {
            var message = new ApprenticeshipContractType2RequiredPaymentEvent
            {
                SfaContributionPercentage = sfaContribution,
                AmountDue = amountDue,
                JobId = "H001"
            };

            var payment = processor.Process(message);

            Assert.AreEqual(expectedAmount, payment.Amount);
        }



    }
}