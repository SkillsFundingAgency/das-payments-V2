using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.EarningEvents.Application.Handlers;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.JobContextMessageHandling.Infrastructure;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using ITelemetry = SFA.DAS.Payments.Application.Infrastructure.Telemetry.ITelemetry;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests
{
    [TestFixture]
    public class JobContextMessageHandlerTests
    {
        private AutoMock mocker;
        private FM36Global fm36Global;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            var endpointInstance = new Mock<IEndpointInstance>();

            mocker.Mock<IEndpointInstance>()
                .Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()))
                .Returns(Task.CompletedTask);
            mocker.Mock<IEndpointInstanceFactory>()
                .Setup(factory => factory.GetEndpointInstance())
                .ReturnsAsync(mocker.Mock<IEndpointInstance>().Object);
            fm36Global = new FM36Global
            {
                UKPRN = 123,
                Year = "1819",
                Learners = new List<FM36Learner>
                {
                    new FM36Learner
                    {
                        ULN = 12345,
                        LearnRefNumber = "ref-1234"
                    }
                }
            };
            mocker.Mock<IFileService>()
                .Setup(svc => 
                    svc.OpenReadStreamAsync(It.Is<string>(file => file.Equals("valid path")), 
                        It.Is<string>(containerName => containerName.Equals("container")), 
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream());
            mocker.Mock<IJsonSerializationService>()
                .Setup(svc => svc.Deserialize<FM36Global>(It.IsAny<Stream>()))
                .Returns(fm36Global);
            mocker.Mock<IEarningsJobClientFactory>()
                .Setup(factory => factory.Create())
                .Returns(mocker.Mock<IEarningsJobClient>().Object);
            mocker.Mock<IEarningsJobClient>()
                .Setup(svc => svc.StartJob(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<short>(),
                    It.IsAny<byte>(),
                    It.IsAny<List<GeneratedMessage>>(),
                    It.IsAny<DateTimeOffset>()))
                .Returns(Task.CompletedTask);
            mocker.Mock<ISubmittedLearnerAimBuilder>()
                .Setup(builder => builder.Build(It.IsAny<ProcessLearnerCommand>()))
                .Returns(new List<SubmittedLearnerAimModel>());
        }

        [Test]
        public async Task Uses_Correct_Filename_For_In_Period_Submission()
        {
            var jobContextMessage = new JobContextMessage
            {
                JobId = 1,
                Topics = new List<ITopicItem>
                {
                    new TopicItem
                    {
                        SubscriptionName = "GenerateFM36Payments",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{ JobContextMessageConstants.Tasks.ProcessSubmission}
                            }
                        }
                    },
                    new TopicItem
                    {
                        SubscriptionName = "Other Task",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{"Something else"}
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object> {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 },
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 },
                    { JobContextMessageConstants.KeyValuePairs.Ukprn, 2123 },
                    { JobContextMessageConstants.KeyValuePairs.FundingFm36Output, "valid path" },
                    { JobContextMessageConstants.KeyValuePairs.FundingFm36OutputPeriodEnd, "invalid path" },
                    { JobContextMessageConstants.KeyValuePairs.Container, "container" },
                    { JobContextMessageConstants.KeyValuePairs.Filename, "filename" },
                }
            };

            var handler = mocker.Create<JobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            
            mocker.Mock<IFileService>().Verify(svc => svc.OpenReadStreamAsync(
                It.Is<string>(file => file.Equals("valid path")),
                It.Is<string>(containerName => containerName.Equals("container")),
                It.IsAny<CancellationToken>()));
        }


        [Test]
        public async Task Uses_Correct_Filename_For_Period_End_Submission()
        {
            var jobContextMessage = new JobContextMessage
            {
                JobId = 1,
                Topics = new List<ITopicItem>
                {
                    new TopicItem
                    {
                        SubscriptionName = "GenerateFM36Payments",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{ JobContextMessageConstants.Tasks.ProcessPeriodEndSubmission }
                            }
                        }
                    },
                    new TopicItem
                    {
                        SubscriptionName = "Other Task",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{"Something else"}
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object> {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 },
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 },
                    { JobContextMessageConstants.KeyValuePairs.Ukprn, 2123 },
                    { JobContextMessageConstants.KeyValuePairs.FundingFm36Output, "invalid path" },
                    { JobContextMessageConstants.KeyValuePairs.FundingFm36OutputPeriodEnd, "valid path" },
                    { JobContextMessageConstants.KeyValuePairs.Container, "container" },
                    { JobContextMessageConstants.KeyValuePairs.Filename, "filename" },
                }
            };

            var handler = mocker.Create<JobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);

            mocker.Mock<IFileService>().Verify(svc => svc.OpenReadStreamAsync(
                It.Is<string>(file => file.Equals("valid path")),
                It.Is<string>(containerName => containerName.Equals("container")),
                It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task HandlesJobSuccess()
        {
            var jobContextMessage = new JobContextMessage
            {
                JobId = 1,
                TopicPointer = 0,
                Topics = new List<ITopicItem>
                {
                    new TopicItem
                    {
                        SubscriptionName = "GenerateFM36Payments",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{ JobContextMessageConstants.Tasks.JobSuccess }
                            }
                        }
                    },
                    new TopicItem
                    {
                        SubscriptionName = "Other Task",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{"Something else"}
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object> {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 },
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 },
                    { JobContextMessageConstants.KeyValuePairs.Ukprn, 2123 },
                    { JobContextMessageConstants.KeyValuePairs.FundingFm36Output, "invalid path" },
                    { JobContextMessageConstants.KeyValuePairs.FundingFm36OutputPeriodEnd, "valid path" },
                    { JobContextMessageConstants.KeyValuePairs.Container, "container" },
                    { JobContextMessageConstants.KeyValuePairs.Filename, "filename" },
                }
            };

            var handler = mocker.Create<JobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);

            mocker.Mock<IEndpointInstance>().Verify(x => x.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()), Times.Once);
        }

        [Test]
        public async Task HandlesSubmissionWhenJobSuccessAlsoPresent()
        {
            var jobContextMessage = new JobContextMessage
            {
                JobId = 1,
                TopicPointer = 1,
                Topics = new List<ITopicItem>
                {
                    new TopicItem
                    {
                        SubscriptionName = "GenerateFM36Payments",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{ JobContextMessageConstants.Tasks.JobSuccess }
                            }
                        }
                    },
                    new TopicItem
                    {
                        SubscriptionName = "Other Task",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{"Something else"}
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object> {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 },
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 },
                    { JobContextMessageConstants.KeyValuePairs.Ukprn, 2123 },
                    { JobContextMessageConstants.KeyValuePairs.FundingFm36Output, "invalid path" },
                    { JobContextMessageConstants.KeyValuePairs.FundingFm36OutputPeriodEnd, "valid path" },
                    { JobContextMessageConstants.KeyValuePairs.Container, "container" },
                    { JobContextMessageConstants.KeyValuePairs.Filename, "filename" },
                }
            };

            var handler = mocker.Create<JobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);

            mocker.Mock<IEndpointInstance>().Verify(x => x.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()), Times.Once);

            mocker.Mock<ITelemetry>().Verify(x => x.TrackEvent("Sent All ProcessLearnerCommand Messages", It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, double>>()));
        }

        [Test]
        public async Task HandlesJobFailure()
        {
            var jobContextMessage = new JobContextMessage
            {
                JobId = 1,
                TopicPointer = 0,
                Topics = new List<ITopicItem>
                {
                    new TopicItem
                    {
                        SubscriptionName = "GenerateFM36Payments",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{ JobContextMessageConstants.Tasks.JobFailure }
                            }
                        }
                    },
                    new TopicItem
                    {
                        SubscriptionName = "Other Task",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{"Something else"}
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object> {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 },
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 },
                    { JobContextMessageConstants.KeyValuePairs.Ukprn, 2123 },
                    { JobContextMessageConstants.KeyValuePairs.FundingFm36Output, "invalid path" },
                    { JobContextMessageConstants.KeyValuePairs.FundingFm36OutputPeriodEnd, "valid path" },
                    { JobContextMessageConstants.KeyValuePairs.Container, "container" },
                    { JobContextMessageConstants.KeyValuePairs.Filename, "filename" },
                }
            };

            var handler = mocker.Create<JobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);

            mocker.Mock<IEndpointInstance>().Verify(x => x.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()));
        }

        [Test]
        public async Task HandlesSubmissionWhenJobFailureAlsoPresent()
        {
            var jobContextMessage = new JobContextMessage
            {
                JobId = 1,
                TopicPointer = 1,
                Topics = new List<ITopicItem>
                {
                    new TopicItem
                    {
                        SubscriptionName = "GenerateFM36Payments",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{ JobContextMessageConstants.Tasks.JobFailure }
                            }
                        }
                    },
                    new TopicItem
                    {
                        SubscriptionName = "Other Task",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{"Something else"}
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object> {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 },
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 },
                    { JobContextMessageConstants.KeyValuePairs.Ukprn, 2123 },
                    { JobContextMessageConstants.KeyValuePairs.FundingFm36Output, "invalid path" },
                    { JobContextMessageConstants.KeyValuePairs.FundingFm36OutputPeriodEnd, "valid path" },
                    { JobContextMessageConstants.KeyValuePairs.Container, "container" },
                    { JobContextMessageConstants.KeyValuePairs.Filename, "filename" },
                }
            };

            var handler = mocker.Create<JobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);

            mocker.Mock<IEndpointInstance>().Verify(x => x.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()), Times.Once);

            mocker.Mock<ITelemetry>().Verify(x => x.TrackEvent("Sent All ProcessLearnerCommand Messages", It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, double>>()));
        }

        [Test]
        public async Task HandlesSubmissionTopicPointerIncorrect()
        {
            var jobContextMessage = new JobContextMessage
            {
                JobId = 1,
                TopicPointer = 5,
                Topics = new List<ITopicItem>
                {
                    new TopicItem
                    {
                        SubscriptionName = "GenerateFM36Payments",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{ JobContextMessageConstants.Tasks.JobSuccess }
                            }
                        }
                    },
                    new TopicItem
                    {
                        SubscriptionName = "Other Task",
                        Tasks = new List<ITaskItem>
                        {
                            new TaskItem
                            {
                                SupportsParallelExecution = false,
                                Tasks = new List<string>{"Something else"}
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object> {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 },
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 },
                    { JobContextMessageConstants.KeyValuePairs.Ukprn, 2123 },
                    { JobContextMessageConstants.KeyValuePairs.FundingFm36Output, "invalid path" },
                    { JobContextMessageConstants.KeyValuePairs.FundingFm36OutputPeriodEnd, "valid path" },
                    { JobContextMessageConstants.KeyValuePairs.Container, "container" },
                    { JobContextMessageConstants.KeyValuePairs.Filename, "filename" },
                }
            };

            var handler = mocker.Create<JobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);

            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()), Times.Never);
        }
    }
}