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
    public class RefundRemovedLearningAimServiceTests
    {
        private AutoMock mocker;
        private List<Payment> history;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            history = new List<Payment>();
        }

        [Test]
        public void Refunds_All_Delivery_Periods()
        {
            mocker.Mock<IRefundService>().Setup(x => x.GetRefund(It.IsAny<decimal>(), It.IsAny<List<Payment>>()))
                .Returns(new List<RequiredPayment>());

            history.AddRange(Enumerable.Range(1, 12).Select(period => new Payment { DeliveryPeriod = (byte)period, SfaContributionPercentage = .9M, Amount = period * 10, PriceEpisodeIdentifier = "pe-1" }));
            var service = mocker.Create<RefundRemovedLearningAimService>();
            var requiredPayments = service.RefundLearningAim(history);
            for (var i = 1; i <= 12; i++)
            {
                var period = 1;
                mocker.Mock<IRefundService>()
                    .Verify(
                        x => x.GetRefund(It.Is<decimal>(amount => amount == 0M),
                            It.Is<List<Payment>>(payments =>
                                payments.All(payment => payment.DeliveryPeriod == period))), Times.Once);
            }
        }
    }
}