using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class CompletedJobsServiceTests
    {
        private AutoMock mocker;
        private List<JobModel> jobs;
        private ILifetimeScope scope;
        private IContainer container;
        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            jobs = new List<JobModel>
            {
                new JobModel
                {
                    Id = 9909, 
                    StartTime = DateTimeOffset.UtcNow,
                    CollectionPeriod = 1,
                    AcademicYear = 1819,
                    Ukprn = 999,
                    Status = JobStatus.InProgress,
                    JobType = JobType.EarningsJob,
                    IlrSubmissionTime = DateTime.UtcNow.AddSeconds(-10),
                    DcJobId = 99
                },
                new JobModel
                {
                    Id = 9910,
                    StartTime = DateTimeOffset.UtcNow,
                    CollectionPeriod = 1,
                    AcademicYear = 1819,
                    Ukprn = 1000,
                    Status = JobStatus.InProgress,
                    JobType = JobType.MonthEndJob,
                    DcJobId = 100,
                },
            };
            mocker.Mock<IJobsDataContext>()
                .Setup(dc => dc.GetInProgressJobs())
                .Returns(Task.FromResult(jobs));
            
            var builder = new ContainerBuilder();
            builder.RegisterInstance<IJobStatusService>(mocker.Mock<IJobStatusService>().Object);
            container = builder.Build();
            mocker.Mock<IContainerScopeFactory>()
                .Setup(factory => factory.CreateScope())
                .Returns(() => container.BeginLifetimeScope());
        }

        [TearDown]
        public void CleanUp()
        {
            scope?.Dispose();
            scope = null;
            container?.Dispose();
            container = null;
        }

        [Test]
        public async Task Uses_JobStatusService_For_All_UnCompleted_Jobs()
        {
            var service = mocker.Create<CompletedJobsService>();
            await service.UpdateCompletedJobs(default(CancellationToken));
            jobs.ForEach(job =>
            {
                mocker.Mock<IJobStatusService>()
                    .Verify(svc => svc.UpdateStatus(It.Is<JobModel>(j => job.Id == j.Id), It.IsAny<CancellationToken>()), Times.Once, $"Failed to invoke JobStatusService.JobStepsCompleted for job: {job.Id}");
            });
        }
    }
}