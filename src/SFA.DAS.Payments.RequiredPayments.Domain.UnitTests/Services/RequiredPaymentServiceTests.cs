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
    public class RequiredPaymentServiceTests
    {
        protected RequiredPaymentService Sut;
        protected Mock<IRefundService> RefundService;
        protected Mock<IPaymentDueProcessor> PaymentsDueService;
        protected List<Payment> PaymentHistory;


        [SetUp]
        public void Setup()
        {
            var automocker = AutoMock.GetStrict();
            PaymentHistory = new List<Payment>();
            PaymentsDueService = automocker.Mock<IPaymentDueProcessor>();
            RefundService = automocker.Mock<IRefundService>();
            Sut = new RequiredPaymentService(PaymentsDueService.Object, RefundService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            PaymentsDueService.Verify();
            RefundService.Verify();
        }

        [TestFixture]
        public class WhenAmountIsMoreThanTotalAmountInHistory : RequiredPaymentServiceTests
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
        public class WhenAmountIsLessThanTotalAmountInHistory : RequiredPaymentServiceTests
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
