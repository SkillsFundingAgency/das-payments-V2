using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;


namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests
{
    [TestFixture]
    public class SfaCoInvestedPaymentProcessorTest
    {
        private SfaCoInvestedPaymentProcessor processor;

        [OneTimeSetUp]
        public void Setup()
        {
            processor = new SfaCoInvestedPaymentProcessor();
        }
     
        [Test]
        public void ShouldThrowExceptionForZeroSfaContributionPercentage()
        {
            var message = new ApprenticeshipContractType2RequiredPaymentEvent
            {
               SfaContributionPercentage = 0
            };

            Assert.That(() => processor.Process(message), Throws.ArgumentException);
        }

        [TestCase(0.9, 20600.87, 18540.783)]
        [TestCase(0.9, 552580.20, 497322.18)]
        [TestCase(1, 552580.20, 552580.20)]
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