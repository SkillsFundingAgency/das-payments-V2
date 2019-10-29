using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class RequiredPaymentProcessorTests
    {
        protected RequiredPaymentProcessor sut;
        protected List<Payment> paymentHistory;
        protected decimal expectedAmount;
        protected Earning testEarning;
        
        [SetUp]
        public void Setup()
        {
            paymentHistory = new List<Payment>();

            testEarning = new Earning
            {
                Amount = 50,
                PriceEpisodeIdentifier = string.Empty,
                SfaContributionPercentage = 0,
                EarningType = EarningType.Levy,
            };

            sut = new RequiredPaymentProcessor(new PaymentDueProcessor(), new RefundService());
            expectedAmount = 50;
        }
        
        [TestFixture]
        public class WhenAmountIsMoreThanTotalAmountInHistory : RequiredPaymentProcessorTests
        {
            [Test]
            public void ThenRequiredPaymentHasCorrectAmount()
            {
                var actual = sut.GetRequiredPayments(testEarning, paymentHistory);

                actual.Single().Amount.Should().Be(expectedAmount);
            }

            [Test, AutoData]
            public void ThenRequiredPaymentHasSfaContributionPercentageOfInput(decimal expectedSfaContribution)
            {
                testEarning.SfaContributionPercentage = expectedSfaContribution;

                var actual = sut.GetRequiredPayments(testEarning, paymentHistory);

                actual.Single().SfaContributionPercentage.Should().Be(expectedSfaContribution);
            }

            [Test]
            [TestCase(EarningType.CoInvested)]
            [TestCase(EarningType.Levy)]
            [TestCase(EarningType.Incentive)]
            public void ThenRequiredPaymentHasEarningTypeOfInput(EarningType expectedEarningType)
            {
                testEarning.EarningType = expectedEarningType;

                var actual = sut.GetRequiredPayments(testEarning, paymentHistory);

                actual.Single().EarningType.Should().Be(expectedEarningType);
            }

            [Test]
            public void AndThereIsNoSfaContributionPercentage_ThenAnExceptionIsThrown()
            {
                testEarning.SfaContributionPercentage = null;

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
                testEarning.Amount = 0;
                
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
                testEarning.SfaContributionPercentage = null;
                testEarning.Amount = 0;

                paymentHistory.Add(new Payment { SfaContributionPercentage = .9M, Amount = 10, FundingSource = FundingSourceType.Levy});
                paymentHistory.Add(new Payment { SfaContributionPercentage = 1M, Amount = 20, FundingSource = FundingSourceType.Levy });
                paymentHistory.Add(new Payment { SfaContributionPercentage = .95M, Amount = 30, FundingSource = FundingSourceType.Levy });

                var actual = sut.GetRequiredPayments(testEarning, paymentHistory);

                actual.Sum(x => x.Amount).Should().Be(-60);
            }
        }

        [TestFixture]
        public class WhenHistoricPaymentsUsedADifferentSfaContribution : RequiredPaymentProcessorTests
        {
            [Test]
            public void ThenHistoricPaymentsWithDifferentSfaContributionShouldNotBeIncludedInRequiredAmountCalculation()
            {
                testEarning.Amount = 0;
                testEarning.SfaContributionPercentage = 0.9m;

                paymentHistory.Add(new Payment { SfaContributionPercentage = .9m, Amount = 10, FundingSource = FundingSourceType.Levy});
                paymentHistory.Add(new Payment { SfaContributionPercentage = 1m, Amount = 10, FundingSource = FundingSourceType.Levy });
                paymentHistory.Add(new Payment { SfaContributionPercentage = .95m, Amount = 10, FundingSource = FundingSourceType.Levy});
                
                var actual = sut.GetRequiredPayments(testEarning, paymentHistory);

                actual.Sum(x => x.Amount).Should().Be(-30);
            }

            [Test]
            public void ThenTheHistoricPaymentsWithADifferentSfaContributionPercentageShouldBeRefunded()
            {
                testEarning.Amount = 5;
                testEarning.SfaContributionPercentage = 0.9m;

                paymentHistory.Add(new Payment { SfaContributionPercentage = .9m, Amount = 5, FundingSource = FundingSourceType.Levy});
                paymentHistory.Add(new Payment { SfaContributionPercentage = 1m, Amount = 10, FundingSource = FundingSourceType.Levy });
                paymentHistory.Add(new Payment { SfaContributionPercentage = .95m, Amount = 20, FundingSource = FundingSourceType.Levy });
                
                var requiredPayments = sut.GetRequiredPayments(testEarning, paymentHistory);
                requiredPayments.Count.Should().Be(2);
                requiredPayments.All(rp => rp.SfaContributionPercentage != .9m).Should().BeTrue();
                requiredPayments.Select(rp => rp.Amount).Sum().Should().Be(-30m);
            }

            [Test]
            public void ThenShouldRefundHistoricPaymentsWithADifferentSfaContributionAndCreateRequiredPositivePayment()
            {
                testEarning.SfaContributionPercentage = 0.9m;
                testEarning.Amount = 45;

                expectedAmount = 45;
                paymentHistory.Add(new Payment { SfaContributionPercentage = .9m, Amount = 5, DeliveryPeriod = 1, FundingSource = FundingSourceType.Levy});
                paymentHistory.Add(new Payment { SfaContributionPercentage = 1m, Amount = 10, DeliveryPeriod = 1, FundingSource = FundingSourceType.Levy });
                paymentHistory.Add(new Payment { SfaContributionPercentage = .95m, Amount = 20, DeliveryPeriod = 1, FundingSource = FundingSourceType.Levy });
                
                var requiredPayments = sut.GetRequiredPayments(testEarning, paymentHistory);
                requiredPayments.Count.Should().Be(3);
                requiredPayments.Count(rp => rp.SfaContributionPercentage == .9M).Should().Be(1);
                requiredPayments.Count(rp => rp.SfaContributionPercentage == .95M).Should().Be(1);
                requiredPayments.Count(rp => rp.SfaContributionPercentage == 1M).Should().Be(1);
                requiredPayments.Where(x => x.Amount < 0).Sum(x => x.Amount).Should().Be(-30);
                requiredPayments.Where(x => x.Amount > 0).Sum(x => x.Amount).Should().Be(40);
            }

            [Test]
            public void ThenShouldRefundHistoricPaymentsWithADifferentSfaContributionAndRefundRequiredPayment()
            {
                testEarning.SfaContributionPercentage = 0.9m;
                testEarning.Amount = 0;

                paymentHistory.Add(new Payment { SfaContributionPercentage = .9m, Amount = 5, FundingSource = FundingSourceType.Levy});
                paymentHistory.Add(new Payment { SfaContributionPercentage = 1m, Amount = 10, FundingSource = FundingSourceType.Levy });
                paymentHistory.Add(new Payment { SfaContributionPercentage = .95m, Amount = 20, FundingSource = FundingSourceType.Levy });
                
                var requiredPayments = sut.GetRequiredPayments(testEarning, paymentHistory);
                requiredPayments.Count.Should().Be(3);
                requiredPayments.Count(rp => rp.SfaContributionPercentage == .9M).Should().Be(1);
                requiredPayments.Count(rp => rp.SfaContributionPercentage == .95M).Should().Be(1);
                requiredPayments.Count(rp => rp.SfaContributionPercentage == 1M).Should().Be(1);
                requiredPayments.Sum(rp => rp.Amount).Should().Be(-35);
            }
        }
    }
}
