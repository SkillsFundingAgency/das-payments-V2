using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class NegativeEarningsServiceTests
    {
        private NegativeEarningsService sut;
        private Mock<IRefundService> refundService;
        private AutoMock mocker;
        
        [SetUp]
        public void Setup()
        {
            mocker = AutoMock.GetStrict();
            refundService = mocker.Mock<IRefundService>();
            sut = new NegativeEarningsService(new RefundService());
        }

        [TearDown]
        public void Teardown()
        {
            mocker.Dispose();
        }

        [Test, AutoData]
        public void ShouldReturnCorrectApprenticeshipId(Payment paymentHistory)
        {
            paymentHistory.DeliveryPeriod = 1;
            var testPaymentHistory = new List<Payment>{paymentHistory};

            var actual = sut.ProcessNegativeEarning(-1, testPaymentHistory, 1, "");
            actual.Should().BeEquivalentTo(new
            {
                ApprenticeshipId = paymentHistory.ApprenticeshipId,
            });
        }

        [Test, AutoData]
        public void ShouldReturnCorrectApprenticeshipPriceEpisodeId(Payment paymentHistory)
        {
            paymentHistory.DeliveryPeriod = 1;
            var testPaymentHistory = new List<Payment> { paymentHistory };

            var actual = sut.ProcessNegativeEarning(-1, testPaymentHistory, 1, "");
            actual.Should().BeEquivalentTo(new
            {
                ApprenticeshipPriceEpisodeId = paymentHistory.ApprenticeshipPriceEpisodeId,
            });
        }

        [Test, AutoData]
        public void ShouldReturnCorrectApprenticeshipEmployerType(Payment paymentHistory)
        {
            paymentHistory.DeliveryPeriod = 1;
            var testPaymentHistory = new List<Payment> { paymentHistory };

            var actual = sut.ProcessNegativeEarning(-1, testPaymentHistory, 1, "");
            actual.Should().BeEquivalentTo(new
            {
                ApprenticeshipEmployerType = paymentHistory.ApprenticeshipEmployerType,
            });
        }

        [Test]
        public void ShouldNotRefundMoreThanPaymentHistory()
        {
            var history = new List<Payment>
            {
                new Payment{Amount = 200, DeliveryPeriod = 1, TransactionType = 1, FundingSource = FundingSourceType.Levy},
            };

            var payments = new List<RequiredPayment>
            {
                new RequiredPayment {Amount = -200},
            };

            refundService.Setup(x => x.GetRefund(-300, history)).Returns(payments);


            var actual = sut.ProcessNegativeEarning(-300, history, 2, It.IsAny<string>());

            actual.First().Amount.Should().Be(-200);
        }

        [Test]
        public void ShouldWorkBackwardsThroughPeriods()
        {
            var period3History = new List<Payment>
            {
                new Payment{Amount = 100, DeliveryPeriod = 3, TransactionType = 1, FundingSource = FundingSourceType.Levy},
            };
            var period2History = new List<Payment>
            {
                new Payment{Amount = 100, DeliveryPeriod = 2, TransactionType = 1, FundingSource = FundingSourceType.CoInvestedSfa},
            };
            var period1History = new List<Payment>
            {
                new Payment{Amount = 100, DeliveryPeriod = 1, TransactionType = 1, FundingSource = FundingSourceType.Transfer},
            };

            var actual = sut.ProcessNegativeEarning(-150, period1History.Union(period2History).Union(period3History).ToList(), 3, It.IsAny<string>());

            actual.Sum(x => x.Amount).Should().Be(-150);
            actual.Should().HaveCount(2);
            actual.Should().BeEquivalentTo(new
            {
                EarningType = EarningType.Levy,
                Amount = -100,
            }, new
            {
                EarningType = EarningType.CoInvested,
                Amount = -50,
            });
        }

        [Test]
        public void PriceEpisodeOfRefundsShouldBeTheSameAsPassedIn()
        {
            var expectedPriceEpisode = "test price episode";

            var history = new List<Payment>
            {
                new Payment{Amount = 100, DeliveryPeriod = 2, TransactionType = 1, FundingSource = FundingSourceType.Levy},
            };

            var payments = new List<RequiredPayment>
            {
                new RequiredPayment(),
            };

            refundService.Setup(x => x.GetRefund(-100, history)).Returns(payments);


            var actual = sut.ProcessNegativeEarning(-100, history, 3, expectedPriceEpisode);

            actual.First().PriceEpisodeIdentifier.Should().Be(expectedPriceEpisode);
        }
    }
}
