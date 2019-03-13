using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class RequiredPaymentProcessorTests
    {
        protected AutoMock Automocker;
        protected RequiredPaymentProcessor Sut;
        protected Mock<IRefundService> RefundService;
        protected Mock<IPaymentDueProcessor> PaymentsDueService;
        protected List<Payment> PaymentHistory;


        [SetUp]
        public void Setup()
        {
            Automocker = AutoMock.GetStrict();
            PaymentHistory = new List<Payment>();
            PaymentsDueService = Automocker.Mock<IPaymentDueProcessor>();
            RefundService = Automocker.Mock<IRefundService>();
            Sut = new RequiredPaymentProcessor(PaymentsDueService.Object, RefundService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            Automocker.Dispose();
        }

        [TestFixture]
        public class WhenAmountIsMoreThanTotalAmountInHistory : RequiredPaymentProcessorTests
        {
            [Test]
            public void RequiredPaymentHasCorrectAmount()
            {
                var testEarning = new Earning
                {
                    SfaContributionPercentage = 0,
                };
                var expectedAmount = 50;
                
                PaymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(0, PaymentHistory)).Returns(expectedAmount);

                var actual = Sut.GetRequiredPayments(testEarning, PaymentHistory);

                actual.Single().Amount.Should().Be(expectedAmount);
            }

            [Test]
            public void RequiredPaymentHasSfaContributionPercentageOfInput()
            {
                var expectedSfaContribution = 0.7m;

                var testEarning = new Earning
                {
                    SfaContributionPercentage = expectedSfaContribution,
                };
                
                PaymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(0, PaymentHistory)).Returns(50);

                var actual = Sut.GetRequiredPayments(testEarning, PaymentHistory);

                actual.Single().SfaContributionPercentage.Should().Be(expectedSfaContribution);
            }

            [Test]
            public void RequiredPaymentHasEarningTypeOfInput()
            {
                var expectedEarningType = EarningType.Levy;

                var testEarning = new Earning
                {
                    EarningType = EarningType.Levy,
                    SfaContributionPercentage = 0,
                };

                PaymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(0, PaymentHistory)).Returns(50);

                var actual = Sut.GetRequiredPayments(testEarning, PaymentHistory);

                actual.Single().EarningType.Should().Be(expectedEarningType);
            }
        }

        [TestFixture]
        public class WhenAmountIsEqualToTotalAmountInHistory : RequiredPaymentProcessorTests
        {
            [Test]
            public void ThenNothingIsReturned()
            {
                var testEarning = new Earning();
                
                PaymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(0, PaymentHistory)).Returns(0);
                
                var actual = Sut.GetRequiredPayments(testEarning, PaymentHistory);

                actual.Should().BeEmpty();
            }
        }

        [TestFixture]
        public class WhenAmountIsLessThanTotalAmountInHistory : RequiredPaymentProcessorTests
        {
            [Test]
            public void RefundIsCreated()
            {
                var testEarning = new Earning();
                var expectedRefundPayments = new List<RequiredPayment>();

                PaymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(0, PaymentHistory)).Returns(-50);
                RefundService.Setup(x => x.GetRefund(-50, PaymentHistory)).Returns(expectedRefundPayments);

                var actual = Sut.GetRequiredPayments(testEarning, PaymentHistory);

                actual.Should().BeSameAs(expectedRefundPayments);
            }
        }
    }
}
