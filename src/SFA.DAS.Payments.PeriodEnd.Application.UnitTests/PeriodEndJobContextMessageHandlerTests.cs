﻿using System;
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
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.PeriodEnd.Application.Handlers;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using SFA.DAS.Payments.PeriodEnd.Model;

namespace SFA.DAS.Payments.PeriodEnd.Application.UnitTests
{
    [TestFixture]
    public class PeriodEndJobContextMessageHandlerTests
    {
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
                .Setup(client => client.StartPeriodEndJob(It.IsAny<RecordPeriodEndJob>()))
                .Returns(Task.CompletedTask);
            mocker.Mock<IJobStatusService>()
                .Setup(svc =>
                    svc.WaitForJobToFinish(It.IsAny<long>(), It.IsAny<CancellationToken>(), It.IsAny<TimeSpan?>()))
                .ReturnsAsync(true);
            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetNonFailedDcJobId(It.IsAny<JobType>(), It.IsAny<short>(), It.IsAny<byte>()))
                .ReturnsAsync(0);
        }

        private AutoMock mocker;

        [Test]
        public async Task Publishes_Period_End_Started_Event_From_Period_End_Start_Task()
        {
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndStart);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.Is<PeriodEndStartedEvent>(startedEvent => startedEvent.JobId == 1
                        && startedEvent.CollectionPeriod.Period == 10
                        && startedEvent.CollectionPeriod.AcademicYear == 1819),
                    It.IsAny<PublishOptions>()), Times.Once);
        }

        [Test]
        public async Task Does_Not_Publish_Period_End_Started_Event_When_Job_Already_Exists()
        {
            long existingJobId = 124312;
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndStart);
            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetNonFailedDcJobId(It.IsAny<JobType>(), It.IsAny<short>(), It.IsAny<byte>()))
                .ReturnsAsync(existingJobId);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();

            await handler.HandleAsync(jobContextMessage, CancellationToken.None);

            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.IsAny<PeriodEndStartedEvent>(),
                    It.IsAny<PublishOptions>()), Times.Never);
        }


        [Test]
        public async Task Awaits_For_Existing_Job_When_Job_Already_Exists()
        {
            long existingJobId = 124312;
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndRun);
            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetNonFailedDcJobId(It.IsAny<JobType>(), It.IsAny<short>(), It.IsAny<byte>()))
                .ReturnsAsync(existingJobId);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();

            await handler.HandleAsync(jobContextMessage, CancellationToken.None);

            mocker.Mock<IJobStatusService>()
                .Verify(
                    svc => svc.WaitForPeriodEndRunJobToFinish(It.Is<long>(jobId => jobId == existingJobId),
                        CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task Waits_For_Existing_PeriodEndStartedToFinish_When_Job_Already_Exists()
        {
            long existingJobId = 124312;
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndStart);
            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetNonFailedDcJobId(It.IsAny<JobType>(), It.IsAny<short>(), It.IsAny<byte>()))
                .ReturnsAsync(existingJobId);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();

            await handler.HandleAsync(jobContextMessage, CancellationToken.None);

            mocker.Mock<IJobStatusService>()
                .Verify(
                    svc => svc.WaitForPeriodEndJobToFinish(It.Is<long>(jobId => jobId == existingJobId),
                        CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task Records_Period_End_Started_Job_From_Period_End_Start_Task()
        {
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndStart);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IPeriodEndJobClient>()
                .Verify(x =>
                    x.StartPeriodEndJob(It.Is<RecordPeriodEndJob>(y =>
                        y.GetType().Name == nameof(RecordPeriodEndStartJob) &&
                        y.JobId == 1 &&
                        y.CollectionYear == 1819 &&
                        y.CollectionPeriod == 10 &&
                        y.GeneratedMessages.Any(msg =>
                            msg.MessageName.Equals(typeof(PeriodEndStartedEvent).FullName)))), Times.Once);
        }


        [Test]
        public async Task Publishes_Period_End_Run_Event_From_Period_End_Run_Task()
        {
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndRun);

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
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndRun);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IPeriodEndJobClient>()
                .Verify(x =>
                    x.StartPeriodEndJob(It.Is<RecordPeriodEndJob>(y =>
                        y.GetType().Name == nameof(RecordPeriodEndRunJob) &&
                        y.JobId == 1 &&
                        y.CollectionYear == 1819 &&
                        y.CollectionPeriod == 10 &&
                        y.GeneratedMessages.Any(msg =>
                            msg.MessageName.Equals(typeof(PeriodEndRunningEvent).FullName)))), Times.Once);
        }

        [Test]
        public async Task Publishes_Period_End_Stopped_Event_From_Period_End_Stop_Task()
        {
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndStop);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.Is<PeriodEndStoppedEvent>(startedEvent => startedEvent.JobId == 1
                        && startedEvent.CollectionPeriod.Period == 10
                        && startedEvent.CollectionPeriod.AcademicYear == 1819),
                    It.IsAny<PublishOptions>()), Times.Once);
        }

        [Test]
        public async Task Publishes_Period_End_Reports_Event_From_Period_End_Reports_Task()
        {
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndReports);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.Is<PeriodEndRequestReportsEvent>(startedEvent => startedEvent.JobId == 1
                        && startedEvent.CollectionPeriod.Period == 10
                        && startedEvent.CollectionPeriod.AcademicYear == 1819),
                    It.IsAny<PublishOptions>()), Times.Once);
        }

        [Test]
        public async Task Publishes_Period_End_Validation_Submission_Event_From_Period_End_ValidateSubmission_Task()
        {
            var jobContextMessage =
                CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndSubmissionWindowValidation);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.Is<PeriodEndRequestValidateSubmissionWindowEvent>(startedEvent =>
                        startedEvent.JobId == 1
                        && startedEvent.CollectionPeriod.Period == 10
                        && startedEvent.CollectionPeriod.AcademicYear == 1819),
                    It.IsAny<PublishOptions>()), Times.Once);
        }


        [Test]
        public async Task Records_Period_End_Stopped_Job_From_Period_End_Stop_Task()
        {
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndStop);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IPeriodEndJobClient>()
                .Verify(x =>
                    x.StartPeriodEndJob(It.Is<RecordPeriodEndJob>(y =>
                        y.GetType().Name == nameof(RecordPeriodEndStopJob) &&
                        y.JobId == 1 &&
                        y.CollectionYear == 1819 &&
                        y.CollectionPeriod == 10 &&
                        y.GeneratedMessages.Any(msg =>
                            msg.MessageName.Equals(typeof(PeriodEndStoppedEvent).FullName)))), Times.Once);
        }

        [Test]
        public async Task Fails_If_No_Task_Name()
        {
            var jobContextMessage = CreateJobContextMessage(null);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.HandleAsync(jobContextMessage, CancellationToken.None));
        }

        [Test]
        public async Task Fails_If_Invalid_Task_Name()
        {
            var jobContextMessage = CreateJobContextMessage("Some Task");

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.HandleAsync(jobContextMessage, CancellationToken.None));
        }

        [Test]
        public async Task Fails_If_No_Return_Period()
        {
            var jobContextMessage = CreateJobContextMessage("PeriodEndStop", false);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.HandleAsync(jobContextMessage, CancellationToken.None));
        }

        [Test]
        public async Task Fails_If_No_Collection_Year()
        {
            var jobContextMessage = CreateJobContextMessage("PeriodEndStop", CreateCollectionYear: false);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.HandleAsync(jobContextMessage, CancellationToken.None));
        }

        [Test]
        public async Task PeriodEnd_Stop_Does_Not_Wait_For_Job_To_Complete()
        {
            var jobContextMessage = CreateJobContextMessage("PeriodEndStop");
            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            var completed = await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IJobStatusService>()
                .Verify(
                    svc => svc.WaitForJobToFinish(It.Is<long>(jobId => jobId == 1), CancellationToken.None,
                        It.IsAny<TimeSpan?>()), Times.Exactly(0));
            mocker.Mock<IJobStatusService>()
                .Verify(
                    svc => svc.WaitForPeriodEndRunJobToFinish(It.Is<long>(jobId => jobId == 1), CancellationToken.None),
                    Times.Exactly(0));
        }

        [Test]
        public async Task PeriodEndRun_Waits_For_PeriodEndRunJob_To_Complete()
        {
            var jobContextMessage = CreateJobContextMessage("PeriodEndRun");
            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            var completed = await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IJobStatusService>()
                .Verify(
                    svc => svc.WaitForPeriodEndRunJobToFinish(It.Is<long>(jobId => jobId == 1), CancellationToken.None),
                    Times.Exactly(1));
        }

        [Test]
        public async Task PeriodEndStart_Waits_For_Job_To_Complete()
        {
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndStart);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            var completed = await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IJobStatusService>()
                .Verify(
                    svc => svc.WaitForPeriodEndJobToFinish(It.Is<long>(jobId => jobId == 1), CancellationToken.None),
                    Times.Once);
        }

        [Test]
        public async Task PeriodEndSubmissionWindowValidation_Waits_For_Job_To_Complete()
        {
            var jobContextMessage =
                CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndSubmissionWindowValidation);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            var completed = await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IJobStatusService>()
                .Verify(
                    svc => svc.WaitForPeriodEndJobToFinish(It.Is<long>(jobId => jobId == 1), CancellationToken.None),
                    Times.Once);
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
                                Tasks = new List<string> { "PeriodEndStop" }
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object>
                {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 },
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 }
                }
            };
            mocker.Mock<IJobStatusService>()
                .Setup(svc =>
                    svc.WaitForJobToFinish(It.IsAny<long>(), It.IsAny<CancellationToken>(), It.IsAny<TimeSpan?>()))
                .ReturnsAsync(false);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            var completed = await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            completed.Should().BeTrue();
        }

        [Test]
        public async Task
            Records_Period_End_Submissions_Window_Validation_Job_From_Period_End_Submission_Window_Validation_Task()
        {
            var jobContextMessage =
                CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndSubmissionWindowValidation);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IPeriodEndJobClient>()
                .Verify(x =>
                        x.StartPeriodEndJob(It.Is<RecordPeriodEndJob>(y =>
                            y.GetType().Name == nameof(RecordPeriodEndSubmissionWindowValidationJob) &&
                            y.JobId == 1 &&
                            y.CollectionYear == 1819 &&
                            y.CollectionPeriod == 10 &&
                            y.GeneratedMessages.Any(msg =>
                                msg.MessageName.Equals(typeof(PeriodEndRequestValidateSubmissionWindowEvent)
                                    .FullName)))),
                    Times.Once);
        }

        [Test]
        public async Task Records_Period_End_Request_Reports_Job_From_Period_End_Reports_Task()
        {
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndReports);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IPeriodEndJobClient>()
                .Verify(x =>
                    x.StartPeriodEndJob(It.Is<RecordPeriodEndJob>(y =>
                        y.GetType().Name == nameof(RecordPeriodEndRequestReportsJob) &&
                        y.JobId == 1 &&
                        y.CollectionYear == 1819 &&
                        y.CollectionPeriod == 10 &&
                        y.GeneratedMessages.Any(msg =>
                            msg.MessageName.Equals(typeof(PeriodEndRequestReportsEvent).FullName)))), Times.Once);
        }

        [Test]
        public async Task Publishes_Period_End_Started_Event_From_Period_End_IlrReprocessing_Task()
        {
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndIlrReprocessing);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.Is<PeriodEndIlrReprocessingStartedEvent>(startedEvent => startedEvent.JobId == 1
                        && startedEvent.CollectionPeriod.Period == 10
                        && startedEvent.CollectionPeriod.AcademicYear == 1819),
                    It.IsAny<PublishOptions>()), Times.Once);
        }

        [Test]
        public async Task Does_Not_Publish_Period_End_IlrReprocessing_Event_When_Job_Already_Exists()
        {
            long existingJobId = 124312;
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndIlrReprocessing);
            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetNonFailedDcJobId(It.IsAny<JobType>(), It.IsAny<short>(), It.IsAny<byte>()))
                .ReturnsAsync(existingJobId);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();

            await handler.HandleAsync(jobContextMessage, CancellationToken.None);

            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.IsAny<PeriodEndIlrReprocessingStartedEvent>(),
                    It.IsAny<PublishOptions>()), Times.Never);
        }

        private static JobContextMessage CreateJobContextMessage(string task, bool CreateReturnPeriod = true,
            bool CreateCollectionYear = true)
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
                                Tasks = new List<string> { task }
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object>()
            };
            if (CreateReturnPeriod)
                jobContextMessage.KeyValuePairs.Add(JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10);

            if (CreateCollectionYear)
                jobContextMessage.KeyValuePairs.Add(JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819);

            return jobContextMessage;
        }

        private static JobContextMessage CreatePeriodEndJobContextMessage(PeriodEndTaskType taskType)
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
                                Tasks = new List<string> { taskType.ToString() }
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object>
                {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 },
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 }
                }
            };
            return jobContextMessage;
        }

        [Test]
        public async Task Publishes_FCS_Handover_Complete_Event_From_FCS_Handover_Complete_Task()
        {
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndFcsHandoverComplete);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.Is<PeriodEndFcsHandOverCompleteEvent>(startedEvent => startedEvent.JobId == 1
                        && startedEvent.CollectionPeriod.Period == 10
                        && startedEvent.CollectionPeriod.AcademicYear == 1819),
                    It.IsAny<PublishOptions>()), Times.Once);
        }

        [Test]
        public async Task Does_Not_Publish_FCS_Handover_Complete_Event_When_Job_Already_Exists()
        {
            long existingJobId = 124312;
            var jobContextMessage = CreatePeriodEndJobContextMessage(PeriodEndTaskType.PeriodEndFcsHandoverComplete);
            mocker.Mock<IJobsDataContext>()
                .Setup(x => x.GetNonFailedDcJobId(It.IsAny<JobType>(), It.IsAny<short>(), It.IsAny<byte>()))
                .ReturnsAsync(existingJobId);

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();

            await handler.HandleAsync(jobContextMessage, CancellationToken.None);

            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.IsAny<PeriodEndFcsHandOverCompleteEvent>(),
                    It.IsAny<PublishOptions>()), Times.Never);
        }
    }
}