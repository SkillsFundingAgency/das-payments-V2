using System;
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
    public class RefundServiceTests
    {
        [Test]
        public void NoEarningReturnsEmptyResult()
        {
            var sut = new RefundService();
            var actual = sut.GetRefund(0, new List<Payment>());
            actual.Should().BeEmpty();
        }

        [Test]
        public void NoHistoryReturnsEmptyResult()
        {
            var sut = new RefundService();
            var actual = sut.GetRefund(-100, new List<Payment>());
            actual.Should().BeEmpty();
        }

        [Test]
        [TestCase(EarningType.CoInvested)]
        [TestCase(EarningType.Incentive)]
        [TestCase(EarningType.Levy)]
        public void EarningsFromASingleFundingSourceReturnRequiredPaymentsFromTheSameSource(EarningType testType)
        {
            var sut = new RefundService();

            var expectedAmount = -123.2m;
            
            var actual = sut.GetRefund(expectedAmount, new List<Payment>
            {
                new Payment
                {
                    Amount = -1 * expectedAmount,
                    FundingSource = ConvertToFundingSource(testType),
                    SfaContributionPercentage = 1m,
                }
            });

            actual.Where(x => x.EarningType == testType).Should().HaveCount(1);
            actual.Single().Amount.Should().Be(expectedAmount);
            actual.Single().SfaContributionPercentage.Should().Be(1);
        }

        FundingSourceType ConvertToFundingSource(EarningType earningType)
        {
            switch (earningType)
            {
                case EarningType.CoInvested:
                    return FundingSourceType.CoInvestedSfa;
                case EarningType.Levy:
                    return FundingSourceType.Levy;
                case EarningType.Incentive:
                    return FundingSourceType.FullyFundedSfa;
            }
            throw new ApplicationException($"Unknown earning type: {earningType}");
        }

        [Test]
        public void CoInvestmentRefund()
        {
            var sut = new RefundService();

            var testHistory = new List<Payment>
            {
                new Payment {Amount = 90, FundingSource = FundingSourceType.CoInvestedSfa, SfaContributionPercentage = 0.9m},
                new Payment {Amount = 10, FundingSource = FundingSourceType.CoInvestedEmployer, SfaContributionPercentage = 0.9m},
            };

            var actual = sut.GetRefund(-50, testHistory);

            actual.Should().HaveCount(1);
            actual[0].EarningType.Should().Be(EarningType.CoInvested);
            actual[0].Amount.Should().Be(-50);
            actual[0].SfaContributionPercentage.Should().Be(0.9m);
        }

        [Test]
        public void CoInvestmentRefundFromLevyEarning()
        {
            var sut = new RefundService();

            var testHistory = new List<Payment>
            {
                new Payment {Amount = 90, FundingSource = FundingSourceType.CoInvestedSfa, SfaContributionPercentage = 0.9m},
                new Payment {Amount = 10, FundingSource = FundingSourceType.CoInvestedEmployer, SfaContributionPercentage = 0.9m},
            };

            var actual = sut.GetRefund(-50, testHistory);

            actual.Should().HaveCount(1);
            actual[0].EarningType.Should().Be(EarningType.CoInvested);
            actual[0].Amount.Should().Be(-50);
            actual[0].SfaContributionPercentage.Should().Be(0.9m);
        }

        [Test]
        public void MixedRefundOneSfaContribution()
        {
            var sut = new RefundService();

            var testHistory = new List<Payment>
            {
                new Payment {Amount = 50, FundingSource = FundingSourceType.Levy, SfaContributionPercentage = 1m},
                new Payment {Amount = 45, FundingSource = FundingSourceType.CoInvestedSfa, SfaContributionPercentage = 0.9m},
                new Payment {Amount = 5, FundingSource = FundingSourceType.CoInvestedEmployer, SfaContributionPercentage = 0.9m},
            };

            var actual = sut.GetRefund(-50, testHistory);

            actual.Should().HaveCount(2);
            actual.Single(x => x.EarningType == EarningType.CoInvested).Amount.Should().Be(-25);
            actual.Single(x => x.EarningType == EarningType.CoInvested).SfaContributionPercentage.Should().Be(0.9m);

            actual.Single(x => x.EarningType == EarningType.Levy).Amount.Should().Be(-25);
            actual.Single(x => x.EarningType == EarningType.Levy).SfaContributionPercentage.Should().Be(1m);
        }

        [Test]
        public void MixedRefundTwoSfaContribution()
        {
            var sut = new RefundService();

            var testHistory = new List<Payment>
            {
                new Payment {Amount = 50, FundingSource = FundingSourceType.Levy, SfaContributionPercentage = 0.9m},
                new Payment {Amount = 45, FundingSource = FundingSourceType.CoInvestedSfa, SfaContributionPercentage = 0.9m},
                new Payment {Amount = 5, FundingSource = FundingSourceType.CoInvestedEmployer, SfaContributionPercentage = 0.9m},
                new Payment {Amount = 40, FundingSource = FundingSourceType.CoInvestedSfa, SfaContributionPercentage = 0.8m},
                new Payment {Amount = 10, FundingSource = FundingSourceType.CoInvestedEmployer, SfaContributionPercentage = 0.8m},
                new Payment {Amount = 50, FundingSource = FundingSourceType.CoInvestedSfa, SfaContributionPercentage = 1m},
            };

            var actual = sut.GetRefund(-200, testHistory);

            actual.Should().HaveCount(4);
            actual.Single(x => x.EarningType == EarningType.CoInvested && x.SfaContributionPercentage == 0.9m).Amount.Should().Be(-50);
            actual.Single(x => x.EarningType == EarningType.CoInvested && x.SfaContributionPercentage == 0.8m).Amount.Should().Be(-50);
            actual.Single(x => x.EarningType == EarningType.CoInvested && x.SfaContributionPercentage == 1m).Amount.Should().Be(-50);
            
            actual.Single(x => x.EarningType == EarningType.Levy).Amount.Should().Be(-50);
            actual.Single(x => x.EarningType == EarningType.Levy).SfaContributionPercentage.Should().Be(0.9m);
        }

        [Test]
        public void LevyRefund()
        {
            var sut = new RefundService();

            var testHistory = new List<Payment>
            {
                new Payment {Amount = 100, FundingSource = FundingSourceType.Levy, SfaContributionPercentage = 0.9m},
            };

            var actual = sut.GetRefund(-50, testHistory);

            actual.Should().HaveCount(1);
            actual[0].EarningType.Should().Be(EarningType.Levy);
            actual[0].Amount.Should().Be(-50);
            actual[0].SfaContributionPercentage.Should().Be(0.9m);
        }

        [Test]
        public void IncentiveRefund()
        {
            var sut = new RefundService();

            var testHistory = new List<Payment>
            {
                new Payment {Amount = 100, FundingSource = FundingSourceType.FullyFundedSfa, SfaContributionPercentage = 0.9m},
            };

            var actual = sut.GetRefund(-50, testHistory);

            actual.Should().HaveCount(1);
            actual[0].EarningType.Should().Be(EarningType.Incentive);
            actual[0].Amount.Should().Be(-50);
            actual[0].SfaContributionPercentage.Should().Be(0.9m);
        }
    }
}
