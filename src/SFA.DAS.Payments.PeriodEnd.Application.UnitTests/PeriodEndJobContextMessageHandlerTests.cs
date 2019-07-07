using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using Moq;
using NServiceBus;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Payments.Application.Messaging;
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
        }

        [Test]
        public async Task Publishes_Period_End_Started_Event_From_Period_End_Start_Task()
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
                                Tasks = new List<string>{"PeriodEndStart"}
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object> {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 },
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 } }
            };
            
            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.Is<PeriodEndStartedEvent>(startedEvent => startedEvent.JobId == 1
                                                                                    && startedEvent.CollectionPeriod.Period == 10
                                                                                    && startedEvent.CollectionPeriod.AcademicYear == 1819),
                    It.IsAny<PublishOptions>()), Times.Once);
        }

        [Test]
        public async Task Publishes_Period_End_Run_Event_From_Period_End_Run_Task()
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
                                Tasks = new List<string>{"PeriodEndRun"}
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object> {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 },
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 } }
            };

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.Is<PeriodEndRunningEvent>(startedEvent => startedEvent.JobId == 1
                                                                                    && startedEvent.CollectionPeriod.Period == 10
                                                                                    && startedEvent.CollectionPeriod.AcademicYear == 1819),
                    It.IsAny<PublishOptions>()), Times.Once);
        }

        [Test]
        public async Task Publishes_Period_End_Stopped_Event_From_Period_End_Stop_Task()
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

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            await handler.HandleAsync(jobContextMessage, CancellationToken.None);
            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.Is<PeriodEndStoppedEvent>(startedEvent => startedEvent.JobId == 1
                                                                                    && startedEvent.CollectionPeriod.Period == 10
                                                                                    && startedEvent.CollectionPeriod.AcademicYear == 1819),
                    It.IsAny<PublishOptions>()), Times.Once);
        }

        [Test]
        public async Task Fails_If_No_Task_Name()
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
                                Tasks = new List<string>( )
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object> {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 },
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 } }
            };

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(jobContextMessage, CancellationToken.None));
        }

        [Test]
        public async Task Fails_If_Invalid_Task_Name()
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
                                Tasks = new List<string>{"Some Task"}
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object> {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 },
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 } }
            };
            
            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(jobContextMessage, CancellationToken.None));
        }

        [Test]
        public async Task Fails_If_No_Return_Period()
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
                                Tasks = new List<string>{"Some Task"}
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object> {
                    { JobContextMessageConstants.KeyValuePairs.CollectionYear, 1819 }
                }
            };

            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(jobContextMessage, CancellationToken.None));
        }

        [Test]
        public async Task Fails_If_No_Collection_Year()
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
                                Tasks = new List<string>{"Some Task"}
                            }
                        }
                    }
                },
                KeyValuePairs = new Dictionary<string, object> {
                    { JobContextMessageConstants.KeyValuePairs.ReturnPeriod, 10 }
                }
            };
            
            var handler = mocker.Create<PeriodEndJobContextMessageHandler>();
            Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(jobContextMessage, CancellationToken.None));
        }

    }
}