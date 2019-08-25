using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Microsoft.EntityFrameworkCore;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Handlers;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Exceptions;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class RecordJobMessageProcessingStatusHandlerTests
    {
        private AutoMock mocker;
        private List<JobStepModel> jobSteps;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            var mockConfig = mocker.Mock<IConfigurationHelper>();
            mockConfig.Setup(cfg => cfg.HasSetting(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            mockConfig.Setup(cfg => cfg.GetSetting(It.IsAny<string>(), It.IsAny<string>())).Returns("1");
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.StoreJobMessages(It.IsAny<List<JobStepModel>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            jobSteps = new List<JobStepModel>();
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobMessages(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobSteps);
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetInProgressMessageIdentifiers(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid> ());
            mocker.Mock<IJobStorageService>()
                .Setup(x => x.GetJobStatus(It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobStepStatus.Completed,DateTimeOffset.UtcNow));

        }

        [Test]
        public async Task Stores_New_Job_Messages()
        {
            var recordJobMessageProcessingStatus = new RecordJobMessageProcessingStatus
            {
                JobId = 1,
                MessageName = "Test Message",
                Id = Guid.NewGuid()
            };

            var jobMessageService = mocker.Create<JobMessageService>();
            await jobMessageService.JobMessageCompleted(recordJobMessageProcessingStatus);

            mocker.Mock<IJobStorageService>()
                .Verify(svc => svc.StoreJobMessages(It.Is<List<JobStepModel>>(lst => lst.Count == 1 && lst.All(item => item.MessageId == recordJobMessageProcessingStatus.Id)),It.IsAny<CancellationToken>()),Times.Once);
        }

        [Test]
        public async Task Updates_End_Time_For_Job_Messages()
        {
            var recordJobMessageProcessingStatus = new RecordJobMessageProcessingStatus
            {
                JobId = 1,
                MessageName = "Test Message",
                Id = Guid.NewGuid(),
                Succeeded = true,
                EndTime = DateTimeOffset.Now
            };

            jobSteps.Add(new JobStepModel
            {
                MessageId = recordJobMessageProcessingStatus.Id
            });

            var jobMessageService = mocker.Create<JobMessageService>();
            await jobMessageService.JobMessageCompleted(recordJobMessageProcessingStatus);

            mocker.Mock<IJobStorageService>()
                .Verify(svc => svc.StoreJobMessages(It.Is<List<JobStepModel>>(lst => lst.Count == 1 && lst.All(item => item.MessageId == recordJobMessageProcessingStatus.Id &&
                                                                                                                       item.EndTime == recordJobMessageProcessingStatus.EndTime )), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Records_Status_Completed_If_Succeeded()
        {
            var recordJobMessageProcessingStatus = new RecordJobMessageProcessingStatus
            {
                JobId = 1,
                MessageName = "Test Message",
                Id = Guid.NewGuid(),
                Succeeded = true,
                EndTime = DateTimeOffset.Now
            };

            jobSteps.Add(new JobStepModel
            {
                MessageId = recordJobMessageProcessingStatus.Id
            });

            var jobMessageService = mocker.Create<JobMessageService>();
            await jobMessageService.JobMessageCompleted(recordJobMessageProcessingStatus);

            mocker.Mock<IJobStorageService>()
                .Verify(svc => svc.StoreJobMessages(It.Is<List<JobStepModel>>(lst => lst.Count == 1 && lst.All(item => item.MessageId == recordJobMessageProcessingStatus.Id &&
                                                                                                                       item.Status == JobStepStatus.Completed)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Records_Status_Failed_If_Not_Succeeded()
        {
            var recordJobMessageProcessingStatus = new RecordJobMessageProcessingStatus
            {
                JobId = 1,
                MessageName = "Test Message",
                Id = Guid.NewGuid(),
                Succeeded = false,
                EndTime = DateTimeOffset.Now
            };

            jobSteps.Add(new JobStepModel
            {
                MessageId = recordJobMessageProcessingStatus.Id
            });

            var jobMessageService = mocker.Create<JobMessageService>();
            await jobMessageService.JobMessageCompleted(recordJobMessageProcessingStatus);

            mocker.Mock<IJobStorageService>()
                .Verify(svc => svc.StoreJobMessages(It.Is<List<JobStepModel>>(lst => lst.Count == 1 && lst.All(item => item.MessageId == recordJobMessageProcessingStatus.Id &&
                                                                                                                       item.Status == JobStepStatus.Failed)), It.IsAny<CancellationToken>()), Times.Once);
        }
        //[Test]
        //public async Task Sends_Message_Back_To_Queue_If_Constraint_Violation()
        //{
        //    mocker.Mock<IJobMessageService>()
        //        .Setup(svc => svc.JobMessageCompleted(It.IsAny<RecordJobMessageProcessingStatus>()))
        //        .Throws(new DbUpdateException("Violation of primary key", (Exception)null));
        //    mocker.Mock<ISqlExceptionService>().Setup(svc => svc.IsConstraintViolation(It.IsAny<DbUpdateException>()))
        //        .Returns(true);
        //    var mockContext = mocker.Mock<IMessageHandlerContext>();
        //    mockContext.Setup(ctx => ctx.MessageHeaders).Returns(new Dictionary<string, string>());
        //    var handler = mocker.Create<RecordJobMessageProcessingStatusHandler>();
        //    var recordJobMessageProcessingStatus = new RecordJobMessageProcessingStatus
        //    {
        //        JobId = 1,
        //        MessageName = "Test Message",
        //        Id = Guid.NewGuid()
        //    };
        //    await handler.Handle(recordJobMessageProcessingStatus, mockContext.Object);
        //    mockContext.Verify(ctx => ctx.Send(It.Is<Object>(msg => recordJobMessageProcessingStatus == msg), It.IsAny<SendOptions>()), Times.Once);
        //}

        //[Test]
        //public async Task ReThrows_Exception_If_Constraint_Violation_Failed_Too_Many_Times()
        //{
        //    mocker.Mock<IJobMessageService>()
        //        .Setup(svc => svc.JobMessageCompleted(It.IsAny<RecordJobMessageProcessingStatus>()))
        //        .Throws(new DbUpdateException("Violation of primary key", (Exception)null));
        //    mocker.Mock<ISqlExceptionService>().Setup(svc => svc.IsConstraintViolation(It.IsAny<DbUpdateException>()))
        //        .Returns(true);
        //    var mockContext = mocker.Mock<IMessageHandlerContext>();
        //    mockContext.Setup(ctx => ctx.MessageHeaders).Returns(new Dictionary<string, string> {{ "JobUpdateFailedRetries", "5" }});
        //    var handler = mocker.Create<RecordJobMessageProcessingStatusHandler>();
        //    var recordJobMessageProcessingStatus = new RecordJobMessageProcessingStatus
        //    {
        //        JobId = 1,
        //        MessageName = "Test Message",
        //        Id = Guid.NewGuid()
        //    };
        //    Assert.ThrowsAsync<InvalidOperationException>(async () => await handler.Handle(recordJobMessageProcessingStatus, mockContext.Object));
        //}

        //[Test]
        //public async Task ReThrows_Exception_If_Not_Constraint_Violation()
        //{
        //    mocker.Mock<IJobMessageService>()
        //        .Setup(svc => svc.JobMessageCompleted(It.IsAny<RecordJobMessageProcessingStatus>()))
        //        .Throws(new DbUpdateException("Violation of primary key", (Exception)null));
        //    mocker.Mock<ISqlExceptionService>().Setup(svc => svc.IsConstraintViolation(It.IsAny<DbUpdateException>()))
        //        .Returns(false);
        //    var mockContext = mocker.Mock<IMessageHandlerContext>();
        //    var handler = mocker.Create<RecordJobMessageProcessingStatusHandler>();
        //    var recordJobMessageProcessingStatus = new RecordJobMessageProcessingStatus
        //    {
        //        JobId = 1,
        //        MessageName = "Test Message",
        //        Id = Guid.NewGuid()
        //    };
        //    Assert.ThrowsAsync<DbUpdateException>(async () => await handler.Handle(recordJobMessageProcessingStatus, mockContext.Object));
        //}

        //[Test]
        //public async Task Sends_Message_Back_To_Queue_If_Job_Not_Found()
        //{
        //    mocker.Mock<IJobMessageService>()
        //        .Setup(svc => svc.JobMessageCompleted(It.IsAny<RecordJobMessageProcessingStatus>()))
        //        .Throws(new DcJobNotFoundException(1));
        //    var mockContext = mocker.Mock<IMessageHandlerContext>();
        //    mockContext.Setup(ctx => ctx.MessageHeaders).Returns(new Dictionary<string, string>());
        //    var handler = mocker.Create<RecordJobMessageProcessingStatusHandler>();
        //    var recordJobMessageProcessingStatus = new RecordJobMessageProcessingStatus
        //    {
        //        JobId = 1,
        //        MessageName = "Test Message",
        //        Id = Guid.NewGuid()
        //    };
        //    await handler.Handle(recordJobMessageProcessingStatus, mockContext.Object);
        //    mockContext.Verify(ctx => ctx.Send(It.Is<Object>(msg => recordJobMessageProcessingStatus == msg), It.IsAny<SendOptions>()), Times.Once);
        //}

        //[Test]
        //public async Task ReThrows_Exception_If_Job_Not_Found_Too_Many_Times()
        //{
        //    mocker.Mock<IJobMessageService>()
        //        .Setup(svc => svc.JobMessageCompleted(It.IsAny<RecordJobMessageProcessingStatus>()))
        //        .Throws(new DcJobNotFoundException(1));
        //    var mockContext = mocker.Mock<IMessageHandlerContext>();
        //    mockContext.Setup(ctx => ctx.MessageHeaders).Returns(new Dictionary<string, string> { { "JobNotFoundRetries", "5" } });
        //    var handler = mocker.Create<RecordJobMessageProcessingStatusHandler>();
        //    var recordJobMessageProcessingStatus = new RecordJobMessageProcessingStatus
        //    {
        //        JobId = 1,
        //        MessageName = "Test Message",
        //        Id = Guid.NewGuid()
        //    };
        //    Assert.ThrowsAsync<InvalidOperationException>(async () => await handler.Handle(recordJobMessageProcessingStatus, mockContext.Object));
        //}
    }
}