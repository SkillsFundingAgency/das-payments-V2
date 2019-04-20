using System;
using System.Collections.Generic;
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

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class RecordJobMessageProcessingStatusHandlerTests
    {
        private AutoMock mocker;
        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            var mockConfig = mocker.Mock<IConfigurationHelper>();
            mockConfig.Setup(cfg => cfg.HasSetting(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            mockConfig.Setup(cfg => cfg.GetSetting(It.IsAny<string>(), It.IsAny<string>())).Returns("1");
        }

        [Test]
        public async Task Sends_Message_Back_To_Queue_If_Constraint_Violation()
        {
            mocker.Mock<IJobStepService>()
                .Setup(svc => svc.JobStepCompleted(It.IsAny<RecordJobMessageProcessingStatus>()))
                .Throws(new DbUpdateException("Violation of primary key", (Exception)null));
            mocker.Mock<ISqlExceptionService>().Setup(svc => svc.IsConstraintViolation(It.IsAny<DbUpdateException>()))
                .Returns(true);
            var mockContext = mocker.Mock<IMessageHandlerContext>();
            mockContext.Setup(ctx => ctx.MessageHeaders).Returns(new Dictionary<string, string>());
            var handler = mocker.Create<RecordJobMessageProcessingStatusHandler>();
            var recordJobMessageProcessingStatus = new RecordJobMessageProcessingStatus
            {
                JobId = 1,
                MessageName = "Test Message",
                Id = Guid.NewGuid()
            };
            await handler.Handle(recordJobMessageProcessingStatus, mockContext.Object);
            mockContext.Verify(ctx => ctx.Send(It.Is<Object>(msg => recordJobMessageProcessingStatus == msg), It.IsAny<SendOptions>()), Times.Once);
        }

        [Test]
        public async Task ReThrows_Exception_If_Constraint_Violation_Failed_Too_Many_Times()
        {
            mocker.Mock<IJobStepService>()
                .Setup(svc => svc.JobStepCompleted(It.IsAny<RecordJobMessageProcessingStatus>()))
                .Throws(new DbUpdateException("Violation of primary key", (Exception)null));
            mocker.Mock<ISqlExceptionService>().Setup(svc => svc.IsConstraintViolation(It.IsAny<DbUpdateException>()))
                .Returns(true);
            var mockContext = mocker.Mock<IMessageHandlerContext>();
            mockContext.Setup(ctx => ctx.MessageHeaders).Returns(new Dictionary<string, string> {{ "JobUpdateFailedRetries", "5" }});
            var handler = mocker.Create<RecordJobMessageProcessingStatusHandler>();
            var recordJobMessageProcessingStatus = new RecordJobMessageProcessingStatus
            {
                JobId = 1,
                MessageName = "Test Message",
                Id = Guid.NewGuid()
            };
            Assert.ThrowsAsync<DbUpdateException>(async () => await handler.Handle(recordJobMessageProcessingStatus, mockContext.Object));
        }

        [Test]
        public async Task ReThrows_Exception_If_Not_Constraint_Violation()
        {
            mocker.Mock<IJobStepService>()
                .Setup(svc => svc.JobStepCompleted(It.IsAny<RecordJobMessageProcessingStatus>()))
                .Throws(new DbUpdateException("Violation of primary key", (Exception)null));
            mocker.Mock<ISqlExceptionService>().Setup(svc => svc.IsConstraintViolation(It.IsAny<DbUpdateException>()))
                .Returns(false);
            var mockContext = mocker.Mock<IMessageHandlerContext>();
            var handler = mocker.Create<RecordJobMessageProcessingStatusHandler>();
            var recordJobMessageProcessingStatus = new RecordJobMessageProcessingStatus
            {
                JobId = 1,
                MessageName = "Test Message",
                Id = Guid.NewGuid()
            };
            Assert.ThrowsAsync<DbUpdateException>(async () => await handler.Handle(recordJobMessageProcessingStatus, mockContext.Object));
        }

        [Test]
        public async Task Sends_Message_Back_To_Queue_If_Job_Not_Found()
        {
            mocker.Mock<IJobStepService>()
                .Setup(svc => svc.JobStepCompleted(It.IsAny<RecordJobMessageProcessingStatus>()))
                .Throws(new DcJobNotFoundException(1));
            var mockContext = mocker.Mock<IMessageHandlerContext>();
            mockContext.Setup(ctx => ctx.MessageHeaders).Returns(new Dictionary<string, string>());
            var handler = mocker.Create<RecordJobMessageProcessingStatusHandler>();
            var recordJobMessageProcessingStatus = new RecordJobMessageProcessingStatus
            {
                JobId = 1,
                MessageName = "Test Message",
                Id = Guid.NewGuid()
            };
            await handler.Handle(recordJobMessageProcessingStatus, mockContext.Object);
            mockContext.Verify(ctx => ctx.Send(It.Is<Object>(msg => recordJobMessageProcessingStatus == msg), It.IsAny<SendOptions>()), Times.Once);
        }

        [Test]
        public async Task ReThrows_Exception_If_Job_Not_Found_Too_Many_Times()
        {
            mocker.Mock<IJobStepService>()
                .Setup(svc => svc.JobStepCompleted(It.IsAny<RecordJobMessageProcessingStatus>()))
                .Throws(new DcJobNotFoundException(1));
            var mockContext = mocker.Mock<IMessageHandlerContext>();
            mockContext.Setup(ctx => ctx.MessageHeaders).Returns(new Dictionary<string, string> { { "JobNotFoundRetries", "5" } });
            var handler = mocker.Create<RecordJobMessageProcessingStatusHandler>();
            var recordJobMessageProcessingStatus = new RecordJobMessageProcessingStatus
            {
                JobId = 1,
                MessageName = "Test Message",
                Id = Guid.NewGuid()
            };
            Assert.ThrowsAsync<DcJobNotFoundException>(async () => await handler.Handle(recordJobMessageProcessingStatus, mockContext.Object));
        }
    }
}