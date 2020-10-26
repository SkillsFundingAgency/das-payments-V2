using System;
using AutoFixture;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.Submission.Summary
{
    [TestFixture]
    public class SubmissionsSummaryTests
    {
        private SubmissionsSummaryFixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new SubmissionsSummaryFixture();
        }

        [Test]
        public void WhenCalculatingIsWithinTolerance_AndSubmissionsSummaryModelIsNull_ThenNoExceptionThrown()
        {
            Assert.DoesNotThrow(() => fixture.CalculateIsWithinTolerance(99.99m, 100.01m));
        }

        [TestCase(99.85, 99.83, 100.19, true)] // Submission percentage within configured tolerance range
        [TestCase(99.82, 99.83, 100.19, false)] // Submission percentage below lower configured tolerance range
        [TestCase(99.83, 99.83, 100.19, true)] // Submission percentage equal to lower configured tolerance range
        [TestCase(100.20, 99.83, 100.19, false)] // Submission percentage above upper configured tolerance range
        [TestCase(100.19, 99.83, 100.19, true)] // Submission percentage equal to upper configured tolerance range
        [TestCase(99.93, null, 100.19, true)] // Submission percentage within lower hardcoded tolerance range
        [TestCase(99.91, null, 100.19, false)] // Submission percentage below lower hardcoded tolerance range
        [TestCase(99.92, null, 100.19, true)] // Submission percentage equal to lower hardcoded tolerance range
        [TestCase(100.07, 99.83, null, true)] // Submission percentage within upper hardcoded tolerance range
        [TestCase(100.10, 99.83, null, false)] // Submission percentage above upper hardcoded tolerance range
        [TestCase(100.08, 99.83, null, true)] // Submission percentage equal to upper hardcoded tolerance range
        public void WhenCalculatingIsWithinTolerance_ThenCorrectlyCalculatesSubmissionTolerance(decimal actualSubmissionPercentage, decimal? lowerTolerance, decimal? upperTolerance, bool expectedIsWithinTolerance)
        {
            fixture
                .With_SubmissionSummaryModels_HavingPercentage(actualSubmissionPercentage)
                .With_GetMetricsCalled();

            fixture.CalculateIsWithinTolerance(lowerTolerance, upperTolerance);

            fixture.Assert_IsWithinToleranceSetTo(expectedIsWithinTolerance);
        }
    }

    internal class SubmissionsSummaryFixture
    {
        private readonly long jobId;
        private readonly short academicYear;
        private readonly byte currentCollectionPeriod;
        private readonly SubmissionsSummary sut;

        private readonly List<SubmissionSummaryModel> submissionSummaryModels;
        private SubmissionsSummaryModel getMetricsSubmissionsSummaryResult;

        public SubmissionsSummaryFixture()
        {
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            jobId = fixture.Create<long>();
            academicYear = fixture.Create<short>();
            currentCollectionPeriod = fixture.Create<byte>();
            sut = new SubmissionsSummary();

            submissionSummaryModels = fixture.Create<List<SubmissionSummaryModel>>();
        }

        public void CalculateIsWithinTolerance(decimal? lowerTolerance, decimal? upperTolerance) => sut.CalculateIsWithinTolerance(lowerTolerance, upperTolerance);

        public SubmissionsSummaryFixture With_GetMetricsCalled()
        {
            getMetricsSubmissionsSummaryResult = sut.GetMetrics(jobId, academicYear, currentCollectionPeriod, submissionSummaryModels);
            
            return this;
        }

        public SubmissionsSummaryFixture With_SubmissionSummaryModels_HavingPercentage(decimal percentage)
        {
            submissionSummaryModels.ForEach(x =>
            {
                x.DcEarnings.ContractType1 = 10_000_000;
                x.DcEarnings.ContractType2 = 10_000_000;
            });

            var modelCount = submissionSummaryModels.Count;
            var dcTotal = submissionSummaryModels.Sum(s => s.DcEarnings.ContractType1) + submissionSummaryModels.Sum(s => s.DcEarnings.ContractType2);
            var submissionAmountRequiredPerModel = ((percentage / 100) * dcTotal) / modelCount;

            submissionSummaryModels.ForEach(x =>
                {
                    x.SubmissionMetrics.ContractType1 = Math.Round(submissionAmountRequiredPerModel / 2);
                    x.SubmissionMetrics.ContractType2 = Math.Round(submissionAmountRequiredPerModel / 2);
                });
            
            return this;
        }

        public void Assert_IsWithinToleranceSetTo(bool isWithinTolerance)
        {
            getMetricsSubmissionsSummaryResult.IsWithinTolerance.Should().Be(isWithinTolerance);
        }
    }
}