using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.Submission.Summary;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.PeriodEnd.Summary
{
    [TestFixture]
    public class PeriodEndProviderSummaryTests
    {
        private Fixture fixture;
        private Random random;
        private List<TransactionTypeAmountsByContractType> defaultDcEarnings;
        private List<TransactionTypeAmountsByContractType> paymentTransactionTypes;
        private List<ProviderFundingSourceAmounts> paymentFundingSources;
        private List<ProviderNegativeEarningsLearnerDcEarningAmounts> providerLearnerNegativeEarnings;
        private List<ProviderNegativeEarningsLearnerContractTypeAmounts> providerLearnerPayments;
        private List<ProviderNegativeEarningsLearnerDataLockFundingLineTypeAmounts> providerLearnerDataLocks;
        private ProviderContractTypeAmounts heldBackAmounts;
        private PeriodEndProviderDataLockTypeCounts periodEndProviderDataLockTypeCounts;
        protected PeriodEndProviderSummary GetPeriodEndProviderSummary => TestHelper.DefaultPeriodEndProviderSummary;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            random = new Random();
            providerLearnerNegativeEarnings = CreateLearnerNegativeEarnings();
            providerLearnerPayments = TestHelper.DefaultLearnerPayments();
            providerLearnerDataLocks = TestHelper.DefaultLearnerDataLockedEarnings();
            defaultDcEarnings = TestHelper.GetDefaultDcEarnings;
            paymentTransactionTypes = TestHelper.GetPaymentTransactionTypes;
            paymentFundingSources = TestHelper.GetPaymentFundingSourceAmounts;
            heldBackAmounts = TestHelper.DefaultHeldBackCompletionPayments;
            periodEndProviderDataLockTypeCounts = TestHelper.DefaultPeriodEndProviderDataLockTypeCounts;
        }

        private PeriodEndProviderSummary GetPopulatedPeriodEndProviderSummary()
        {
            var summary = TestHelper.DefaultPeriodEndProviderSummary;
            summary.AddDcEarnings(defaultDcEarnings);
            var yearToDateAmounts = TestHelper.DefaultYearToDateAmounts;
            summary.AddPaymentsYearToDate(yearToDateAmounts);
            var alreadyPaidDataLockedEarnings = TestHelper.AlreadyPaidDataLockedEarnings;
            summary.AddDataLockedAlreadyPaid(alreadyPaidDataLockedEarnings);
            var dataLockedEarningsTotal = TestHelper.DefaultDataLockedTotal;
            summary.AddDataLockedEarnings(dataLockedEarningsTotal);
            summary.AddTransactionTypes(paymentTransactionTypes);
            summary.AddFundingSourceAmounts(paymentFundingSources);
            summary.AddHeldBackCompletionPayments(heldBackAmounts);
            summary.AddPeriodEndProviderDataLockTypeCounts(periodEndProviderDataLockTypeCounts);
            summary.AddLearnerNegativeEarnings(providerLearnerNegativeEarnings);
            summary.AddLearnerPayments(providerLearnerPayments);
            summary.AddLearnerDataLockedEarnings(providerLearnerDataLocks);

            return summary;
        }

        private ProviderPeriodEndSummaryModel GetSubmissionSummaryMetrics()
        {
            var summary = GetPopulatedPeriodEndProviderSummary();
            return summary.GetMetrics();
        }

        [Test]
        public void Calculates_Correct_Year_To_Date_Payments_Totals()
        {
            var summary = TestHelper.DefaultPeriodEndProviderSummary;
            var yearToDateAmounts = TestHelper.DefaultYearToDateAmounts;
            summary.AddPaymentsYearToDate(yearToDateAmounts);
            summary.AddDataLockedEarnings(TestHelper.DefaultDataLockedTotal);
            summary.AddDataLockedAlreadyPaid(TestHelper.AlreadyPaidDataLockedEarnings);
            var metrics = summary.GetMetrics();
            metrics.YearToDatePayments.ContractType1.Should().Be(yearToDateAmounts.ContractType1);
            metrics.YearToDatePayments.ContractType2.Should().Be(yearToDateAmounts.ContractType2);
            metrics.YearToDatePayments.Total.Should().Be(yearToDateAmounts.Total);
        }

        [Test]
        public void Calculates_Correct_DataLocked_Earnings_Totals()
        {
            var summary = GetPeriodEndProviderSummary;
            var alreadyPaidDataLockedEarnings = TestHelper.AlreadyPaidDataLockedEarnings;
            summary.AddDataLockedAlreadyPaid(alreadyPaidDataLockedEarnings);
            var dataLockedEarningsTotal = TestHelper.DefaultDataLockedTotal;
            summary.AddDataLockedEarnings(dataLockedEarningsTotal);

            var metrics = summary.GetMetrics();
            metrics.AlreadyPaidDataLockedEarnings.Should().Be(alreadyPaidDataLockedEarnings.Total);
            metrics.AlreadyPaidDataLockedEarnings16To18.Should().Be(alreadyPaidDataLockedEarnings.FundingLineType16To18Amount);
            metrics.AlreadyPaidDataLockedEarnings19Plus.Should().Be(alreadyPaidDataLockedEarnings.FundingLineType19PlusAmount);
            metrics.TotalDataLockedEarnings.Should().Be(dataLockedEarningsTotal.Total);
            metrics.TotalDataLockedEarnings16To18.Should().Be(dataLockedEarningsTotal.FundingLineType16To18Amount);
            metrics.TotalDataLockedEarnings19Plus.Should().Be(dataLockedEarningsTotal.FundingLineType19PlusAmount);
            metrics.AdjustedDataLockedEarnings.Should().Be(dataLockedEarningsTotal.Total - alreadyPaidDataLockedEarnings.Total);
            metrics.AdjustedDataLockedEarnings16To18.Should().Be(dataLockedEarningsTotal.FundingLineType16To18Amount - alreadyPaidDataLockedEarnings.FundingLineType16To18Amount);
            metrics.AdjustedDataLockedEarnings19Plus.Should().Be(dataLockedEarningsTotal.FundingLineType19PlusAmount - alreadyPaidDataLockedEarnings.FundingLineType19PlusAmount);
        }

        [Test]
        public void Calculates_Correct_Metrics_For_DcEarnings()
        {
            var submissionSummary = GetPeriodEndProviderSummary;

            submissionSummary.AddDcEarnings(defaultDcEarnings);
            submissionSummary.AddDataLockedEarnings(TestHelper.DefaultDataLockedTotal);
            submissionSummary.AddDataLockedAlreadyPaid(TestHelper.AlreadyPaidDataLockedEarnings);
            var metrics = submissionSummary.GetMetrics();
            metrics.DcEarnings.ContractType1.Should().Be(58300);
            metrics.DcEarnings.ContractType2.Should().Be(57300);
            metrics.DcEarnings.Total.Should().Be(115600);
        }

        [Test]
        public void Calculates_Correct_HeldBack_Payments_Totals()
        {
            var summary = GetPeriodEndProviderSummary;
            summary.AddHeldBackCompletionPayments(heldBackAmounts);
            summary.AddDataLockedEarnings(TestHelper.DefaultDataLockedTotal);
            summary.AddDataLockedAlreadyPaid(TestHelper.AlreadyPaidDataLockedEarnings);
            var metrics = summary.GetMetrics();
            metrics.HeldBackCompletionPayments.ContractType1.Should().Be(heldBackAmounts.ContractType1);
            metrics.HeldBackCompletionPayments.ContractType2.Should().Be(heldBackAmounts.ContractType2);
            metrics.HeldBackCompletionPayments.Total.Should().Be(heldBackAmounts.Total);
        }

        [Test]
        public void Calculates_Correct_Metrics_For_PaymentTransactionTypes()
        {
            var summary = GetPeriodEndProviderSummary;
            paymentTransactionTypes = TestHelper.GetPaymentTransactionTypes;
            summary.AddTransactionTypes(paymentTransactionTypes);
            summary.AddDataLockedEarnings(TestHelper.DefaultDataLockedTotal);
            summary.AddDataLockedAlreadyPaid(TestHelper.AlreadyPaidDataLockedEarnings);
            var metrics = summary.GetMetrics();
            metrics.TransactionTypeAmounts.Should().NotBeNull();
            metrics.TransactionTypeAmounts.Count.Should().Be(2);
            metrics.TransactionTypeAmounts
                .FirstOrDefault(x => x.TransactionTypeAmounts.ContractType == ContractType.Act1)?.TransactionTypeAmounts
                .Total.Should().Be(paymentTransactionTypes.FirstOrDefault(x => x.ContractType == ContractType.Act1)
                    ?.Total);
            metrics.TransactionTypeAmounts
                .FirstOrDefault(x => x.TransactionTypeAmounts.ContractType == ContractType.Act2)?.TransactionTypeAmounts
                .Total.Should().Be(paymentTransactionTypes.FirstOrDefault(x => x.ContractType == ContractType.Act2)
                    ?.Total);
        }

        [Test]
        public void Calculates_Correct_Metrics_For_PaymentFundingSource()
        {
            var summary = GetPeriodEndProviderSummary;
            paymentFundingSources = TestHelper.GetPaymentFundingSourceAmounts;
            summary.AddFundingSourceAmounts(paymentFundingSources);
            summary.AddDataLockedEarnings(TestHelper.DefaultDataLockedTotal);
            summary.AddDataLockedAlreadyPaid(TestHelper.AlreadyPaidDataLockedEarnings);
            var metrics = summary.GetMetrics();
            metrics.FundingSourceAmounts.Count.Should().Be(2);
            metrics.FundingSourceAmounts.FirstOrDefault(x => x.ContractType == ContractType.Act1)
                ?.Total.Should().Be(paymentFundingSources.FirstOrDefault(x => x.ContractType == ContractType.Act1)
                    ?.Total);
            metrics.FundingSourceAmounts.FirstOrDefault(x => x.ContractType == ContractType.Act2)
                ?.Total.Should().Be(paymentFundingSources.FirstOrDefault(x => x.ContractType == ContractType.Act2)
                    ?.Total);
        }

        [Test]
        public void Calculates_Correct_Metrics_For_PaymentTotals()
        {
            var summary = GetPeriodEndProviderSummary;
            var paymentsByTransactionType = TestHelper.GetPaymentTransactionTypes;
            summary.AddTransactionTypes(paymentsByTransactionType);
            summary.AddDataLockedEarnings(TestHelper.DefaultDataLockedTotal);
            summary.AddDataLockedAlreadyPaid(TestHelper.AlreadyPaidDataLockedEarnings);
            var metrics = summary.GetMetrics();
            metrics.Payments.ContractType1.Should().Be(32600m);
            metrics.Payments.ContractType2.Should().Be(32600m);
            metrics.Payments.Total.Should().Be(65200m);
        }

        [Test]
        public void Calculates_Correct_PaymentMetrics_ContractTypes()
        {
            var metrics = GetSubmissionSummaryMetrics();
            metrics.PaymentMetrics.ContractType1.Should().Be(
                paymentTransactionTypes.GetTotal(ContractType.Act1) +
                TestsHelper.DefaultYearToDateAmounts.ContractType1 +
                (TestsHelper.DefaultDataLockedTotal - TestsHelper.AlreadyPaidDataLockedEarnings) +
                heldBackAmounts.ContractType1);
            metrics.PaymentMetrics.ContractType2.Should().Be(
                paymentTransactionTypes.GetTotal(ContractType.Act2) +
                +TestsHelper.DefaultYearToDateAmounts.ContractType2 +
                heldBackAmounts.ContractType2);
        }

        [Test]
        public void Calculates_Correct_ContractType_Percentages()
        {
            providerLearnerNegativeEarnings.Clear();
            var metrics = GetSubmissionSummaryMetrics();
            var precision = 0.01m;
            metrics.PaymentMetrics.PercentageContractType1.Should().BeApproximately(92.45m, precision);
            metrics.PaymentMetrics.PercentageContractType2.Should().BeApproximately(87.08m, precision);
            metrics.PaymentMetrics.Percentage.Should().BeApproximately(89.79m, precision);
        }

        [Test]
        public void Calculates_Correct_Percentage()
        {
            providerLearnerNegativeEarnings.Clear();
            var metrics = GetSubmissionSummaryMetrics();
            var precision = 0.01m;
            metrics.Percentage.Should().BeApproximately(89.79m, precision);
        }

        [Test]
        public void WhenProviderLearnerNegativeEarningsEmpty_ThenNegativeEarningsNull()
        {
            //Arrange
            providerLearnerNegativeEarnings.Clear();

            //Act
            var result = GetSubmissionSummaryMetrics();

            //Assert
            result.NegativeEarnings.ContractType1.Should().BeNull();
            result.NegativeEarnings.ContractType2.Should().BeNull();
        }
        
        [Test]
        public void WhenProviderLearnerNegativeEarningsNull_ThenNegativeEarningsNull()
        {
            //Arrange
            providerLearnerNegativeEarnings = null;

            //Act
            var result = GetSubmissionSummaryMetrics();

            //Assert
            result.NegativeEarnings.ContractType1.Should().BeNull();
            result.NegativeEarnings.ContractType2.Should().BeNull();
        }
        
        [Test]
        public void WhenProviderHasSingleLearnerWithNegativeEarnings_AndLearnerHasPayments_ThenNegativeEarningsNull()
        {
            //Arrange
            providerLearnerNegativeEarnings.RemoveRange(1, providerLearnerNegativeEarnings.Count - 1);
            AddPaymentForLearner(providerLearnerNegativeEarnings.First().Uln);

            //Act
            var result = GetSubmissionSummaryMetrics();

            //Assert
            result.NegativeEarnings.ContractType1.Should().BeNull();
            result.NegativeEarnings.ContractType2.Should().BeNull();
        }
        
        [Test]
        public void WhenProviderHasSingleLearnerWithNegativeEarnings_AndLearnerHasNoPayments_AndLearnerHasDataLocks_ThenNegativeEarningsAdded()
        {
            //Arrange
            providerLearnerNegativeEarnings.RemoveRange(1, providerLearnerNegativeEarnings.Count - 1);
            AddDataLockForLearner(providerLearnerNegativeEarnings.First().Uln);

            var expectedAct1NegativeEarningsTotal = providerLearnerNegativeEarnings.Where(x => x.ContractType == ContractType.Act1).Sum(x => x.NegativeEarningsTotal);
            var expectedAct2NegativeEarningsTotal = providerLearnerNegativeEarnings.Where(x => x.ContractType == ContractType.Act2).Sum(x => x.NegativeEarningsTotal);

            //Act
            var result = GetSubmissionSummaryMetrics();

            //Assert
            if (expectedAct1NegativeEarningsTotal == 0m)
            {
                result.NegativeEarnings.ContractType1.Should().BeNull();
                result.NegativeEarnings.ContractType2.Should().Be(expectedAct2NegativeEarningsTotal);
            }
            else
            {
                result.NegativeEarnings.ContractType1.Should().Be(expectedAct1NegativeEarningsTotal);
                result.NegativeEarnings.ContractType2.Should().BeNull();
            }
        }
        
        [Test]
        public void WhenProviderHasSingleLearnerWithNegativeEarnings_AndLearnerHasNoPayments_AndLearnerHasDataLocks_ThenNegativeDataLocksAdjustedCorrectly()
        {
            //Arrange
            providerLearnerNegativeEarnings.RemoveRange(1, providerLearnerNegativeEarnings.Count - 1);
            AddDataLockForLearner(providerLearnerNegativeEarnings.First().Uln);

            var expectedNegativeDataLocksTotal = TestHelper.DefaultDataLockedTotal.Total - providerLearnerDataLocks.Sum(x => x.Total);
            var expectedNegativeDataLocks16To18Total = TestHelper.DefaultDataLockedTotal.FundingLineType16To18Amount - providerLearnerDataLocks.Sum(x => x.FundingLineType16To18Amount);
            var expectedNegativeDataLocks19PlusTotal = TestHelper.DefaultDataLockedTotal.FundingLineType19PlusAmount - providerLearnerDataLocks.Sum(x => x.FundingLineType19PlusAmount);

            //Act
            var result = GetSubmissionSummaryMetrics();

            //Assert
            result.TotalDataLockedEarnings.Should().Be(expectedNegativeDataLocksTotal);
            result.TotalDataLockedEarnings16To18.Should().Be(expectedNegativeDataLocks16To18Total);
            result.TotalDataLockedEarnings19Plus.Should().Be(expectedNegativeDataLocks19PlusTotal);
        }
        
        [Test]
        public void WhenProviderHasSingleLearnerWithNegativeEarnings_AndLearnerHasNoPayments_AndLearnerNoDataLocks_ThenNegativeEarningsCorrectlyCalculated()
        {
            //Arrange
            providerLearnerNegativeEarnings.RemoveRange(1, providerLearnerNegativeEarnings.Count - 1);

            var expectedAct1NegativeEarningsTotal = providerLearnerNegativeEarnings.Where(x => x.ContractType == ContractType.Act1).Sum(x => x.NegativeEarningsTotal);
            var expectedAct2NegativeEarningsTotal = providerLearnerNegativeEarnings.Where(x => x.ContractType == ContractType.Act2).Sum(x => x.NegativeEarningsTotal);

            //Act
            var result = GetSubmissionSummaryMetrics();

            //Assert
            if (expectedAct1NegativeEarningsTotal == 0m)
            {
                result.NegativeEarnings.ContractType1.Should().BeNull();
                result.NegativeEarnings.ContractType2.Should().Be(expectedAct2NegativeEarningsTotal);
            }
            else
            {
                result.NegativeEarnings.ContractType1.Should().Be(expectedAct1NegativeEarningsTotal);
                result.NegativeEarnings.ContractType2.Should().BeNull();
            }
        }
        
        [Test]
        public void WhenProviderHasMultipleLearnersWithNegativeEarnings_AndOnlyOneHasNoPaymentsAndNoDataLocks_ThenNegativeEarningsCorrectlyCalculated()
        {
            //Arrange
            var ulnUnderTest = providerLearnerNegativeEarnings.First().Uln;
            var remainingUlns = providerLearnerNegativeEarnings.Where(x => x.Uln != ulnUnderTest).ToList();

            remainingUlns.ForEach(x =>
            {
                AddDataLockForLearner(x.Uln);
                AddPaymentForLearner(x.Uln);
            });

            var expectedAct1NegativeEarningsTotal = providerLearnerNegativeEarnings.Where(x => x.ContractType == ContractType.Act1 && x.Uln == ulnUnderTest).Sum(x => x.NegativeEarningsTotal);
            var expectedAct2NegativeEarningsTotal = providerLearnerNegativeEarnings.Where(x => x.ContractType == ContractType.Act2 && x.Uln == ulnUnderTest).Sum(x => x.NegativeEarningsTotal);


            //Act
            var result = GetSubmissionSummaryMetrics();

            //Assert
            if (expectedAct1NegativeEarningsTotal == 0m)
            {
                result.NegativeEarnings.ContractType1.Should().BeNull();
                result.NegativeEarnings.ContractType2.Should().Be(expectedAct2NegativeEarningsTotal);
            }
            else
            {
                result.NegativeEarnings.ContractType1.Should().Be(expectedAct1NegativeEarningsTotal);
                result.NegativeEarnings.ContractType2.Should().BeNull();
            }
        }
        
        [Test]
        public void WhenProviderHasMultipleLearnersWithNegativeEarnings_AndManyLearnersHaveNoPaymentsAndNoDataLocks_ThenNegativeEarningsCorrectlyCalculated()
        {
            //Arrange
            var ulnsUnderTest = providerLearnerNegativeEarnings.Take(2).Select(x => x.Uln);
            var remainingUlns = providerLearnerNegativeEarnings.Where(x => !ulnsUnderTest.Contains(x.Uln)).ToList();

            remainingUlns.ForEach(x =>
            {
                AddDataLockForLearner(x.Uln);
                AddPaymentForLearner(x.Uln);
            });

            var expectedAct1NegativeEarningsTotal = providerLearnerNegativeEarnings.Where(x => x.ContractType == ContractType.Act1 && ulnsUnderTest.Contains(x.Uln)).Sum(x => x.NegativeEarningsTotal);
            var expectedAct2NegativeEarningsTotal = providerLearnerNegativeEarnings.Where(x => x.ContractType == ContractType.Act2 && ulnsUnderTest.Contains(x.Uln)).Sum(x => x.NegativeEarningsTotal);

            //Act
            var result = GetSubmissionSummaryMetrics();

            //Assert
            if (expectedAct1NegativeEarningsTotal == 0m)
            {
                result.NegativeEarnings.ContractType1.Should().BeNull();
                result.NegativeEarnings.ContractType2.Should().Be(expectedAct2NegativeEarningsTotal);
            }
            else if (expectedAct2NegativeEarningsTotal == 0m)
            {
                result.NegativeEarnings.ContractType1.Should().Be(expectedAct1NegativeEarningsTotal);
                result.NegativeEarnings.ContractType2.Should().BeNull();
            }
            else
            {
                result.NegativeEarnings.ContractType1.Should().Be(expectedAct1NegativeEarningsTotal);
                result.NegativeEarnings.ContractType2.Should().Be(expectedAct2NegativeEarningsTotal);
            }
        }
        
        private void AddDataLockForLearner(long uln)
        {
            var dataLock = fixture.Create<ProviderNegativeEarningsLearnerDataLockFundingLineTypeAmounts>();
            dataLock.LearnerUln = uln;
            dataLock.Ukprn = TestHelper.DefaultPeriodEndProviderSummary.Ukprn;
            providerLearnerDataLocks.Add(dataLock);
        }

        private void AddPaymentForLearner(long uln)
        {
            var payment = fixture.Create<ProviderNegativeEarningsLearnerContractTypeAmounts>();
            payment.LearnerUln = uln;
            payment.Ukprn = TestHelper.DefaultPeriodEndProviderSummary.Ukprn;
            providerLearnerPayments.Add(payment);
        }

        private List<ProviderNegativeEarningsLearnerDcEarningAmounts> CreateLearnerNegativeEarnings()
        {
            var providerLearnerNegativeEarnings = fixture.CreateMany<ProviderNegativeEarningsLearnerDcEarningAmounts>(5).ToList();
            providerLearnerNegativeEarnings.ForEach(x =>
            {
                x.Ukprn = TestHelper.DefaultPeriodEndProviderSummary.Ukprn;
                x.ContractType = random.Next(0, 2) == 1 ? ContractType.Act1 : ContractType.Act2;
                x.NegativeEarningsTotal = random.Next(1, 100000);
            });
            
            return providerLearnerNegativeEarnings;
        }
    }
}