using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests.Submission
{
    [TestFixture]
    public class SubmissionSummaryMetricsServiceTests
    {
        private SubmissionSummaryMetricsServiceFixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new SubmissionSummaryMetricsServiceFixture();
        }

        [Test]
        public async Task WhenGeneratingSubmissionSummaryMetrics_ThenGetsSubmissionSummaryMetricsFromRepository()
        {
            fixture.With_Returning_Mock_SubmissionSummary();

            await fixture.GenerateSubmissionsSummaryMetrics();

            fixture.Verify_GetSubmissionSummaryMetrics_IsCalled();
        }

        [Test]
        public async Task WhenGeneratingSubmissionSummaryMetrics_ThenGetsCollectionPeriodTolerance()
        {
            fixture.With_Returning_Mock_SubmissionSummary();

            await fixture.GenerateSubmissionsSummaryMetrics();

            fixture.Verify_GetCollectionPeriodTolerance_WasCalled();
        }

        [Test]
        public async Task WhenGeneratingSubmissionSummaryMetrics_ThenCalculatesIfSubmissionIsWithinTolerance()
        {
            fixture
                .With_Returning_Mock_SubmissionSummary()
                .With_SubmissionMetricsRepository_ReturningCollectionPeriodTolerance();

            await fixture.GenerateSubmissionsSummaryMetrics();

            fixture.Verify_CalculateIsWithinTolerance_WasCalled();
        }

        [Test]
        public async Task WhenGeneratingSubmissionSummaryMetrics_ThenSavesSubmissionSummaryMetrics()
        {
            fixture.With_Returning_Mock_SubmissionSummary();

            await fixture.GenerateSubmissionsSummaryMetrics();

            fixture.Verify_SaveSubmissionSummaryMetrics_IsCalled();
        }

        [Test]
        public void WhenGeneratingSubmissionSummaryMetrics_AndExceptionThrown_ThenWarningLogged()
        {
            fixture.With_SubmissionMetricsRepository_ThrowingException();

            Assert.ThrowsAsync<ArgumentNullException>(async () => { await fixture.GenerateSubmissionsSummaryMetrics(); });

            fixture.Verify_Logger_LogsWarningWithJobId();
        }

        [Test]
        public async Task WhenGeneratingSubmissionSummaryMetrics_ThenCorrectTelemetrySent()
        {
            fixture.With_Returning_Mock_SubmissionSummary();

            await fixture.GenerateSubmissionsSummaryMetrics();

            fixture.Verify_SubmissionsSummaryMetricsTelemetry_TrackEvent_IsCalled();
        }

        [Test]
        public async Task WhenGeneratingSubmissionSummaryMetrics_ThenMetricsReturned()
        {
            fixture.With_Returning_calculated_SubmissionSummary();

            var result = await fixture.GenerateSubmissionsSummaryMetrics();

            fixture.Assert_SubmissionsSummaryMetrics_AreReturned(result);
        }

        [Test]
        public async Task WhenGeneratingSubmissionSummaryMetrics_ThenInfoLogged()
        {
            fixture.With_Returning_Mock_SubmissionSummary();

            await fixture.GenerateSubmissionsSummaryMetrics();

            fixture.Verify_Logger_LogsInfoSubmissionsSummaryMetrics();
        }
    }

    internal class SubmissionSummaryMetricsServiceFixture
    {
        private readonly long jobId;
        private readonly short academicYear;
        private readonly byte collectionPeriod;
        private readonly Mock<IPaymentLogger> logger;
        private readonly Mock<ISubmissionMetricsRepository> submissionMetricsRepository;
        private readonly Mock<ISubmissionsSummary> submissionsSummary;
        private readonly Mock<ITelemetry> telemetry;
        private readonly SubmissionWindowValidationService sut;

        private readonly List<SubmissionSummaryModel> getSubmissionsSummaryMetricsResponse;
        private readonly SubmissionsSummaryModel getMetricsResponse;
        private readonly Mock<CollectionPeriodToleranceModel> getCollectionPeriodToleranceResponse;
        private readonly Dictionary<string, string> submissionsSummaryModelTelemetryProperties;

        public SubmissionSummaryMetricsServiceFixture()
        {
            var fixture = new Fixture();

            jobId = fixture.Create<long>();
            academicYear = fixture.Create<byte>();
            collectionPeriod = fixture.Create<byte>();
            logger = new Mock<IPaymentLogger>();
            submissionMetricsRepository = new Mock<ISubmissionMetricsRepository>();
            submissionsSummary = new Mock<ISubmissionsSummary>();
            telemetry = new Mock<ITelemetry>();
            sut = new SubmissionWindowValidationService(logger.Object, submissionMetricsRepository.Object, submissionsSummary.Object, telemetry.Object);

            getSubmissionsSummaryMetricsResponse = new List<SubmissionSummaryModel>
            {
                new SubmissionSummaryModel
                {
                    JobId = jobId,
                    AcademicYear = academicYear,
                    CollectionPeriod = collectionPeriod,
                    Percentage = 100,
                    SubmissionMetrics = new ContractTypeAmountsVerbose {ContractType1 = 100, ContractType2 = 100},
                    DcEarnings = new ContractTypeAmounts {ContractType1 = 200, ContractType2 = 200},
                    DasEarnings = new ContractTypeAmountsVerbose {ContractType1 = 100, ContractType2 = 100},
                    TotalDataLockedEarnings = 1,
                    AlreadyPaidDataLockedEarnings = 2,
                    AdjustedDataLockedEarnings = 0,
                    RequiredPayments = new ContractTypeAmounts {ContractType1 = 100, ContractType2 = 100},
                    HeldBackCompletionPayments = new ContractTypeAmounts {ContractType1 = 1, ContractType2 = 2},
                    YearToDatePayments = new ContractTypeAmounts {ContractType1 = 3, ContractType2 = 4}
                }
            };

            getMetricsResponse = new SubmissionsSummaryModel
            {
                JobId = jobId,
                AcademicYear = academicYear,
                CollectionPeriod = collectionPeriod,
                Percentage = 50,
                SubmissionMetrics = new ContractTypeAmountsVerbose { ContractType1 = 100, ContractType2 = 100, DifferenceContractType1 = -100, DifferenceContractType2 = -100, PercentageContractType1 = 50, PercentageContractType2 = 50, Percentage = 50 },
                DcEarnings = new ContractTypeAmounts { ContractType1 = 200, ContractType2 = 200 },
                DasEarnings = new ContractTypeAmountsVerbose { ContractType1 = 100, ContractType2 = 100, DifferenceContractType1 = -100, DifferenceContractType2 = -100, PercentageContractType1 = 50, PercentageContractType2 = 50, Percentage = 50 },
                TotalDataLockedEarnings = 1,
                AlreadyPaidDataLockedEarnings = 2,
                AdjustedDataLockedEarnings = 0,
                RequiredPayments = new ContractTypeAmounts { ContractType1 = 100, ContractType2 = 100 },
                HeldBackCompletionPayments = new ContractTypeAmounts { ContractType1 = 1, ContractType2 = 2 },
                YearToDatePayments = new ContractTypeAmounts { ContractType1 = 3, ContractType2 = 4 }
            };

            submissionsSummaryModelTelemetryProperties = new Dictionary<string, string>
            {
                { TelemetryKeys.JobId, jobId.ToString()},
                { TelemetryKeys.CollectionPeriod, collectionPeriod.ToString()},
                { TelemetryKeys.AcademicYear, academicYear.ToString()},
                { "IsWithinTolerance" , (getMetricsResponse.Percentage > 99.92m && getMetricsResponse.Percentage < 100.08m).ToString() },
            };

            getCollectionPeriodToleranceResponse = new Mock<CollectionPeriodToleranceModel>();
        }

        public Task<SubmissionsSummaryModel> GenerateSubmissionsSummaryMetrics() => sut.ValidateSubmissionWindow(jobId, academicYear, collectionPeriod, It.IsAny<CancellationToken>());

        public SubmissionSummaryMetricsServiceFixture With_Returning_Mock_SubmissionSummary()
        {
            submissionMetricsRepository
                .Setup(x => x.GetSubmissionsSummaryMetrics(jobId, academicYear, collectionPeriod,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(getSubmissionsSummaryMetricsResponse);

            submissionsSummary.Setup(x =>
                    x.GetMetrics(jobId, academicYear, collectionPeriod, getSubmissionsSummaryMetricsResponse))
                .Returns(getMetricsResponse);

            return this;
        }

        public SubmissionSummaryMetricsServiceFixture With_Returning_calculated_SubmissionSummary()
        {
            submissionMetricsRepository
                .Setup(x => x.GetSubmissionsSummaryMetrics(jobId, academicYear, collectionPeriod,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(getSubmissionsSummaryMetricsResponse);

            submissionsSummary.Setup(x =>
                    x.GetMetrics(jobId, academicYear, collectionPeriod, getSubmissionsSummaryMetricsResponse))
                .Returns(new SubmissionsSummary().GetMetrics(jobId, academicYear, collectionPeriod, getSubmissionsSummaryMetricsResponse));

            return this;
        }

        public SubmissionSummaryMetricsServiceFixture With_SubmissionMetricsRepository_ThrowingException()
        {
            submissionMetricsRepository
                .Setup(x => x.GetSubmissionsSummaryMetrics(jobId, academicYear, collectionPeriod,
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentNullException());

            return this;
        }

        public SubmissionSummaryMetricsServiceFixture With_SubmissionMetricsRepository_ReturningCollectionPeriodTolerance()
        {
            submissionMetricsRepository
                .Setup(x => x.GetCollectionPeriodTolerance(collectionPeriod, academicYear,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(getCollectionPeriodToleranceResponse.Object);
            
            return this;
        }

        public void Verify_GetSubmissionSummaryMetrics_IsCalled()
        {
            submissionMetricsRepository
                .Verify(x => x.GetSubmissionsSummaryMetrics(jobId, academicYear, collectionPeriod, It.IsAny<CancellationToken>()), Times.Once);
        }

        public void Verify_SaveSubmissionSummaryMetrics_IsCalled()
        {
            submissionMetricsRepository
                .Verify(x => x.SaveSubmissionsSummaryMetrics(getMetricsResponse, It.IsAny<CancellationToken>()), Times.Once);
        }

        public void Verify_Logger_LogsWarningWithJobId()
        {
            logger
                .Verify(x => x.LogWarning(It.IsRegex($".*{jobId}.*"), It.IsAny<object[]>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        public void Verify_Logger_LogsInfoSubmissionsSummaryMetrics()
        {
            logger
                .Verify(x => x.LogInfo(It.IsRegex($".*{jobId}.*{academicYear}.*{collectionPeriod}.*\\d*ms"), It.IsAny<object[]>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        public void Verify_GetCollectionPeriodTolerance_WasCalled()
        {
            submissionMetricsRepository
                .Verify(x => x.GetCollectionPeriodTolerance(collectionPeriod, academicYear, It.IsAny<CancellationToken>()), Times.Once);
        }

        public void Verify_CalculateIsWithinTolerance_WasCalled()
        {
            submissionsSummary
                .Verify(x => x.CalculateIsWithinTolerance(getCollectionPeriodToleranceResponse.Object.SubmissionToleranceLower, getCollectionPeriodToleranceResponse.Object.SubmissionToleranceUpper));
        }

        public void Verify_SubmissionsSummaryMetricsTelemetry_TrackEvent_IsCalled()
        {
            telemetry
                .Verify(x => x.TrackEvent("Finished Generating Submissions Summary Metrics",
                    submissionsSummaryModelTelemetryProperties, It.Is<Dictionary<string, double>>(y => 
                   y["Percentage"] == (double)getMetricsResponse.SubmissionMetrics.Percentage &&
                   y["ContractType1Percentage"] == (double)getMetricsResponse.SubmissionMetrics.PercentageContractType1 &&
                   y["ContractType2Percentage"] == (double)getMetricsResponse.SubmissionMetrics.PercentageContractType2 &&

                   y["DifferenceTotal"] == (double)getMetricsResponse.SubmissionMetrics.DifferenceTotal &&
                   y["DifferenceContractType1"] == (double)getMetricsResponse.SubmissionMetrics.DifferenceContractType1 &&
                   y["DifferenceContractType2"] == (double)getMetricsResponse.SubmissionMetrics.DifferenceContractType2 &&

                   y["ContractAmountTotal"] == (double)getMetricsResponse.SubmissionMetrics.Total &&
                   y["ContractType1Amount"] == (double)getMetricsResponse.SubmissionMetrics.ContractType1 &&
                   y["ContractType2Amount"] == (double)getMetricsResponse.SubmissionMetrics.ContractType2 &&

                   y["DasEarningsPercentage"] == (double)getMetricsResponse.DasEarnings.Percentage &&
                   y["DasEarningsPercentageContractType1"] == (double)getMetricsResponse.DasEarnings.PercentageContractType1 &&
                   y["DasEarningsPercentageContractType2"] == (double)getMetricsResponse.DasEarnings.PercentageContractType2 &&

                   y["DasEarningsDifferenceTotal"] == (double)getMetricsResponse.DasEarnings.DifferenceTotal &&
                   y["DasEarningsDifferenceContractType1"] == (double)getMetricsResponse.DasEarnings.DifferenceContractType1 &&
                   y["DasEarningsDifferenceContractType2"] == (double)getMetricsResponse.DasEarnings.DifferenceContractType2 &&

                   y["DasEarningsTotal"] == (double)getMetricsResponse.DasEarnings.Total &&
                   y["DasEarningsContractType1Total"] == (double)getMetricsResponse.DasEarnings.ContractType1 &&
                   y["DasEarningsContractType2Total"] == (double)getMetricsResponse.DasEarnings.ContractType2 &&

                   y["DcEarningsTotal"] == (double)getMetricsResponse.DcEarnings.Total &&
                   y["DcEarningsContractType1Total"] == (double)getMetricsResponse.DcEarnings.ContractType1 &&
                   y["DcEarningsContractType2Total"] == (double)getMetricsResponse.DcEarnings.ContractType2 &&

                   y["DataLockedEarnings"] == (double)getMetricsResponse.TotalDataLockedEarnings &&
                   y["DataLockedAlreadyPaidAmount"] == (double)getMetricsResponse.AlreadyPaidDataLockedEarnings &&
                   y["DataLockedAdjustedAmount"] == (double)getMetricsResponse.AdjustedDataLockedEarnings &&

                   y["HeldBackCompletionPaymentsTotal"] == (double)getMetricsResponse.HeldBackCompletionPayments.Total &&
                   y["HeldBackCompletionPaymentsContractType1"] == (double)getMetricsResponse.HeldBackCompletionPayments.ContractType1 &&
                   y["HeldBackCompletionPaymentsContractType2"] == (double)getMetricsResponse.HeldBackCompletionPayments.ContractType2 &&

                   y["RequiredPaymentsTotal"] == (double)getMetricsResponse.RequiredPayments.Total &&
                   y["RequiredPaymentsAct1Total"] == (double)getMetricsResponse.RequiredPayments.ContractType1 &&
                   y["RequiredPaymentsAc2Total"] == (double)getMetricsResponse.RequiredPayments.ContractType2 &&

                   y["YearToDatePaymentsTotal"] == (double)getMetricsResponse.YearToDatePayments.Total &&
                   y["YearToDatePaymentsContractType1Total"] == (double)getMetricsResponse.YearToDatePayments.ContractType1 &&
                   y["YearToDatePaymentsContractType2Total"] == (double)getMetricsResponse.YearToDatePayments.ContractType2 &&

                   y["RequiredPaymentsDasEarningsPercentageComparison"] == Math.Round(((double)(getMetricsResponse.YearToDatePayments.Total + getMetricsResponse.RequiredPayments.Total) / (double)getMetricsResponse.DasEarnings.Total) * 100, 2))));
        }

        public void Assert_SubmissionsSummaryMetrics_AreReturned(SubmissionsSummaryModel result)
        {
            result.Should().BeEquivalentTo(getMetricsResponse);
        }
    }
}