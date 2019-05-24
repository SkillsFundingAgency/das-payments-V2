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
        protected AutoMock autoMocker;
        protected RequiredPaymentProcessor sut;
        protected Mock<IRefundService> refundService;
        protected Mock<IPaymentDueProcessor> paymentsDueService;
        protected List<Payment> paymentHistory;
        protected decimal expectedAmount;


        [SetUp]
        public void Setup()
        {
            autoMocker = AutoMock.GetStrict();
            paymentHistory = new List<Payment>();
            paymentsDueService = autoMocker.Mock<IPaymentDueProcessor>();
            refundService = autoMocker.Mock<IRefundService>();
            sut = autoMocker.Create<RequiredPaymentProcessor>();
            expectedAmount = 50;
            refundService
                .Setup(svc => svc.GetRefund(It.IsAny<decimal>(), It.IsAny<List<Payment>>()))
                .Returns(new List<RequiredPayment>());
        }

        [TearDown]
        public void TearDown()
        {
            autoMocker.Dispose();
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
                var testEarning = new Earning
                {
                    SfaContributionPercentage = .9M
                };
                var expectedRefundPayments = new List<RequiredPayment>
                {
                    new RequiredPayment
                    {
                        Amount = -50,
                        SfaContributionPercentage = .9M,
                    }
                };

                paymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(0, paymentHistory)).Returns(-50);
                refundService.Setup(x => x.GetRefund(-50, paymentHistory)).Returns(expectedRefundPayments);

                var actual = sut.GetRequiredPayments(testEarning, paymentHistory);

                actual.All(rp => rp.SfaContributionPercentage == .9M && rp.Amount == -50).Should().BeTrue();
            }
        }

        [TestFixture]
        public class WhenAmountIsZero : RequiredPaymentProcessorTests
        {
            [Test]
            public void ThenFullRefundIsCreated()
            {
                var testEarning = new Earning
                {
                    SfaContributionPercentage = null,
                    Amount = 0
                };
                expectedAmount = -50;
                var expectedRefundPayments = new List<RequiredPayment>
                {
                    new RequiredPayment
                    {
                        Amount = -50,
                        SfaContributionPercentage = .9M,
                    }
                };
                paymentHistory.Add(new Payment { SfaContributionPercentage = .9M });
                paymentHistory.Add(new Payment { SfaContributionPercentage = 1M });
                paymentHistory.Add(new Payment { SfaContributionPercentage = .95M });
                paymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(It.IsAny<decimal>(), It.IsAny<List<Payment>>())).Returns(expectedAmount);
                sut.GetRequiredPayments(testEarning, paymentHistory);
                refundService.Verify(svc => 
                    svc.GetRefund(
                        It.Is<decimal>(amount => amount == expectedAmount), 
                        It.Is<List<Payment>>(history => 
                            history.Count == 3 
                            && history.Any(p => p.SfaContributionPercentage == .9M)
                            && history.Any(p => p.SfaContributionPercentage == .95M)
                            && history.Any(p => p.SfaContributionPercentage == 1M))));
            }
        }

        [TestFixture]
        public class WhenHistoricPaymentsUsedADifferentSfaContribution : RequiredPaymentProcessorTests
        {
            [Test]
            public void ThenHistoricPaymentsWithDifferentSfaContributionShouldNotBeIncludedInRequiredAmountCalculation()
            {
                var testEarning = new Earning
                {
                    SfaContributionPercentage = .9M,
                };
                paymentHistory.Add(new Payment { SfaContributionPercentage = .9M });
                paymentHistory.Add(new Payment { SfaContributionPercentage = 1M });
                paymentHistory.Add(new Payment { SfaContributionPercentage = .95M });
                paymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(It.IsAny<decimal>(), It.IsAny<List<Payment>>())).Returns(expectedAmount);

                sut.GetRequiredPayments(testEarning, paymentHistory);
                paymentsDueService.Verify(x => x.CalculateRequiredPaymentAmount(It.IsAny<decimal>(), It.Is<List<Payment>>(payments => payments.All(p => p.SfaContributionPercentage == .9M))), Times.Once);
            }

            [Test]
            public void ThenTheHistoricPaymentsWithADifferentSfaContributionPercentageShouldBeRefunded()
            {
                var testEarning = new Earning
                {
                    SfaContributionPercentage = .9M,
                    Amount = 5,
                };
                expectedAmount = 0;
                paymentHistory.Add(new Payment { SfaContributionPercentage = .9M, Amount = 5 });
                paymentHistory.Add(new Payment { SfaContributionPercentage = 1M, Amount = 10 });
                paymentHistory.Add(new Payment { SfaContributionPercentage = .95M, Amount = 20 });
                paymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(It.IsAny<decimal>(), It.IsAny<List<Payment>>())).Returns(expectedAmount);
                refundService
                    .Setup(svc => svc.GetRefund(
                        It.Is<decimal>(amount => amount == 0),
                        It.Is<List<Payment>>(payments => payments.All(p => p.SfaContributionPercentage == 1M))))
                    .Returns(new List<RequiredPayment> { new RequiredPayment { SfaContributionPercentage = 1M, Amount = 10 } });
                refundService
                    .Setup(svc => svc.GetRefund(
                        It.Is<decimal>(amount => amount == 0),
                        It.Is<List<Payment>>(payments => payments.All(p => p.SfaContributionPercentage == .95M))))
                    .Returns(new List<RequiredPayment> { new RequiredPayment { SfaContributionPercentage = 95M, Amount = 20 } });
                var requiredPayments = sut.GetRequiredPayments(testEarning, paymentHistory);
                requiredPayments.Count.Should().Be(2);
                requiredPayments.All(rp => rp.SfaContributionPercentage != .9M).Should().BeTrue();
                requiredPayments.Select(rp => rp.Amount).Sum().Should().Be(30M);
            }

            [Test]
            public void ThenShouldRefundHistoricPaymentsWithADifferentSfaContributionAndCreateRequiredPositivePayment()
            {
                var testEarning = new Earning
                {
                    SfaContributionPercentage = .9M,
                    Amount = 50,
                };
                expectedAmount = 45;
                paymentHistory.Add(new Payment { SfaContributionPercentage = .9M, Amount = 5 });
                paymentHistory.Add(new Payment { SfaContributionPercentage = 1M, Amount = 10 });
                paymentHistory.Add(new Payment { SfaContributionPercentage = .95M, Amount = 20 });
                paymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(It.IsAny<decimal>(), It.IsAny<List<Payment>>())).Returns(expectedAmount);

                refundService
                    .Setup(svc => svc.GetRefund(
                        It.Is<decimal>(amount => amount == 0),
                        It.Is<List<Payment>>(payments => payments.All(p => p.SfaContributionPercentage == 1M))))
                    .Returns(new List<RequiredPayment> { new RequiredPayment { SfaContributionPercentage = 1M, Amount = 10 } });

                refundService
                    .Setup(svc => svc.GetRefund(
                        It.Is<decimal>(amount => amount == 0),
                        It.Is<List<Payment>>(payments => payments.All(p => p.SfaContributionPercentage == .95M))))
                    .Returns(new List<RequiredPayment> { new RequiredPayment { SfaContributionPercentage = .95M, Amount = 20 } });

                var requiredPayments = sut.GetRequiredPayments(testEarning, paymentHistory);
                requiredPayments.Count.Should().Be(3);
                requiredPayments.Count(rp => rp.SfaContributionPercentage == .9M).Should().Be(1);
                requiredPayments.Count(rp => rp.SfaContributionPercentage == .95M).Should().Be(1);
                requiredPayments.Count(rp => rp.SfaContributionPercentage == 1M).Should().Be(1);
                requiredPayments.Select(rp => rp.Amount).Sum().Should().Be(expectedAmount + 10 + 20);
            }

            [Test]
            public void ThenShouldRefundHistoricPaymentsWithADifferentSfaContributionAndRefundRequiredPayment()
            {
                var testEarning = new Earning
                {
                    SfaContributionPercentage = .9M,
                    Amount = 0,
                };
                expectedAmount = -5;
                paymentHistory.Add(new Payment { SfaContributionPercentage = .9M, Amount = 5 });
                paymentHistory.Add(new Payment { SfaContributionPercentage = 1M, Amount = 10 });
                paymentHistory.Add(new Payment { SfaContributionPercentage = .95M, Amount = 20 });
                paymentsDueService.Setup(x => x.CalculateRequiredPaymentAmount(It.IsAny<decimal>(), It.IsAny<List<Payment>>())).Returns(expectedAmount);

                refundService
                    .Setup(svc => svc.GetRefund(
                        It.Is<decimal>(amount => amount == expectedAmount),
                        It.Is<List<Payment>>(payments => payments.All(p => p.SfaContributionPercentage == testEarning.SfaContributionPercentage))))
                    .Returns(new List<RequiredPayment> { new RequiredPayment { SfaContributionPercentage = testEarning.SfaContributionPercentage.Value, Amount = expectedAmount } });

                refundService
                    .Setup(svc => svc.GetRefund(
                        It.Is<decimal>(amount => amount == 0),
                        It.Is<List<Payment>>(payments => payments.All(p => p.SfaContributionPercentage == 1M))))
                    .Returns(new List<RequiredPayment> { new RequiredPayment { SfaContributionPercentage = 1M, Amount = -10 } });

                refundService
                    .Setup(svc => svc.GetRefund(
                        It.Is<decimal>(amount => amount == 0),
                        It.Is<List<Payment>>(payments => payments.All(p => p.SfaContributionPercentage == .95M))))
                    .Returns(new List<RequiredPayment> { new RequiredPayment { SfaContributionPercentage = .95M, Amount = -20 } });

                var requiredPayments = sut.GetRequiredPayments(testEarning, paymentHistory);
                requiredPayments.Count.Should().Be(3);
                requiredPayments.Count(rp => rp.SfaContributionPercentage == .9M).Should().Be(1);
                requiredPayments.Count(rp => rp.SfaContributionPercentage == .95M).Should().Be(1);
                requiredPayments.Count(rp => rp.SfaContributionPercentage == 1M).Should().Be(1);
                requiredPayments.Select(rp => rp.Amount).Sum().Should().Be(expectedAmount - 10 - 20);
            }
        }
    }
}
