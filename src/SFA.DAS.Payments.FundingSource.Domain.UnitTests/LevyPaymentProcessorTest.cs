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
                AmountFunded = 0
            };

            // act
            var payment = processor.Process(requiredPayment);

            // assert
            payment.Should().BeOfType<LevyPayment>();
            payment.AmountDue.Should().Be(100);
            payment.Type.Should().Be(FundingSourceType.Levy);

            requiredPayment.AmountFunded.Should().Be(100);
            requiredPayment.LevyBalance.Should().Be(900);
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
        public void TestProcessPaymentNoSfaContribution()
        {
            // arrange
            var requiredPayment = new RequiredLevyPayment
            {
                SfaContributionPercentage = 1m,
                AmountDue = 100,
                LevyBalance = 1000,
                AmountFunded = 50
            };

            // act
            var payment = processor.Process(requiredPayment);

            // assert
            payment.Should().BeOfType<LevyPayment>();
            payment.AmountDue.Should().Be(50);
            payment.Type.Should().Be(FundingSourceType.Levy);

            requiredPayment.AmountFunded.Should().Be(100);
            requiredPayment.LevyBalance.Should().Be(950);
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
                AmountFunded = 0
            };

            // act
            var payment = processor.Process(requiredPayment);

            // assert
            payment.Should().BeOfType<LevyPayment>();
            payment.AmountDue.Should().Be(5);
            payment.Type.Should().Be(FundingSourceType.Levy);

            requiredPayment.AmountFunded.Should().Be(5);
            requiredPayment.LevyBalance.Should().Be(0);
        }

        [Test]
        public void TestProcessPaymentThatWasFundedInFull()
        {
            // arrange
            var requiredPayment = new RequiredLevyPayment
            {
                SfaContributionPercentage = .9m,
                AmountDue = 100,
                LevyBalance = 500,
                AmountFunded = 100
            };

            // act
            var payment = processor.Process(requiredPayment);

            // assert
            payment.Should().BeNull();

            requiredPayment.AmountFunded.Should().Be(100);
            requiredPayment.LevyBalance.Should().Be(500);
        }

        [Test]
        public void TestProcessPaymentThatWasPartiallyFunded()
        {
            // arrange
            var requiredPayment = new RequiredLevyPayment
            {
                SfaContributionPercentage = .9m,
                AmountDue = 100,
                LevyBalance = 500,
                AmountFunded = 50
            };

            // act
            var payment = processor.Process(requiredPayment);

            // assert
            payment.Should().BeOfType<LevyPayment>();
            payment.AmountDue.Should().Be(50);
            payment.Type.Should().Be(FundingSourceType.Levy);

            requiredPayment.AmountFunded.Should().Be(100);
            requiredPayment.LevyBalance.Should().Be(450);
        }

        [Test]
        public void TestProcessRefundPayment()
        {
            // arrange
            var requiredPayment = new RequiredLevyPayment
            {
                SfaContributionPercentage = .9m,
                AmountDue = -600,
                LevyBalance = 500,
                AmountFunded = 0
            };

            // act
            var payment = processor.Process(requiredPayment);

            // assert
            payment.Should().BeOfType<LevyPayment>();
            payment.AmountDue.Should().Be(-600);
            payment.Type.Should().Be(FundingSourceType.Levy);

            requiredPayment.AmountFunded.Should().Be(-600);
            requiredPayment.LevyBalance.Should().Be(1100);
        }
    }
}