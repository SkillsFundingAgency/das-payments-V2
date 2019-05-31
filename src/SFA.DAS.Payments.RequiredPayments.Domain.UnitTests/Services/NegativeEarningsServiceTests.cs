using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
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
            sut = mocker.Create<NegativeEarningsService>();
        }

        [TearDown]
        public void Teardown()
        {
            mocker.Dispose();
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
                new Payment{Amount = 100, DeliveryPeriod = 2, TransactionType = 1, FundingSource = FundingSourceType.Levy},
            };
            var period1History = new List<Payment>
            {
                new Payment{Amount = 100, DeliveryPeriod = 1, TransactionType = 1, FundingSource = FundingSourceType.Levy},
            };

            var payments = new List<RequiredPayment>();
            var sequence = new MockSequence();

            refundService.InSequence(sequence).Setup(x => x.GetRefund(-200, period3History)).Returns(payments);
            refundService.InSequence(sequence).Setup(x => x.GetRefund(-200, period2History)).Returns(payments);
            refundService.InSequence(sequence).Setup(x => x.GetRefund(-200, period1History)).Returns(payments);

            var actual = sut.ProcessNegativeEarning(-200, period1History.Union(period2History).Union(period3History).ToList(), 3, It.IsAny<string>());

            actual.Sum(x => x.Amount).Should().Be(0);
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
