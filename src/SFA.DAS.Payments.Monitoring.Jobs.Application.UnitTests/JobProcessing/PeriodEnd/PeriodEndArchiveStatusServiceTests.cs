using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests.JobProcessing.PeriodEnd
{
    [TestFixture]
    public class PeriodEndArchiveStatusServiceTests
    {
        private Fixture _fixture;
        
        private Mock<IPeriodEndArchiveConfiguration> _periodEndArchiveConfiguration;
        private Mock<IJobServiceConfiguration> _jobServiceConfiguration;
        private Mock<IJobStorageService> _jobStorageService;
        private Mock<IPaymentLogger> _logger;
        private Mock<ITelemetry> _telemetry;
        private Mock<IPeriodEndArchiveFunctionHttpClient> _httpClient;

        private PeriodEndArchiveStatusService _sut;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();

            _periodEndArchiveConfiguration = new Mock<IPeriodEndArchiveConfiguration>();
            _jobServiceConfiguration = new Mock<IJobServiceConfiguration>();
            _jobStorageService = new Mock<IJobStorageService>();
            _logger = new Mock<IPaymentLogger>();
            _telemetry = new Mock<ITelemetry>();
            _httpClient = new Mock<IPeriodEndArchiveFunctionHttpClient>();

            _jobServiceConfiguration.Setup(x => x.TimeToWaitForPeriodEndArchiveJobToComplete)
                .Returns(new TimeSpan(0, 5, 30, 0));

            _sut = new PeriodEndArchiveStatusService(_jobStorageService.Object, _logger.Object, _telemetry.Object,
                _jobServiceConfiguration.Object, _periodEndArchiveConfiguration.Object, _httpClient.Object);
        }

        [Test]
        public async Task Service_Waits_For_Job_To_Complete()
        {
            var jobId = _fixture.Create<long>();
            var inProgressJob = new JobModel
            {
                DcJobId = jobId,
                Status = JobStatus.InProgress,
                StartTime = DateTime.Now.AddMinutes(-1)
            };

            _jobStorageService.Setup(x => x.GetJob(jobId, It.IsAny<CancellationToken>())).ReturnsAsync(inProgressJob);

            var archiveStatus = new ArchiveRunInformation
            {
                InstanceId = _fixture.Create<string>(),
                JobId = jobId.ToString(),
                Status = "InProgress"
            };
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(archiveStatus))
            };
            _httpClient.Setup(x => x.GetArchiveFunctionStatus(jobId)).ReturnsAsync(httpResponse);

            var result = await _sut.ManageStatus(jobId, new CancellationToken());

            result.Should().BeFalse();
            _jobServiceConfiguration.Verify(x => x.TimeToWaitForPeriodEndArchiveJobToComplete, Times.Once);
        }

        [Test]
        public async Task Service_Updates_Job_Status_When_Completed()
        {
            var jobId = _fixture.Create<long>();
            var inProgressJob = new JobModel
            {
                DcJobId = jobId,
                Status = JobStatus.InProgress,
                StartTime = DateTime.Now.AddMinutes(-1)
            };

            _jobStorageService.Setup(x => x.GetJob(jobId, It.IsAny<CancellationToken>())).ReturnsAsync(inProgressJob);

            var archiveStatus = new ArchiveRunInformation
            {
                InstanceId = _fixture.Create<string>(),
                JobId = jobId.ToString(),
                Status = "Succeeded"
            };
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(archiveStatus))
            };
            _httpClient.Setup(x => x.GetArchiveFunctionStatus(jobId)).ReturnsAsync(httpResponse);

            var result = await _sut.ManageStatus(jobId, new CancellationToken());

            result.Should().BeTrue();
            _jobServiceConfiguration.Verify(x => x.TimeToWaitForPeriodEndArchiveJobToComplete, Times.Once);
            _jobStorageService.Verify(
                x => x.SaveJobStatus(jobId, JobStatus.Completed, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Service_Updates_Job_Status_When_Failed()
        {
            var jobId = _fixture.Create<long>();
            var inProgressJob = new JobModel
            {
                DcJobId = jobId,
                Status = JobStatus.InProgress,
                StartTime = DateTime.Now.AddMinutes(-1)
            };

            _jobStorageService.Setup(x => x.GetJob(jobId, It.IsAny<CancellationToken>())).ReturnsAsync(inProgressJob);

            var archiveStatus = new ArchiveRunInformation
            {
                InstanceId = _fixture.Create<string>(),
                JobId = jobId.ToString(),
                Status = "Failed"
            };
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(archiveStatus))
            };
            _httpClient.Setup(x => x.GetArchiveFunctionStatus(jobId)).ReturnsAsync(httpResponse);

            var result = await _sut.ManageStatus(jobId, new CancellationToken());

            result.Should().BeTrue();
            _jobServiceConfiguration.Verify(x => x.TimeToWaitForPeriodEndArchiveJobToComplete, Times.Once);
            _jobStorageService.Verify(
                x => x.SaveJobStatus(jobId, JobStatus.PaymentsTaskFailed, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Test]
        public async Task Service_Updates_Job_Status_When_Service_Error()
        {
            var jobId = _fixture.Create<long>();
            var inProgressJob = new JobModel
            {
                DcJobId = jobId,
                Status = JobStatus.InProgress,
                StartTime = DateTime.Now.AddMinutes(-1)
            };

            _jobStorageService.Setup(x => x.GetJob(jobId, It.IsAny<CancellationToken>())).ReturnsAsync(inProgressJob);

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            };
            _httpClient.Setup(x => x.GetArchiveFunctionStatus(jobId)).ReturnsAsync(httpResponse);

            var result = await _sut.ManageStatus(jobId, new CancellationToken());

            result.Should().BeTrue();
            _jobServiceConfiguration.Verify(x => x.TimeToWaitForPeriodEndArchiveJobToComplete, Times.Once);
            _jobStorageService.Verify(
                x => x.SaveJobStatus(jobId, JobStatus.PaymentsTaskFailed, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Service_Updates_Job_Status_When_Timed_Out()
        {
            var jobId = _fixture.Create<long>();
            var inProgressJob = new JobModel
            {
                DcJobId = jobId,
                Status = JobStatus.InProgress,
                StartTime = DateTime.Now.AddMinutes(-31)
            };

            _jobStorageService.Setup(x => x.GetJob(jobId, It.IsAny<CancellationToken>())).ReturnsAsync(inProgressJob);

            _jobServiceConfiguration.Setup(x => x.TimeToWaitForPeriodEndArchiveJobToComplete)
                .Returns(new TimeSpan(0, 0, 30, 0));

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            };
            _httpClient.Setup(x => x.GetArchiveFunctionStatus(jobId)).ReturnsAsync(httpResponse);

            var result = await _sut.ManageStatus(jobId, new CancellationToken());

            result.Should().BeTrue();
            _jobServiceConfiguration.Verify(x => x.TimeToWaitForPeriodEndArchiveJobToComplete, Times.Once);
            _jobStorageService.Verify(
                x => x.SaveJobStatus(jobId, JobStatus.TimedOut, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
