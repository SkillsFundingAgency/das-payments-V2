using System;
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
        protected AutoMock automocker;
        protected RequiredPaymentProcessor sut;
        protected Mock<IRefundService> refundService;
        protected Mock<IPaymentDueProcessor> paymentsDueService;
        protected List<Payment> paymentHistory;


        [SetUp]
        public void Setup()
        {
            automocker = AutoMock.GetStrict();
            paymentHistory = new List<Payment>();
            paymentsDueService = automocker.Mock<IPaymentDueProcessor>();
            refundService = automocker.Mock<IRefundService>();
            sut = automocker.Create<RequiredPaymentProcessor>();
        }

        [TearDown]
        public void TearDown()
        {
            automocker.Dispose();
        }

        [TestFixture]
        public class WhenAmountIsMoreThanTotalAmountInHistory : RequiredPaymentProcessorTests
        {
            [Test]
            public void ThenRequiredPaymentHasCorrectAmount()
            {
                var testEarning = new Earning
                {
                    SfaContributionPercentage = 0,
                };
                var expectedAmount = 50;
                
                paymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(0, paymentHistory)).Returns(expectedAmount);

                var actual = sut.GetRequiredPayments(testEarning, paymentHistory);

                actual.Single().Amount.Should().Be(expectedAmount);
            }

            [Test]
            public void ThenRequiredPaymentHasSfaContributionPercentageOfInput()
            {
                var expectedSfaContribution = 0.7m;

                var testEarning = new Earning
                {
                    SfaContributionPercentage = expectedSfaContribution,
                };
                
                paymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(0, paymentHistory)).Returns(50);

                var actual = sut.GetRequiredPayments(testEarning, paymentHistory);

                actual.Single().SfaContributionPercentage.Should().Be(expectedSfaContribution);
            }

            [Test]
            public void ThenRequiredPaymentHasEarningTypeOfInput()
            {
                var expectedEarningType = EarningType.CoInvested;

                var testEarning = new Earning
                {
                    EarningType = expectedEarningType,
                    SfaContributionPercentage = 0,
                };

                paymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(0, paymentHistory)).Returns(50);

                var actual = sut.GetRequiredPayments(testEarning, paymentHistory);

                actual.Single().EarningType.Should().Be(expectedEarningType);
            }

            [Test]
            public void AndThereIsNoSfaContributionPercentage_ThenAnExceptionIsThrown()
            {
                var testEarning = new Earning();

                paymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(0, paymentHistory)).Returns(50);

                sut.Invoking(x => x.GetRequiredPayments(testEarning, paymentHistory))
                    .Should().Throw<ArgumentException>();
            }
        }

        [TestFixture]
        public class WhenAmountIsEqualToTotalAmountInHistory : RequiredPaymentProcessorTests
        {
            [Test]
            public void ThenThenNothingIsReturned()
            {
                var testEarning = new Earning();
                
                paymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(0, paymentHistory)).Returns(0);
                
                var actual = sut.GetRequiredPayments(testEarning, paymentHistory);

                actual.Should().BeEmpty();
            }
        }

        [TestFixture]
        public class WhenAmountIsLessThanTotalAmountInHistory : RequiredPaymentProcessorTests
        {
            [Test]
            public void ThenRefundIsCreated()
            {
                var testEarning = new Earning();
                var expectedRefundPayments = new List<RequiredPayment>();

                paymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(0, paymentHistory)).Returns(-50);
                refundService.Setup(x => x.GetRefund(-50, paymentHistory)).Returns(expectedRefundPayments);

                var actual = sut.GetRequiredPayments(testEarning, paymentHistory);

                actual.Should().BeSameAs(expectedRefundPayments);
            }
        }
    }
}
