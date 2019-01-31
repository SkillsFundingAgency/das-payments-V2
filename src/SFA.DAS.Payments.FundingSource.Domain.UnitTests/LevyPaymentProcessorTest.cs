using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests
{
    [TestFixture]
    public class LevyPaymentProcessorTest
    {
        private ILevyPaymentProcessor processor;

        [SetUp]
        public void SetUp()
        {
            processor = new LevyPaymentProcessor();
        }

        [Test]
        public void TestProcessPaymentWithEnoughLevyBalance()
        {
            // arrange
            var requiredPayment = new RequiredLevyPayment
            {
                SfaContributionPercentage = .9m,
                AmountDue = 100,
                LevyBalance = 1000,
                AmountFunded = 90
            };

            // act
            var payment = processor.Process(requiredPayment);

            // assert
            payment.Should().BeOfType<LevyPayment>();
            payment.AmountDue.Should().Be(10);
            payment.Type.Should().Be(FundingSourceType.Levy);

            requiredPayment.AmountFunded.Should().Be(100);
            requiredPayment.LevyBalance.Should().Be(990);
        }

        [Test]
        public void TestProcessPaymentWithZeroLevyBalance()
        {
            // arrange
            var requiredPayment = new RequiredLevyPayment
            {
                SfaContributionPercentage = .9m,
                AmountDue = 100,
                LevyBalance = 0,
                AmountFunded = 90
            };

            // act
            var payment = processor.Process(requiredPayment);

            // assert
            payment.Should().BeNull();

            requiredPayment.AmountFunded.Should().Be(90);
            requiredPayment.LevyBalance.Should().Be(0);
        }

        [Test]
        public void TestProcessPaymentWithNotEnoughLevyBalance()
        {
            // arrange
            var requiredPayment = new RequiredLevyPayment
            {
                SfaContributionPercentage = .9m,
                AmountDue = 100,
                LevyBalance = 5,
                AmountFunded = 90
            };

            // act
            var payment = processor.Process(requiredPayment);

            // assert
            payment.Should().BeOfType<LevyPayment>();
            payment.AmountDue.Should().Be(5);
            payment.Type.Should().Be(FundingSourceType.Levy);

            requiredPayment.AmountFunded.Should().Be(95);
            requiredPayment.LevyBalance.Should().Be(0);
        }
    }
}
