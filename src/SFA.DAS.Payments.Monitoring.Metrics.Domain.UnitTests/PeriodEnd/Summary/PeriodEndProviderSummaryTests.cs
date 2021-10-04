using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.Submission.Summary;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.PeriodEnd.Summary
{
    [TestFixture]
    public class PeriodEndProviderSummaryTests
    {
        private List<TransactionTypeAmountsByContractType> defaultDcEarnings;
        private List<TransactionTypeAmountsByContractType> paymentTransactionTypes;
        private List<ProviderFundingSourceAmounts> paymentFundingSources;
        private ProviderContractTypeAmounts heldBackAmounts;
        private PeriodEndProviderDataLockTypeCounts periodEndProviderDataLockTypeCounts;
        protected PeriodEndProviderSummary GetPeriodEndProviderSummary => TestHelper.DefaultPeriodEndProviderSummary;

        [SetUp]
        public void SetUp()
        {
            defaultDcEarnings = TestHelper.GetDefaultDcEarnings;
            paymentTransactionTypes = TestHelper.GetPaymentTransactionTypes;
            paymentFundingSources = TestHelper.GetPaymentFundingSourceAmounts;
            heldBackAmounts = TestHelper.DefaultHeldBackCompletionPayments;
            periodEndProviderDataLockTypeCounts = TestHelper.DefaultPeriodEndProviderDataLockTypeCounts;

        }

        private PeriodEndProviderSummary GetPopulatedPeriodEndProviderSummary(decimal act1NegativeEarningsAmount = 0m, decimal act2NegativeEarningsAmount = 0m)
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
            summary.AddNegativeEarnings(TestHelper.GetDefaultDcNegativeEarnings(act1NegativeEarningsAmount, act2NegativeEarningsAmount));

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
            metrics.AlreadyPaidDataLockedEarnings.Should().Be(alreadyPaidDataLockedEarnings);
            metrics.TotalDataLockedEarnings.Should().Be(dataLockedEarningsTotal);
            metrics.AdjustedDataLockedEarnings.Should().Be(dataLockedEarningsTotal - alreadyPaidDataLockedEarnings);
        }

        [Test]
        public void Calculates_Correct_Metrics_For_DcEarnings()
        {
            var submissionSummary = GetPeriodEndProviderSummary;

            submissionSummary.AddDcEarnings(defaultDcEarnings);
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
            var metrics = GetSubmissionSummaryMetrics();
            var precision = 0.01m;
            metrics.PaymentMetrics.PercentageContractType1.Should().BeApproximately(92.45m, precision);
            metrics.PaymentMetrics.PercentageContractType2.Should().BeApproximately(87.08m, precision );
            metrics.PaymentMetrics.Percentage.Should().BeApproximately(89.79m, precision);
        }

        
        [Test]
        public void Calculates_Correct_Percentage()
        {
            var metrics = GetSubmissionSummaryMetrics();
            var precision = 0.01m;
            metrics.Percentage.Should().BeApproximately(89.79m, precision);
        }

        [Test]
        public void WhenGettingMetrics_AndThereAreNegativeEarnings_ThenNegativeEarningContractTypePropertiesArePopulated()
        {
            //Arrange
            var expectedAct1NegativeEarnings = 1000m;
            var expectedAct2NegativeEarnings = 800m;

            var summary = GetPopulatedPeriodEndProviderSummary(expectedAct1NegativeEarnings, expectedAct2NegativeEarnings);

            //Act
            var metrics = summary.GetMetrics();

            //Assert
            metrics.NegativeEarnings.ContractType1.Should().Be(expectedAct1NegativeEarnings);
            metrics.NegativeEarnings.ContractType2.Should().Be(expectedAct2NegativeEarnings);
        }

        [Test]
        public void WhenGettingMetrics_AndThereAreNoNegativeEarnings_ThenNegativeEarningContractTypeProptertiesAreZero()
        {
            //Arrange
            var summary = GetPopulatedPeriodEndProviderSummary();

            //Act
            var metrics = summary.GetMetrics();

            //Assert
            metrics.NegativeEarnings.ContractType1.Should().Be(0m);
            metrics.NegativeEarnings.ContractType2.Should().Be(0m);
        }

        [TestCase(1000.00, 700.00)]
        [TestCase(0.0, 0.0)]
        public void WhenGettingMetrics_ThenNegativeEarningsAreDeductedCorrectly(decimal act1NegativeEarningsAmount, decimal act2NegativeEarningsAmount)
        {
            //Arrange
            var dcEarningsAct1 = TestHelper.GetDefaultDcEarnings.Where(dc => dc.ContractType == ContractType.Act1).Sum(x => x.Total);
            var dcEarningsAct2 = TestHelper.GetDefaultDcEarnings.Where(dc => dc.ContractType == ContractType.Act2).Sum(x => x.Total);

            var expectedAct1DcEarningsTotal = dcEarningsAct1 - act1NegativeEarningsAmount;
            var expectedAct2DcEarningsTotal = dcEarningsAct2 - act2NegativeEarningsAmount;

            var summary = GetPopulatedPeriodEndProviderSummary(act1NegativeEarningsAmount, act2NegativeEarningsAmount);

            //Act
            var result = summary.GetMetrics();

            //Assert
            result.DcEarnings.ContractType1.Should().Be(expectedAct1DcEarningsTotal);
            result.DcEarnings.ContractType2.Should().Be(expectedAct2DcEarningsTotal);
        }


    }
}