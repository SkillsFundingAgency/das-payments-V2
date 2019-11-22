using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.AutoFixture;

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
        public void NoHistoryForFundingSourceReturnsNoRequiredPayment()
        {
            var sut = new RefundService();
            var actual = sut.GetRefund(-100, new List<Payment>
            {
                new Payment{Amount = 100, FundingSource = FundingSourceType.Levy},
                new Payment{Amount = 0, FundingSource = FundingSourceType.FullyFundedSfa},
            });

            actual.Should().HaveCount(1);
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

        [Test, AutoData]
        [InlineAutoData(ApprenticeshipEmployerType.Levy)]
        [InlineAutoData(ApprenticeshipEmployerType.NonLevy)]
        public void RefundProducesRequiredPaymentsWithOriginalEmployerType(
            ApprenticeshipEmployerType testType, RefundService sut)
        {
            var expectedAmount = -123.2m;
            
            var actual = sut.GetRefund(expectedAmount, new List<Payment>
            {
                new Payment
                {
                    Amount = -1 * expectedAmount,
                    FundingSource = FundingSourceType.Levy,
                    ApprenticeshipEmployerType = testType,
                }
            });

            actual.Should().BeEquivalentTo(
                new
                {
                    ApprenticeshipEmployerType = testType,
                }
            );
        }

        [Test, AutoData]
        public void RefundProducesRequiredPaymentsWithOriginalApprenticeshipId(
            Payment historicPayment, [Negative] decimal refundAmount, RefundService sut)
        {                           
            var actual = sut.GetRefund(refundAmount, new List<Payment>
            {
                historicPayment
            });

            actual.Should().BeEquivalentTo(new
            {
                historicPayment.ApprenticeshipId,
            });
        }

        [Test, AutoData]
        public void RefundProducesRequiredPaymentsWithOriginalApprenticeshipId_WhenItIsNull(
            Payment historicPayment, [Negative] decimal refundAmount, RefundService sut)
        {
            historicPayment.ApprenticeshipId = null;

            var actual = sut.GetRefund(refundAmount, new List<Payment>
            {
                historicPayment
            });

            actual.Should().BeEquivalentTo(new
            {
                ApprenticeshipId = (long?)null,
            });
        }

        [Test, AutoData]
        public void RefundProducesRequiredPaymentsWithOriginalApprenticeshipPriceEpisodeId(
            Payment historicPayment, [Negative] decimal refundAmount, RefundService sut)
        {
            var actual = sut.GetRefund(refundAmount, new List<Payment>
            {
                historicPayment
            });

            actual.Should().BeEquivalentTo(new
            {
                historicPayment.ApprenticeshipPriceEpisodeId,
            });
        }

        [Test, AutoData]
        public void RefundProducesRequiredPaymentsWithOriginalApprenticeshipPriceEpisodeId_WhenItIsNull(
            Payment historicPayment, [Negative] decimal refundAmount, RefundService sut)
        {
            historicPayment.ApprenticeshipPriceEpisodeId = null;

            var actual = sut.GetRefund(refundAmount, new List<Payment>
            {
                historicPayment
            });

            actual.Should().BeEquivalentTo(new
            {
                ApprenticeshipPriceEpisodeId = (long?)null,
            });
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
        public void MixedRefundMultipleSfaContributions()
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
        public void MixedRefundRoundsToFiveDecimalPlaces()
        {
            var sut = new RefundService();

            var testHistory = new List<Payment>
            {
                new Payment {Amount = 50, FundingSource = FundingSourceType.Levy, SfaContributionPercentage = 0.9m},
                new Payment {Amount = 90, FundingSource = FundingSourceType.CoInvestedSfa, SfaContributionPercentage = 0.9m},
                new Payment {Amount = 10, FundingSource = FundingSourceType.CoInvestedEmployer, SfaContributionPercentage = 0.9m},
            };

            var actual = sut.GetRefund(-100, testHistory);

            actual.Should().HaveCount(2);
            actual.Single(x => x.EarningType == EarningType.CoInvested).Amount.Should().Be(-66.66667m);
            actual.Single(x => x.EarningType == EarningType.Levy).Amount.Should().Be(-33.33333m);
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

        [Test]
        public void PriceEpisodeComesFromHistory()
        {
            var sut = new RefundService();

            var testHistory = new List<Payment>
            {
                new Payment {Amount = 100, FundingSource = FundingSourceType.FullyFundedSfa, SfaContributionPercentage = 0.9m, PriceEpisodeIdentifier = "1"},
                new Payment {Amount = 100, FundingSource = FundingSourceType.FullyFundedSfa, SfaContributionPercentage = 0.9m, PriceEpisodeIdentifier = "2"},
            };

            var actual = sut.GetRefund(-50, testHistory);

            actual.Should().HaveCount(2);
            actual[0].EarningType.Should().Be(EarningType.Incentive);
            actual[0].Amount.Should().Be(-25);
            actual[0].PriceEpisodeIdentifier.Should().Be("1");
            actual[1].Amount.Should().Be(-25);
            actual[1].PriceEpisodeIdentifier.Should().Be("2");
        }

        [Test]
        public void Refund_More_Than_Is_Available()
        {
            var sut = new RefundService();

            var testHistory = new List<Payment>
            {
                new Payment {Amount = 100, FundingSource = FundingSourceType.Levy, SfaContributionPercentage = 0.9m},
            };

            var actual = sut.GetRefund(-125, testHistory);

            actual.Should().HaveCount(1);
            actual[0].EarningType.Should().Be(EarningType.Levy);
            actual[0].Amount.Should().Be(-100);
            actual[0].SfaContributionPercentage.Should().Be(0.9m);
        }
    }
}
