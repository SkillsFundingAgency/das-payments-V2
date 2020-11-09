using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests.JobProcessing.PeriodEnd
{
    [TestFixture]
    public class PeriodEndSubmissionWindowValidationJobStatusServiceTests
    {
        private AutoMock mocker;

        private JobModel job;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();

            job = new JobModel
            {
                Id = 1,
                DcJobId = 99,
                StartTime = DateTimeOffset.UtcNow.AddSeconds(-1),
                Status = JobStatus.InProgress,
                LearnerCount = null,
                JobType = JobType.PeriodEndStartJob,
                DcJobEndTime = DateTimeOffset.Now,
                DcJobSucceeded = true,
                IlrSubmissionTime = DateTime.Now,
                Ukprn = 1234,
                AcademicYear = 1920,
                CollectionPeriod = 01
            };

            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJob(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(job);
        }

        [Test]
        public async Task ManageStatus_When_MetricsValidationService_Returns_False_Then_CompletesWithCompletedWithErrorsStatus()
        {
            const int jobId = 99;

            mocker.Mock<IMetricsValidationService>()
                .Setup(x => x.Validate(jobId,It.IsAny<short>(),It.IsAny<byte>()))
                .ReturnsAsync(false);

            var sut = mocker.Create<PeriodEndSubmissionWindowValidationJobStatusService>();
            
            var result = await sut.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            
            result.Should().BeTrue();

            mocker.Mock<IJobStorageService>()
                .Verify(x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.CompletedWithErrors),
                        It.Is<DateTimeOffset>(endTime => endTime != null),
                        It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task ManageStatus_When_MetricsValidationService_Returns_True_Then_CompletesWithCompletedStatus()
        {
            const int jobId = 99;

            mocker.Mock<IMetricsValidationService>()
                .Setup(x => x.Validate(jobId,It.IsAny<short>(),It.IsAny<byte>()))
                .ReturnsAsync(true);

            var sut = mocker.Create<PeriodEndSubmissionWindowValidationJobStatusService>();
            
            var result = await sut.ManageStatus(jobId, CancellationToken.None).ConfigureAwait(false);
            
            result.Should().BeTrue();

            mocker.Mock<IJobStorageService>()
                .Verify(x => x.SaveJobStatus(It.Is<long>(id => id == jobId),
                        It.Is<JobStatus>(status => status == JobStatus.Completed),
                        It.Is<DateTimeOffset>(endTime => endTime != null),
                        It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}