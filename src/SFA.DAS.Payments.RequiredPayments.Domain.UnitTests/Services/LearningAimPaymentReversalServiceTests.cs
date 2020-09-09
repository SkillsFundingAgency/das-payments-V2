using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class LearningAimPaymentReversalServiceTests
    {
        private List<Payment> history;
        private RemovedLearningAimReversalReversalService sut;

        [SetUp]
        public void SetUp()
        {
            sut = new RemovedLearningAimReversalReversalService();
            history = new List<Payment>();
        }

        [Test]
        [TestCase(FundingSourceType.CoInvestedEmployer)]
        [TestCase(FundingSourceType.CoInvestedSfa)]
        [TestCase(FundingSourceType.FullyFundedSfa)]
        [TestCase(FundingSourceType.Levy)]
        [TestCase(FundingSourceType.Transfer)]
        public void Refunds_All_Delivery_Periods(FundingSourceType testFundingSource)
        {
            history.AddRange(Enumerable.Range(1, 12)
                .Select(period => new Payment
                {
                    DeliveryPeriod = (byte)period,
                    SfaContributionPercentage = .9M,
                    Amount = period * 10,
                    PriceEpisodeIdentifier = "pe-1",
                    FundingSource = testFundingSource,
                    Id = new Guid(),
                }));
            var requiredPayments = sut.RefundLearningAim(history);

            requiredPayments.Should().HaveCount(12);

            for (var i = 1; i <= 12; i++)
            {
                var period = i;
                requiredPayments.Should()
                    .ContainSingle(x => x.deliveryPeriod == period &&
                                        x.payment.Amount == -period * 10);
            }
        }

        [Test]
        public void Reverses_Payments_After_Learner_Paid_In_First_Collection_But_Removed_In_Next()
        {
            history.Add(new Payment
            {
                DeliveryPeriod = 1,
                Amount = 1000,
                TransactionType = 1,
                FundingSource = FundingSourceType.Levy,
                CollectionPeriod = new CollectionPeriod { Period = 1, AcademicYear = 2021 },
                SfaContributionPercentage = .95M,
                Ukprn = 1001234,
                LearnAimReference = "ZPROG001",
                LearnerReferenceNumber = "Learn-123",
                LearningAimFundingLineType = "funding-line1",
                PriceEpisodeIdentifier = "1-255-1/08/2020",
                Id = Guid.NewGuid(),
                ApprenticeshipId = 123456,
                ApprenticeshipPriceEpisodeId = 12,
            });
            var requiredPayments = sut.RefundLearningAim(history);

            requiredPayments.Should().NotBeNullOrEmpty();
            requiredPayments.Count.Should().Be(1);
            var requiredPayment = requiredPayments.FirstOrDefault();
            requiredPayment.payment.Amount.Should().Be(-1000);
        }

        [Test]
        public void Aggregates_Reversed_CoInvested_Payments()
        {
            history.Add(new Payment
            {
                DeliveryPeriod = 1,
                Amount = 50,
                TransactionType = 1,
                FundingSource = FundingSourceType.CoInvestedEmployer,
                CollectionPeriod = new CollectionPeriod { Period = 1, AcademicYear = 2021 },
                SfaContributionPercentage = .95M,
                Ukprn = 1001234,
                LearnAimReference = "ZPROG001",
                LearnerReferenceNumber = "Learn-123",
                LearningAimFundingLineType = "funding-line1",
                PriceEpisodeIdentifier = "1-255-1/08/2020",
                Id = Guid.NewGuid(),
                ApprenticeshipId = 123456,
                ApprenticeshipPriceEpisodeId = 12,
            });
            history.Add(new Payment
            {
                DeliveryPeriod = 1,
                Amount = 950,
                TransactionType = 1,
                FundingSource = FundingSourceType.CoInvestedSfa,
                CollectionPeriod = new CollectionPeriod { Period = 1, AcademicYear = 2021 },
                SfaContributionPercentage = .95M,
                Ukprn = 1001234,
                LearnAimReference = "ZPROG001",
                LearnerReferenceNumber = "Learn-123",
                LearningAimFundingLineType = "funding-line1",
                PriceEpisodeIdentifier = "1-255-1/08/2020",
                Id = Guid.NewGuid(),
                ApprenticeshipId = 123456,
                ApprenticeshipPriceEpisodeId = 12,
            });
            var requiredPayments = sut.RefundLearningAim(history);

            requiredPayments.Should().NotBeNullOrEmpty();
            requiredPayments.Count.Should().Be(1);
            var requiredPayment = requiredPayments.FirstOrDefault();
            requiredPayment.payment.Amount.Should().Be(-1000);
        }

        [Test]
        public void Handles_Multiple_Reversed_CoInvested_Payments()
        {
            history.Add(new Payment
            {
                DeliveryPeriod = 1,
                Amount = 50,
                TransactionType = 4,
                FundingSource = FundingSourceType.CoInvestedEmployer,
                CollectionPeriod = new CollectionPeriod { Period = 1, AcademicYear = 2021 },
                SfaContributionPercentage = .95M,
                Ukprn = 1001234,
                LearnAimReference = "ZPROG001",
                LearnerReferenceNumber = "Learn-123",
                LearningAimFundingLineType = "funding-line1",
                PriceEpisodeIdentifier = "1-255-1/08/2020",
                Id = Guid.NewGuid(),
                ApprenticeshipId = 123456,
                ApprenticeshipPriceEpisodeId = 12,
            });
            history.Add(new Payment
            {
                DeliveryPeriod = 1,
                Amount = 950,
                TransactionType = 4,
                FundingSource = FundingSourceType.CoInvestedSfa,
                CollectionPeriod = new CollectionPeriod { Period = 1, AcademicYear = 2021 },
                SfaContributionPercentage = .95M,
                Ukprn = 1001234,
                LearnAimReference = "ZPROG001",
                LearnerReferenceNumber = "Learn-123",
                LearningAimFundingLineType = "funding-line1",
                PriceEpisodeIdentifier = "1-255-1/08/2020",
                Id = Guid.NewGuid(),
                ApprenticeshipId = 123456,
                ApprenticeshipPriceEpisodeId = 12,
            });
            history.Add(new Payment
            {
                DeliveryPeriod = 1,
                Amount = 50,
                TransactionType = 5,
                FundingSource = FundingSourceType.CoInvestedEmployer,
                CollectionPeriod = new CollectionPeriod { Period = 1, AcademicYear = 2021 },
                SfaContributionPercentage = .95M,
                Ukprn = 1001234,
                LearnAimReference = "ZPROG001",
                LearnerReferenceNumber = "Learn-123",
                LearningAimFundingLineType = "funding-line1",
                PriceEpisodeIdentifier = "1-255-1/08/2020",
                Id = Guid.NewGuid(),
                ApprenticeshipId = 123456,
                ApprenticeshipPriceEpisodeId = 12,
            });
            history.Add(new Payment
            {
                DeliveryPeriod = 1,
                Amount = 950,
                TransactionType = 5,
                FundingSource = FundingSourceType.CoInvestedSfa,
                CollectionPeriod = new CollectionPeriod { Period = 1, AcademicYear = 2021 },
                SfaContributionPercentage = .95M,
                Ukprn = 1001234,
                LearnAimReference = "ZPROG001",
                LearnerReferenceNumber = "Learn-123",
                LearningAimFundingLineType = "funding-line1",
                PriceEpisodeIdentifier = "1-255-1/08/2020",
                Id = Guid.NewGuid(),
                ApprenticeshipId = 123456,
                ApprenticeshipPriceEpisodeId = 12,
            });
            var requiredPayments = sut.RefundLearningAim(history);

            requiredPayments.Should().NotBeNullOrEmpty();
            requiredPayments.Count.Should().Be(2);
            var requiredPayment = requiredPayments.FirstOrDefault();
            requiredPayment.payment.Amount.Should().Be(-1000);
            
            requiredPayment = requiredPayments.Skip(1).FirstOrDefault();
            requiredPayment.payment.Amount.Should().Be(-1000);
        }

        [Test]
        public void Reverses_Payments_After_Learner_Paid_In_First_Collection_But_Removed_In_Next_Added_In_Next_And_Removed_Again_With_Duplicate_Refund()
        {
            history.Add(new Payment
            {
                DeliveryPeriod = 1,
                Amount = 1000,
                TransactionType = 1,
                FundingSource = FundingSourceType.Levy,
                CollectionPeriod = new CollectionPeriod { Period = 1, AcademicYear = 2021 },
                SfaContributionPercentage = .95M,
                Ukprn = 1001234,
                LearnAimReference = "ZPROG001",
                LearnerReferenceNumber = "Learn-123",
                LearningAimFundingLineType = "funding-line1",
                PriceEpisodeIdentifier = "1-255-1/08/2020",
                Id = Guid.NewGuid(),
                ApprenticeshipId = 123456,
                ApprenticeshipPriceEpisodeId = 12,
            });

            history.Add(new Payment
            {
                DeliveryPeriod = 1,
                Amount = -1000,
                TransactionType = 1,
                FundingSource = FundingSourceType.Levy,
                CollectionPeriod = new CollectionPeriod { Period = 2, AcademicYear = 2021 },
                SfaContributionPercentage = .95M,
                Ukprn = 1001234,
                LearnAimReference = "ZPROG001",
                LearnerReferenceNumber = "Learn-123",
                LearningAimFundingLineType = "funding-line1",
                PriceEpisodeIdentifier = "1-255-1/08/2020",
                Id = Guid.NewGuid(),

                ApprenticeshipId = 123456,
                ApprenticeshipPriceEpisodeId = 12,
            });
            //Duplicate
            history.Add(new Payment
            {
                DeliveryPeriod = 1,
                Amount = -1000,
                TransactionType = 1,
                FundingSource = FundingSourceType.Levy,
                CollectionPeriod = new CollectionPeriod { Period = 2, AcademicYear = 2021 },
                SfaContributionPercentage = .95M,
                Ukprn = 1001234,
                LearnAimReference = "ZPROG001",
                LearnerReferenceNumber = "Learn-123",
                LearningAimFundingLineType = "funding-line1",
                PriceEpisodeIdentifier = "1-255-1/08/2020",
                Id = Guid.NewGuid(),

                ApprenticeshipId = 123456,
                ApprenticeshipPriceEpisodeId = 12,
            });
            history.Add(new Payment
            {
                DeliveryPeriod = 1,
                Amount = 2000,
                TransactionType = 1,
                FundingSource = FundingSourceType.Levy,
                CollectionPeriod = new CollectionPeriod { Period = 3, AcademicYear = 2021 },
                SfaContributionPercentage = .95M,
                Ukprn = 1001234,
                LearnAimReference = "ZPROG001",
                LearnerReferenceNumber = "Learn-123",
                LearningAimFundingLineType = "funding-line1",
                PriceEpisodeIdentifier = "1-255-1/08/2020",
                Id = Guid.NewGuid(),

                ApprenticeshipId = 123456,
                ApprenticeshipPriceEpisodeId = 12,
            });
            history.Add(new Payment
            {
                DeliveryPeriod = 2,
                Amount = 1000,
                TransactionType = 1,
                FundingSource = FundingSourceType.Levy,
                CollectionPeriod = new CollectionPeriod { Period = 3, AcademicYear = 2021 },
                SfaContributionPercentage = .95M,
                Ukprn = 1001234,
                LearnAimReference = "ZPROG001",
                LearnerReferenceNumber = "Learn-123",
                LearningAimFundingLineType = "funding-line1",
                PriceEpisodeIdentifier = "1-255-1/08/2020",
                Id = Guid.NewGuid(),

                ApprenticeshipId = 123456,
                ApprenticeshipPriceEpisodeId = 12,
            });
            history.Add(new Payment
            {
                DeliveryPeriod = 3,
                Amount = 1000,
                TransactionType = 1,
                FundingSource = FundingSourceType.Levy,
                CollectionPeriod = new CollectionPeriod { Period = 3, AcademicYear = 2021 },
                SfaContributionPercentage = .95M,
                Ukprn = 1001234,
                LearnAimReference = "ZPROG001",
                LearnerReferenceNumber = "Learn-123",
                LearningAimFundingLineType = "funding-line1",
                PriceEpisodeIdentifier = "1-255-1/08/2020",
                Id = Guid.NewGuid(),

                ApprenticeshipId = 123456,
                ApprenticeshipPriceEpisodeId = 12,
            });
            ExecuteAndAssertAllPeriodsAreNetZero();
        }

        private void ExecuteAndAssertAllPeriodsAreNetZero()
        {
            var reversals = sut.RefundLearningAim(history);
            var deliveryPeriods = history
                .GroupBy(h => h.DeliveryPeriod)
                .Select(grp => new { DeliveryPeriod = grp.Key, Amounts = grp.Select(p => p.Amount).ToList() })
                .ToList();
            foreach (var reversal in reversals)
            {
                var deliveryPeriod = deliveryPeriods.FirstOrDefault(dp => dp.DeliveryPeriod == reversal.deliveryPeriod);
                if (deliveryPeriod != null)
                    deliveryPeriod.Amounts.Add(reversal.payment.Amount);
                else
                {
                    deliveryPeriods.Add(new { DeliveryPeriod = reversal.deliveryPeriod, Amounts = new List<decimal> { reversal.payment.Amount } });
                }
            }

            deliveryPeriods.ForEach(deliveryPeriod => deliveryPeriod.Amounts.Sum().Should().Be(0, $"Delivery period: {deliveryPeriod.DeliveryPeriod} expected to be 0 but was {deliveryPeriod.Amounts.Sum()}"));
        }
    }
}