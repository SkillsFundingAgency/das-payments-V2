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

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests.Submission
{
    [TestFixture]
    public class SubmissionSummaryMetricsServiceTests
    {
        private SubmissionSummaryMetricsServiceFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new SubmissionSummaryMetricsServiceFixture();
        }

        [Test]
        public async Task WhenGeneratingSubmissionSummaryMetrics_ThenGetsSubmissionSummaryMetricsFromRepository()
        {
            _fixture.With_SubmissionMetricsRepository_ReturningSummary();

            await _fixture.GenerateSubmissionsSummaryMetrics();

            _fixture.Verify_GetSubmissionSummaryMetrics_IsCalled();
        }

        [Test]
        public async Task WhenGeneratingSubmissionSummaryMetrics_ThenSavesSubmissionSummaryMetrics()
        {
            _fixture.With_SubmissionMetricsRepository_ReturningSummary();

            await _fixture.GenerateSubmissionsSummaryMetrics();

            _fixture.Verify_SaveSubmissionSummaryMetrics_IsCalled();
        }

        [Test]
        public void WhenGeneratingSubmissionSummaryMetrics_AndExceptionThrown_ThenWarningLogged()
        {
            _fixture.With_SubmissionMetricsRepository_ThrowingException();

            Assert.ThrowsAsync<ArgumentNullException>(async () => { await _fixture.GenerateSubmissionsSummaryMetrics(); });

            _fixture.Verify_Logger_LogsWarningWithJobId();
        }

        [Test]
        public async Task WhenGeneratingSubmissionSummaryMetrics_ThenCorrectTelemetrySent()
        {
            _fixture.With_SubmissionMetricsRepository_ReturningSummary();

            await _fixture.GenerateSubmissionsSummaryMetrics();

            _fixture.Verify_SubmissionsSummaryMetricsTelemetry_TrackEvent_IsCalled();
        }

        [Test]
        public async Task WhenGeneratingSubmissionSummaryMetrics_ThenMetricsReturned()
        {
            _fixture.With_SubmissionMetricsRepository_ReturningSummary();

            var result = await _fixture.GenerateSubmissionsSummaryMetrics();

            _fixture.Assert_SubmissionsSummaryMetrics_AreReturned(result);
        }

        [Test]
        public async Task WhenGeneratingSubmissionSummaryMetrics_ThenInfoLogged()
        {
            _fixture.With_SubmissionMetricsRepository_ReturningSummary();

            await _fixture.GenerateSubmissionsSummaryMetrics();

            _fixture.Verify_Logger_LogsInfoSubmissionsSummaryMetrics();
        }
    }

    internal class SubmissionSummaryMetricsServiceFixture
    {
        private readonly Fixture _fixture;
        private readonly long _jobId;
        private readonly short _academicYear;
        private readonly byte _collectionPeriod;
        private readonly Mock<IPaymentLogger> _logger;
        private readonly Mock<ISubmissionMetricsRepository> _submissionMetricsRepository;
        private readonly Mock<ITelemetry> _telemetry;
        private readonly SubmissionsSummaryMetricsService _sut;

        private readonly SubmissionsSummaryModel _getSubmissionsSummaryMetricsResponse;
        private readonly Dictionary<string, string> _submissionsSummaryModelTelemetryProperties;

        public SubmissionSummaryMetricsServiceFixture()
        {
            _fixture = new Fixture();

            _jobId = _fixture.Create<long>();
            _academicYear = _fixture.Create<byte>();
            _collectionPeriod = _fixture.Create<byte>();
            _logger = new Mock<IPaymentLogger>();
            _submissionMetricsRepository = new Mock<ISubmissionMetricsRepository>();
            _telemetry = new Mock<ITelemetry>();
            _sut = new SubmissionsSummaryMetricsService(_logger.Object, _submissionMetricsRepository.Object, _telemetry.Object);

            _getSubmissionsSummaryMetricsResponse = _fixture.Create<SubmissionsSummaryModel>();
            _submissionsSummaryModelTelemetryProperties = new Dictionary<string, string>
            {
                { TelemetryKeys.JobId, _getSubmissionsSummaryMetricsResponse.JobId.ToString()},
                { TelemetryKeys.CollectionPeriod, _getSubmissionsSummaryMetricsResponse.CollectionPeriod.ToString()},
                { TelemetryKeys.AcademicYear, _getSubmissionsSummaryMetricsResponse.AcademicYear.ToString()},
                { "IsWithinTolerance" , (_getSubmissionsSummaryMetricsResponse.Percentage > 99.92m && _getSubmissionsSummaryMetricsResponse.Percentage < 100.08m).ToString() },
            };
        }

        public Task<SubmissionsSummaryModel> GenerateSubmissionsSummaryMetrics() => _sut.GenrateSubmissionsSummaryMetrics(_jobId, _academicYear, _collectionPeriod, It.IsAny<CancellationToken>());

        public SubmissionSummaryMetricsServiceFixture With_SubmissionMetricsRepository_ReturningSummary()
        {
            _submissionMetricsRepository
                .Setup(x => x.GetSubmissionsSummaryMetrics(_jobId, _academicYear, _collectionPeriod,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_getSubmissionsSummaryMetricsResponse);

            return this;
        }

        public SubmissionSummaryMetricsServiceFixture With_SubmissionMetricsRepository_ThrowingException()
        {
            _submissionMetricsRepository
                .Setup(x => x.GetSubmissionsSummaryMetrics(_jobId, _academicYear, _collectionPeriod,
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentNullException());

            return this;
        }

        public void Verify_GetSubmissionSummaryMetrics_IsCalled()
        {
            _submissionMetricsRepository
                .Verify(x => x.GetSubmissionsSummaryMetrics(_jobId, _academicYear, _collectionPeriod, It.IsAny<CancellationToken>()), Times.Once);
        }

        public void Verify_SaveSubmissionSummaryMetrics_IsCalled()
        {
            _submissionMetricsRepository
                .Verify(x => x.SaveSubmissionsSummaryMetrics(_getSubmissionsSummaryMetricsResponse, It.IsAny<CancellationToken>()), Times.Once);
        }

        public void Verify_Logger_LogsWarningWithJobId()
        {
            _logger
                .Verify(x => x.LogWarning(It.IsRegex($".*{_jobId}.*"), It.IsAny<object[]>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        public void Verify_Logger_LogsInfoSubmissionsSummaryMetrics()
        {
            _logger
                .Verify(x => x.LogInfo(It.IsRegex($".*{_jobId}.*{_academicYear}.*{_collectionPeriod}.*\\d*ms"), It.IsAny<object[]>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        public void Verify_SubmissionsSummaryMetricsTelemetry_TrackEvent_IsCalled()
        {
            _telemetry
                .Verify(x => x.TrackEvent("Finished Generating Submissions Summary Metrics",
                    _submissionsSummaryModelTelemetryProperties, It.Is<Dictionary<string, double>>(y => 
                        y["Percentage"] == (double) _getSubmissionsSummaryMetricsResponse.SubmissionMetrics.Percentage &&
                        y["ContractType1Percentage"] == (double) _getSubmissionsSummaryMetricsResponse.SubmissionMetrics.PercentageContractType1 &&
                        y["ContractType2Percentage"] == (double) _getSubmissionsSummaryMetricsResponse.SubmissionMetrics.PercentageContractType2 &&
                        
                        y["DifferenceTotal"] == (double) _getSubmissionsSummaryMetricsResponse.SubmissionMetrics.DifferenceTotal &&
                        y["DifferenceContractType1"] == (double) _getSubmissionsSummaryMetricsResponse.SubmissionMetrics.DifferenceContractType1 &&
                        y[ "DifferenceContractType2"] == (double) _getSubmissionsSummaryMetricsResponse.SubmissionMetrics.DifferenceContractType2 &&
                
                        y["DasEarningsPercentage"] == (double) _getSubmissionsSummaryMetricsResponse.DasEarnings.Percentage &&
                        y["DasEarningsPercentageContractType1"] == (double) _getSubmissionsSummaryMetricsResponse.DasEarnings.PercentageContractType1 &&
                        y["DasEarningsPercentageContractType2"] == (double) _getSubmissionsSummaryMetricsResponse.DasEarnings.PercentageContractType2 &&
                        
                        y["DasEarningsDifferenceTotal"] == (double) _getSubmissionsSummaryMetricsResponse.DasEarnings.DifferenceTotal &&
                        y["DasEarningsDifferenceContractType1"] == (double) _getSubmissionsSummaryMetricsResponse.DasEarnings.DifferenceContractType1 &&
                        y["DasEarningsDifferenceContractType2"] == (double) _getSubmissionsSummaryMetricsResponse.DasEarnings.DifferenceContractType2 &&

                        y["DasEarningsTotal"] == (double) _getSubmissionsSummaryMetricsResponse.DasEarnings.Total &&
                        y["DasEarningsContractType1Total"] == (double) _getSubmissionsSummaryMetricsResponse.DasEarnings.ContractType1 &&
                        y["DasEarningsContractType2Total"] == (double) _getSubmissionsSummaryMetricsResponse.DasEarnings.ContractType2 &&
                
                        y["DcEarningsTotal"] == (double) _getSubmissionsSummaryMetricsResponse.DcEarnings.Total &&
                        y["DcEarningsContractType1Total"]== (double) _getSubmissionsSummaryMetricsResponse.DcEarnings.ContractType1 &&
                        y["DcEarningsContractType2Total"] == (double) _getSubmissionsSummaryMetricsResponse.DcEarnings.ContractType2 &&
                
                        y["DataLockedEarnings"] == (double) _getSubmissionsSummaryMetricsResponse.TotalDataLockedEarnings &&
                        y["DataLockedAlreadyPaidAmount"] == (double) _getSubmissionsSummaryMetricsResponse.AlreadyPaidDataLockedEarnings &&
                        y["DataLockedAdjustedAmount"] == (double) _getSubmissionsSummaryMetricsResponse.AdjustedDataLockedEarnings &&
                
                        y["HeldBackCompletionPaymentsTotal"] == (double) _getSubmissionsSummaryMetricsResponse.HeldBackCompletionPayments.Total &&
                        y["HeldBackCompletionPaymentsContractType1"] == (double) _getSubmissionsSummaryMetricsResponse.HeldBackCompletionPayments.ContractType1 &&
                        y["HeldBackCompletionPaymentsContractType2"] == (double) _getSubmissionsSummaryMetricsResponse.HeldBackCompletionPayments.ContractType1 &&
                
                        y["RequiredPaymentsTotal"] == (double) _getSubmissionsSummaryMetricsResponse.RequiredPayments.Total &&
                        y["RequiredPaymentsAct1Total"] == (double) _getSubmissionsSummaryMetricsResponse.RequiredPayments.ContractType1 &&
                        y["RequiredPaymentsAc2Total"] == (double) _getSubmissionsSummaryMetricsResponse.RequiredPayments.ContractType2 &&
                
                        y["YearToDatePaymentsTotal"] == (double) _getSubmissionsSummaryMetricsResponse.YearToDatePayments.Total &&
                        y["YearToDatePaymentsContractType1Total"] == (double) _getSubmissionsSummaryMetricsResponse.YearToDatePayments.ContractType1 &&
                        y["YearToDatePaymentsContractType2Total"]== (double) _getSubmissionsSummaryMetricsResponse.YearToDatePayments.ContractType2 &&
                        
                        y["RequiredPaymentsDasEarningsPercentageComparison"] == Math.Round(((double) (_getSubmissionsSummaryMetricsResponse.YearToDatePayments.Total + _getSubmissionsSummaryMetricsResponse.RequiredPayments.Total) / (double) _getSubmissionsSummaryMetricsResponse.DasEarnings.Total) * 100, 2)
                        )));
        }

        public void Assert_SubmissionsSummaryMetrics_AreReturned(SubmissionsSummaryModel result)
        {
            Assert.AreEqual(_getSubmissionsSummaryMetricsResponse, result);
        }

    }
}