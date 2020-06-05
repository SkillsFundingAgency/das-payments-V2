using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using FluentAssertions;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.JobContextMessageHandling.Infrastructure;
using SFA.DAS.Payments.JobContextMessageHandling.JobStatus;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.PeriodEnd.Application.Handlers;
using SFA.DAS.Payments.PeriodEnd.Application.Infrastructure;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.PeriodEnd.Application.UnitTests
{
    [TestFixture]
    public class PeriodEndJobContextMessageHandlerTests
    {
        private Autofac.Extras.Moq.AutoMock mocker;
        
        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mocker.Mock<IEndpointInstance>()
                .Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()))
                .Returns(Task.CompletedTask);
            mocker.Mock<IEndpointInstanceFactory>()
                .Setup(factory => factory.GetEndpointInstance())
                .ReturnsAsync(mocker.Mock<IEndpointInstance>().Object);
            mocker.Mock<IPeriodEndJobClient>()
                .Setup(client => client.RecordPeriodEndStart(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>(),
                    It.IsAny<List<GeneratedMessage>>()))
                .Returns(Task.CompletedTask);
            mocker.Mock<IPeriodEndJobClient>()
                .Setup(client => client.RecordPeriodEndRun(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>(),
                    It.IsAny<List<GeneratedMessage>>()))
                .Returns(Task.CompletedTask);
            mocker.Mock<IPeriodEndJobClient>()
                .Setup(client => client.RecordPeriodEndStop(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>(),
                    It.IsAny<List<GeneratedMessage>>()))
                .Returns(Task.CompletedTask);
            mocker.Mock<IJobStatusService>()
                .Setup(svc => svc.WaitForJobToFinish(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        }

        [Test]
        public async Task Publishes_Period_End_Started_Event_From_Period_End_Start_Task()
        {
            var jobContextMessage = CreateJobContextMessage("PeriodEndStart");
            
            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.Is<PeriodEndStartedEvent>(startedEvent => startedEvent.JobId == 1
                                                                                    && startedEvent.CollectionPeriod.Period == 10
                                                                                    && startedEvent.CollectionPeriod.AcademicYear == 1819),
                    It.IsAny<PublishOptions>()), Times.Once);
        }


        [Test]
        public async Task Records_Period_End_Started_Job_From_Period_End_Start_Task()
        {

            var jobContextMessage = CreateJobContextMessage("PeriodEndStart");

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IPeriodEndJobClient>()
                .Verify(x => x.RecordPeriodEndStart( It.Is<long>(jobId => jobId == 1), It.Is<short>(collectionYear => collectionYear == 1819),It.Is<byte>(period => period == 10),
                    It.Is<List<GeneratedMessage>>(msgs => msgs.Any(msg => msg.MessageName.Equals(typeof(PeriodEndStartedEvent).FullName)))), Times.Once);
        }



        [Test]
        public async Task Publishes_Period_End_Run_Event_From_Period_End_Run_Task()
        {

            var jobContextMessage = CreateJobContextMessage("PeriodEndRun");

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.Is<PeriodEndRunningEvent>(startedEvent => startedEvent.JobId == 1
                                                                                    && startedEvent.CollectionPeriod.Period == 10
                                                                                    && startedEvent.CollectionPeriod.AcademicYear == 1819),
                    It.IsAny<PublishOptions>()), Times.Once);
        }


        [Test]
        public async Task Records_Period_End_Running_Job_From_Period_End_Run_Task()
        {

            var jobContextMessage = CreateJobContextMessage("PeriodEndRun");

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IPeriodEndJobClient>()
                .Verify(x => x.RecordPeriodEndRun(It.Is<long>(jobId => jobId == 1), It.Is<short>(collectionYear => collectionYear == 1819), It.Is<byte>(period => period == 10),
                    It.Is<List<GeneratedMessage>>(msgs => msgs.Any(msg => msg.MessageName.Equals(typeof(PeriodEndRunningEvent).FullName)))), Times.Once);
        }

        [Test]
        public async Task Publishes_Period_End_Stopped_Event_From_Period_End_Stop_Task()
        {

            var jobContextMessage = CreateJobContextMessage("PeriodEndStop");

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.Is<PeriodEndStoppedEvent>(startedEvent => startedEvent.JobId == 1
                                                                                    && startedEvent.CollectionPeriod.Period == 10
                                                                                    && startedEvent.CollectionPeriod.AcademicYear == 1819),
                    It.IsAny<PublishOptions>()), Times.Once);
        }


        [Test]
        public async Task Records_Period_End_Stopped_Job_From_Period_End_Stop_Task()
        {

            var jobContextMessage = CreateJobContextMessage("PeriodEndStop");

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IPeriodEndJobClient>()
                .Verify(x => x.RecordPeriodEndStop(It.Is<long>(jobId => jobId == 1), It.Is<short>(collectionYear => collectionYear == 1819), It.Is<byte>(period => period == 10),
                    It.Is<List<GeneratedMessage>>(msgs => msgs.Any(msg => msg.MessageName.Equals(typeof(PeriodEndStoppedEvent).FullName)))), Times.Once);
        }

        [Test]
        public async Task Fails_If_No_Task_Name()
        {
            var jobContextMessage = CreateJobContextMessage(null);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(jobContextMessage, CancellationToken.None));
        }

        [Test]
        public async Task Fails_If_Invalid_Task_Name()
        {
            var jobContextMessage = CreateJobContextMessage("Some Task");
            
            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(jobContextMessage, CancellationToken.None));
        }

        [Test]
        public async Task Fails_If_No_Return_Period()
        {
            var jobContextMessage = CreateJobContextMessage("PeriodEndStop", CreateReturnPeriod:false);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            Assert.ThrowsAsync<KeyNotFoundException>(() => handler.HandleAsync(jobContextMessage, CancellationToken.None));
        }

        [Test]
        public async Task Fails_If_No_Collection_Year()
        {
            var jobContextMessage =  CreateJobContextMessage("PeriodEndStop",  CreateCollectionYear:false);
            
            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            Assert.ThrowsAsync<KeyNotFoundException>(() => handler.HandleAsync(jobContextMessage, CancellationToken.None));
        }

        [Test]
        [TestCase("PeriodEndRun", 1)]
        [TestCase("PeriodEndStop", 0)]
        public async Task Waits_For_Job_To_Complete(string task, int numberOfTimes)
        {
            var jobContextMessage = CreateJobContextMessage(task);
            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            var completed = await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IJobStatusService>()
                .Verify(svc => svc.WaitForJobToFinish(It.Is<long>(jobId => jobId == 1), CancellationToken.None),Times.Exactly(numberOfTimes));
        }

        [Test]
        public async Task PeriodEndStart_Waits_For_Job_To_Complete()
        {
            var jobContextMessage = CreateJobContextMessage("PeriodEndStart");

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            var completed = await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IJobStatusService>()
                .Verify(svc => svc.WaitForPeriodEndStartedToFinish(It.Is<long>(jobId => jobId == 1), CancellationToken.None),Times.Once);
        }

        [Test]
        public async Task Returns_True_Even_If_Job_Not_Completed()
        {
            var jobContextMessage = new JobContextMessage
            {
                JobId = 1,
                Topics = new List<ITopicItem>
                {
                    new TopicItem
                    {
                        SubscriptionName = "PeriodEnd",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{"PeriodEndStop"}
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object> {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 },
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 } }
            };
            mocker.Mock<IJobStatusService>()
                .Setup(svc => svc.WaitForJobToFinish(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            var completed = await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            completed.Should().BeTrue();
        }

        private static JobContextMessage CreateJobContextMessage(string task, bool CreateReturnPeriod = true, bool CreateCollectionYear = true)
        {
            var jobContextMessage = new JobContextMessage
            {
                JobId = 1,
                Topics = new List<ITopicItem>
                {
                    new TopicItem
                    {
                        SubscriptionName = "PeriodEnd",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string> {task}
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object>()
               
            };
            if(CreateReturnPeriod)
                jobContextMessage.KeyValuePairs.Add(JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10);

            if(CreateCollectionYear)
                jobContextMessage.KeyValuePairs.Add(JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819);

            return jobContextMessage;
        }
    }
}