using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class NegativeEarningsServiceTests
    {
        [Test]
        public void ShouldNotRefundMoreThanPaymentHistory()
        {
            var sut = new NegativeEarningsService();
            var history = new List<Payment>
            {
                new Payment{Amount = 200, DeliveryPeriod = 1, TransactionType = 1, FundingSource = FundingSourceType.Levy},
            };

            var actual = sut.ProcessNegativeEarning(-300, history, 2);

            actual.First().Amount.Should().Be(-200);
        }

        [Test]
        public void ShouldRefundPeriodsWithSameLevyRatio()
        {
            var sut = new NegativeEarningsService();
            var history = new List<Payment>
            {
                new Payment{Amount = 100, DeliveryPeriod = 2, TransactionType = 1, FundingSource = FundingSourceType.Levy},
                new Payment{Amount = 200, DeliveryPeriod = 1, TransactionType = 1, FundingSource = FundingSourceType.CoInvestedEmployer},
                new Payment{Amount = 200, DeliveryPeriod = 1, TransactionType = 1, FundingSource = FundingSourceType.CoInvestedSfa},
            };

            var actual = sut.ProcessNegativeEarning(-200, history, 3);

            actual.Where(x => x.EarningType == EarningType.Levy).Sum(x => x.Amount).Should().Be(-100);
            actual.Where(x => x.EarningType == EarningType.CoInvested).Sum(x => x.Amount).Should().Be(-100);
        }

        [Test]
        public void ShouldRefundFromCurrentPeriodIfPossible()
        {
            var sut = new NegativeEarningsService();
            var history = new List<Payment>
            {
                new Payment{Amount = 100, DeliveryPeriod = 2, TransactionType = 1, FundingSource = FundingSourceType.Levy},
                new Payment{Amount = 200, DeliveryPeriod = 1, TransactionType = 1, FundingSource = FundingSourceType.CoInvestedEmployer},
                new Payment{Amount = 200, DeliveryPeriod = 1, TransactionType = 1, FundingSource = FundingSourceType.CoInvestedSfa},
            };

            var actual = sut.ProcessNegativeEarning(-100, history, 2);

            actual.Where(x => x.EarningType == EarningType.Levy).Sum(x => x.Amount).Should().Be(-100);
            actual.Where(x => x.EarningType != EarningType.Levy).Should().BeEmpty();
        }

        [Test]
        public void ShouldRefundMoreRecentPeriodFirst()
        {
            ShouldRefundPeriodsWithSameLevyRatio();
        }

        [Test]
        public void ShouldRefundFullPaymentHistoryAmountIfRefundAmountIsMore()
        {
            var sut = new NegativeEarningsService();
            var history = new List<Payment>
            {
                new Payment{Amount = 100, DeliveryPeriod = 2, TransactionType = 1, FundingSource = FundingSourceType.Levy},
                new Payment{Amount = 200, DeliveryPeriod = 1, TransactionType = 1, FundingSource = FundingSourceType.CoInvestedEmployer},
                new Payment{Amount = 200, DeliveryPeriod = 1, TransactionType = 1, FundingSource = FundingSourceType.CoInvestedSfa},
            };

            var actual = sut.ProcessNegativeEarning(-600, history, 3);

            actual.Sum(x => x.Amount).Should().Be(-500);
        }

        [Test]
        public void ShouldRefundIfRefundAmountIsLowerThanPaymentHistory()
        {
            var sut = new NegativeEarningsService();
            var history = new List<Payment>
            {
                new Payment{Amount = 100, DeliveryPeriod = 2, TransactionType = 1, FundingSource = FundingSourceType.Levy},
                new Payment{Amount = 200, DeliveryPeriod = 1, TransactionType = 1, FundingSource = FundingSourceType.CoInvestedEmployer},
                new Payment{Amount = 200, DeliveryPeriod = 1, TransactionType = 1, FundingSource = FundingSourceType.CoInvestedSfa},
            };

            var actual = sut.ProcessNegativeEarning(-200, history, 3);

            actual.Sum(x => x.Amount).Should().Be(-200);
        }
    }
}
