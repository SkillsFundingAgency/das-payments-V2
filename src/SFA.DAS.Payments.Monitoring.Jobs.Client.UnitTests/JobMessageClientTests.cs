using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client.UnitTests
{
    [TestFixture]
    public class JobMessageClientTests
    {
        private AutoMock moqer;

        [SetUp]
        public void SetUp()
        {
            moqer = AutoMock.GetLoose();
            var cfgMock = moqer.Mock<IConfigurationHelper>();
            cfgMock.Setup(cfg => cfg.HasSetting("Settings", "Monitoring_JobsService_EndpointName"))
                    .Returns(true);
            cfgMock.Setup(cfg => cfg.GetSetting("Settings", "Monitoring_JobsService_EndpointName"))
                .Returns("sfa-das-payments-monitoring-jobs");
        }

        [Test]
        public async Task Sends_RecordBatchOfJobMessageProcessingStatus_For_Batches_Of_Status_Messages()
        {
            var client = moqer.Create<JobMessageClient>();
            var messages = Enumerable.Range(1, 501).Select(i => ((long)1, Guid.NewGuid(), "TestMessage")).ToList();
            await client.ProcessedJobMessages(messages);
            moqer.Mock<IMessageSession>()
                .Verify(x => x.Send(It.IsAny<RecordBatchOfJobMessageProcessingStatus>(), It.IsAny<SendOptions>()));
        }


        [Test]
        public async Task Splits_Batch_Job_Status_Messages_Into_Batches_Of_500()
        {
            var client = moqer.Create<JobMessageClient>();
            var messages = Enumerable.Range(1, 501).Select(i => ((long)1, Guid.NewGuid(), "TestMessage")).ToList();
            await client.ProcessedJobMessages(messages);
            moqer.Mock<IMessageSession>()
                .Verify(x => x.Send(It.Is<RecordBatchOfJobMessageProcessingStatus>(msg => msg.JobMessageProcessingStatuses.Count == 500), It.IsAny<SendOptions>()));
            moqer.Mock<IMessageSession>()
                .Verify(x => x.Send(It.Is<RecordBatchOfJobMessageProcessingStatus>(msg => msg.JobMessageProcessingStatuses.Count == 1), It.IsAny<SendOptions>()));
        }

        [Test]
        public async Task Splits_ProcessingStatus_Batches_By_JobId()
        {
            var client = moqer.Create<JobMessageClient>();
            var messages = Enumerable.Range(1, 10).Select(i => ((long)1, Guid.NewGuid(), "TestMessage")).ToList();
            messages.AddRange(Enumerable.Range(1, 20).Select(i => ((long)2, Guid.NewGuid(), "TestMessage")).ToList());
            await client.ProcessedJobMessages(messages);
            moqer.Mock<IMessageSession>()
                .Verify(x => x.Send(It.Is<RecordBatchOfJobMessageProcessingStatus>(msg => msg.JobMessageProcessingStatuses.Count == 10 && msg.JobMessageProcessingStatuses.All(status => status.JobId == 1)), It.IsAny<SendOptions>()));
            moqer.Mock<IMessageSession>()
                .Verify(x => x.Send(It.Is<RecordBatchOfJobMessageProcessingStatus>(msg => msg.JobMessageProcessingStatuses.Count == 20 && msg.JobMessageProcessingStatuses.All(status => status.JobId == 2)), It.IsAny<SendOptions>()));

        }
    }
}