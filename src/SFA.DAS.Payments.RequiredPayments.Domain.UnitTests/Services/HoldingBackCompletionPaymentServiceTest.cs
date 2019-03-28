using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class HoldingBackCompletionPaymentServiceTest
    {
        private IHoldingBackCompletionPaymentService service;

        [SetUp]
        public void SetUp()
        {
            service = new HoldingBackCompletionPaymentService();
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void CompletionPaymentIsNotHeldWhenExemptCodeIsSet(int exemptionCode)
        {
            // arrange
            var paymentHistory = 9;
            var priceEpisode = new PriceEpisode
            {
                EmployerContribution = 10,
                CompletionHoldBackExemptionCode = exemptionCode
            };

            // act
            var result = service.HoldBackCompletionPayment(paymentHistory, priceEpisode);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        [TestCase(0)]
        [TestCase(9)]
        public void CompletionPaymentIsHeldWhenPaymentsInsufficient(decimal paymentHistory)
        {
            // arrange
            var priceEpisode = new PriceEpisode
            {
                EmployerContribution = 10,
                CompletionHoldBackExemptionCode = 0
            };

            // act
            var result = service.HoldBackCompletionPayment(paymentHistory, priceEpisode);

            // assert
            result.Should().BeTrue();
        }

        [Test]
        [TestCase(10)]
        [TestCase(11)]
        public void CompletionPaymentIsNotHeldWhenPaymentsSufficient(decimal paymentHistory)
        {
            // arrange
            var priceEpisode = new PriceEpisode
            {
                EmployerContribution = 10,
                CompletionHoldBackExemptionCode = 0
            };

            // act
            var result = service.HoldBackCompletionPayment(paymentHistory, priceEpisode);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void CompletionPaymentIsNotHeldWhenEmployerContributionNull()
        {
            // arrange
            var paymentHistory = 10;
            var priceEpisode = new PriceEpisode
            {
                EmployerContribution = null,
                CompletionHoldBackExemptionCode = 0
            };

            // act
            var result = service.HoldBackCompletionPayment(paymentHistory, priceEpisode);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void CompletionPaymentIsNotHeldWhenExemptCodeIsNull()
        {
            // arrange
            var paymentHistory = 10;
            var priceEpisode = new PriceEpisode
            {
                EmployerContribution = 12,
                CompletionHoldBackExemptionCode = null
            };

            // act
            var result = service.HoldBackCompletionPayment(paymentHistory, priceEpisode);

            // assert
            result.Should().BeFalse();
        }

    }
}
