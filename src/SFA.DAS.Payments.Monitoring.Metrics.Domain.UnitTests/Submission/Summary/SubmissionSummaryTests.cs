using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.Submission.Summary
{
    [TestFixture]
    public class SubmissionSummaryTests
    {
        private List<TransactionTypeAmounts> dcEarnings;
        private List<TransactionTypeAmounts> dasEarnings;
        private List<TransactionTypeAmounts> requiredPayments;
        private DataLockTypeCounts dataLocks;
        private ContractTypeAmounts heldBackCompletionPayments;

        [SetUp]
        public void SetUp()
        {
            dcEarnings = TestsHelper.DefaultDcEarnings;
            dasEarnings = TestsHelper.DefaultDasEarnings;
            dataLocks = TestsHelper.DefaultDataLockedEarnings;
            requiredPayments = TestsHelper.DefaultRequiredPayments;
            heldBackCompletionPayments = TestsHelper.DefaultHeldBackCompletionPayments;
        }

        private SubmissionSummary GetPopulatedSubmissionSummary()
        {
            var summary = TestsHelper.DefaultSubmissionSummary;
            summary.AddEarnings(dcEarnings, dasEarnings);
            summary.AddDataLockTypeCounts(TestsHelper.DefaultDataLockedTotal, dataLocks, TestsHelper.AlreadyPaidDataLockedEarnings);
            summary.AddRequiredPayments(requiredPayments);
            summary.AddHeldBackCompletionPayments(heldBackCompletionPayments);
            summary.AddYearToDatePaymentTotals(TestsHelper.DefaultYearToDateAmounts);
            return summary;
        }

        protected SubmissionSummary GetSubmissionSummary => TestsHelper.DefaultSubmissionSummary;
        
        private SubmissionSummaryModel GetSubmissionSummaryMetrics()
        {
            var summary = GetPopulatedSubmissionSummary();
            return summary.GetMetrics();
        }

        [Test]
        public void Calculates_Correct_Year_To_Date_Payments_Totals()
        {
            var summary = TestsHelper.DefaultSubmissionSummary;
            var yearToDateAmounts = TestsHelper.DefaultYearToDateAmounts;
            summary.AddYearToDatePaymentTotals(yearToDateAmounts);
            var metrics = summary.GetMetrics();
            metrics.YearToDatePayments.ContractType1.Should().Be(yearToDateAmounts.ContractType1);
            metrics.YearToDatePayments.ContractType2.Should().Be(yearToDateAmounts.ContractType2);
            metrics.YearToDatePayments.Total.Should().Be(yearToDateAmounts.Total);
        }

        [Test]
        public void Calculates_Correct_DataLocked_Earnings_Totals()
        {
            var summary = GetSubmissionSummary;
            summary.AddDataLockTypeCounts(TestsHelper.DefaultDataLockedEarnings.Total, TestsHelper.DefaultDataLockedEarnings, TestsHelper.AlreadyPaidDataLockedEarnings);
            var metrics = summary.GetMetrics();
            metrics.AdjustedDataLockedEarnings.Should().Be(TestsHelper.DefaultDataLockedTotal - TestsHelper.AlreadyPaidDataLockedEarnings);
            metrics.DataLockMetrics.Count.Should().Be(1);
            metrics.DataLockMetrics.Sum(x => x.Amounts.Total).Should()
                .Be(TestsHelper.DefaultDataLockedEarnings.Total);
        }
        
        [Test]
        public void Calculates_Correct_Metrics_For_DcEarnings()
        {
            var submissionSummary = GetSubmissionSummary;
            submissionSummary.AddEarnings(dcEarnings, dasEarnings);
            var metrics = submissionSummary.GetMetrics();
            metrics.DcEarnings.ContractType1.Should().Be(32600);
            metrics.DcEarnings.ContractType2.Should().Be(32600);
            metrics.DcEarnings.Total.Should().Be(65200);
        }

        [Test]
        public void Calculates_Correct_Totals_For_DasEarnings()
        {
            var submissionSummary = GetSubmissionSummary;
            submissionSummary.AddEarnings(dcEarnings, dasEarnings);
            var metrics = submissionSummary.GetMetrics();
            metrics.DasEarnings.ContractType1.Should().Be(32600);
            metrics.DasEarnings.ContractType2.Should().Be(32600);
            metrics.DasEarnings.Total.Should().Be(65200);
        }

        [Test]
        public void Calculates_Correct_Percentage_For_DasEarnings()
        {
            var summary = TestsHelper.DefaultSubmissionSummary;
            var localDasEarnings = new List<TransactionTypeAmounts>
            {
                new TransactionTypeAmounts
                {
                    ContractType = ContractType.Act1,
                    TransactionType1 = 3000,
                },
            };
            var localDcEarnings = new List<TransactionTypeAmounts>
            {
                new TransactionTypeAmounts
                {
                    ContractType = ContractType.Act1,
                    TransactionType1 = 5000,
                },
            };
            summary.AddEarnings(localDcEarnings, localDasEarnings);
            
            var metrics = summary.GetMetrics();
            metrics.DasEarnings.Percentage.Should().Be(60);
        }
        

        [Test]
        public void Calculates_Correct_Differences_For_DasEarnings()
        {
            var submissionSummary = GetSubmissionSummary;
            submissionSummary.AddEarnings(dcEarnings, dasEarnings);
            var metrics = submissionSummary.GetMetrics();
            metrics.DasEarnings.DifferenceContractType1.Should().Be(0);
            metrics.DasEarnings.DifferenceContractType2.Should().Be(0);
        }

        [Test]
        public void Calculates_Correct_Percentages_For_DasEarnings_Matching_DcEarnings()
        {
            var submissionSummary = GetSubmissionSummary;
            submissionSummary.AddEarnings(dcEarnings, dasEarnings);
            var metrics = submissionSummary.GetMetrics();
            metrics.DasEarnings.PercentageContractType1.Should().Be(100);
            metrics.DasEarnings.PercentageContractType2.Should().Be(100);
        }

        [Test]
        public void Calculates_Correct_Percentages_For_DasEarnings_With_No_Dc_Earnings()
        {
            var submissionSummary = GetSubmissionSummary;
            dcEarnings.Clear();
            submissionSummary.AddEarnings(dcEarnings, dasEarnings);
            var metrics = submissionSummary.GetMetrics();
            metrics.DasEarnings.PercentageContractType1.Should().Be(0);
            metrics.DasEarnings.PercentageContractType2.Should().Be(0);
        }

        [Test]
        public void Does_Not_Include_Zero_Earnings_Contract_Types_In_Final_Percentage()
        {
            var submissionSummary = GetSubmissionSummary;
            dcEarnings.RemoveAll(x => x.ContractType == ContractType.Act2);
            dasEarnings.RemoveAll(x => x.ContractType == ContractType.Act2);
            var metrics = submissionSummary.GetMetrics();
            metrics.Percentage.Should().Be(100);
        }

        [Test]
        public void Calculates_Correct_Percentages_For_DasEarnings_With_Greater_Dc_Earnings()
        {
            var submissionSummary = GetSubmissionSummary;
            dasEarnings = new List<TransactionTypeAmounts>
            {
                new TransactionTypeAmounts
                {
                    ContractType = ContractType.Act1,
                    TransactionType1 = 6000,
                    TransactionType2 = 0,
                    TransactionType3 = 1500,
                    TransactionType4 = 50,
                    TransactionType5 = 50,
                    TransactionType6 = 50,
                    TransactionType7 = 50,
                    TransactionType8 = 50,
                    TransactionType9 = 50,
                    TransactionType10 = 50,
                    TransactionType11 = 50,
                    TransactionType12 = 50,
                    TransactionType13 = 50,
                    TransactionType14 = 50,
                    TransactionType15 = 50,
                    TransactionType16 = 50,
                },
                new TransactionTypeAmounts
                {
                    ContractType = ContractType.Act2,
                    TransactionType1 = 3000,
                    TransactionType2 = 0,
                    TransactionType3 = 750,
                    TransactionType4 = 25,
                    TransactionType5 = 25,
                    TransactionType6 = 25,
                    TransactionType7 = 25,
                    TransactionType8 = 25,
                    TransactionType9 = 25,
                    TransactionType10 = 25,
                    TransactionType11 = 25,
                    TransactionType12 = 25,
                    TransactionType13 = 25,
                    TransactionType14 = 25,
                    TransactionType15 = 25,
                    TransactionType16 = 25,
                }
            };
            submissionSummary.AddEarnings(dcEarnings, dasEarnings);
            var metrics = submissionSummary.GetMetrics();
            metrics.DasEarnings.PercentageContractType1.Should().Be(25);
            metrics.DasEarnings.PercentageContractType2.Should().Be(12.5M);
        }

        [Test]
        public void Calculates_Correct_HeldBack_Payments_Totals()
        {
            var summary = GetSubmissionSummary;
            var heldBackAmounts = TestsHelper.DefaultHeldBackCompletionPayments;
            summary.AddHeldBackCompletionPayments(heldBackAmounts);
            var metrics = summary.GetMetrics();
            metrics.HeldBackCompletionPayments.ContractType1.Should().Be(heldBackAmounts.ContractType1);
            metrics.HeldBackCompletionPayments.ContractType2.Should().Be(heldBackAmounts.ContractType2);
            metrics.HeldBackCompletionPayments.Total.Should().Be(heldBackAmounts.Total);
        }

        [Test]
        public void Calculates_Correct_Metrics_For_RequiredPayments()
        {
            var summary = GetSubmissionSummary;
            var requiredPayments = TestsHelper.DefaultRequiredPayments;
            summary.AddRequiredPayments(requiredPayments);
            var metrics = summary.GetMetrics();
            metrics.RequiredPaymentsMetrics.Count.Should().Be(2);
            metrics.RequiredPaymentsMetrics.FirstOrDefault(x => x.Amounts.ContractType == ContractType.Act1).Amounts
                .Total.Should().Be(requiredPayments.FirstOrDefault(x => x.ContractType == ContractType.Act1).Total);
            metrics.RequiredPaymentsMetrics.FirstOrDefault(x => x.Amounts.ContractType == ContractType.Act2).Amounts
                .Total.Should().Be(requiredPayments.FirstOrDefault(x => x.ContractType == ContractType.Act2).Total);
        }

        [Test]
        public void Calculates_Correct_Contract_Types_For_Required_Payments()
        {
            var summary = GetSubmissionSummary;
            var requiredPayments = TestsHelper.DefaultRequiredPayments;
            summary.AddRequiredPayments(requiredPayments);
            var metrics = summary.GetMetrics();
            metrics.RequiredPaymentsMetrics.Count.Should().Be(2);
            metrics.RequiredPayments.ContractType1.Should().Be(requiredPayments.FirstOrDefault(x => x.ContractType == ContractType.Act1).Total);
            metrics.RequiredPayments.ContractType2.Should().Be(requiredPayments.FirstOrDefault(x => x.ContractType == ContractType.Act2).Total);
        }
        
        [Test]
        public void Calculates_Correct_ContractTypes()
        {
            var metrics = GetSubmissionSummaryMetrics();
            metrics.SubmissionMetrics.ContractType1.Should().Be(
                requiredPayments.GetTotal(ContractType.Act1) + 
                TestsHelper.DefaultYearToDateAmounts.ContractType1 + 
                (TestsHelper.DefaultDataLockedTotal - TestsHelper.AlreadyPaidDataLockedEarnings) +
                heldBackCompletionPayments.ContractType1);
            metrics.SubmissionMetrics.ContractType2.Should().Be(
                requiredPayments.GetTotal(ContractType.Act2) +
                +TestsHelper.DefaultYearToDateAmounts.ContractType2 +
                heldBackCompletionPayments.ContractType2);
        }

        [Test]
        public void Calculates_Correct_ContractType_Percentages()
        {
            var metrics = GetSubmissionSummaryMetrics();
            metrics.SubmissionMetrics.PercentageContractType1.Should().Be(100);
            metrics.SubmissionMetrics.PercentageContractType2.Should().Be(100);
        }
        
        [Test]
        public void Calculates_Correct_Percentage()
        {
            var metrics = GetSubmissionSummaryMetrics();
            metrics.Percentage.Should().Be(100);
        }

        [Test]
        public void WhenPercentageIsNot100_Should_CalculateCorrectPercentage()
        {
            var summary = TestsHelper.DefaultSubmissionSummary;
            summary.AddRequiredPayments(new List<TransactionTypeAmounts>
            {
                new TransactionTypeAmounts
                {
                    ContractType = ContractType.Act1,
                    TransactionType1 = 1000,
                },
                new TransactionTypeAmounts
                {
                    ContractType = ContractType.Act2,
                    TransactionType1 = 2000,
                }
            });
            var earnings = new List<TransactionTypeAmounts>
            {
                new TransactionTypeAmounts
                {
                    ContractType = ContractType.Act1,
                    TransactionType1 = 2000,
                },
                new TransactionTypeAmounts
                {
                    ContractType = ContractType.Act2,
                    TransactionType1 = 3000,
                }
            };
            summary.AddEarnings(earnings, earnings);
            
            var metrics = summary.GetMetrics();
            metrics.Percentage.Should().Be(60);
        }
    }
}