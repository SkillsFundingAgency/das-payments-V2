using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Config;
using ESFA.DC.FileService.Config.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Queueing.Interface.Configuration;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class DcHelper : IDcHelper
    {
        private readonly IJsonSerializationService serializationService;
        private readonly ITopicPublishService<JobContextDto> topicPublishingService;
        private readonly IFileService azureFileService;
        private readonly TestPaymentsDataContext dataContext;

        public DcHelper(IJsonSerializationService serializationService,
            ITopicPublishService<JobContextDto> topicPublishingService,
            IFileService azureFileService,
            TestPaymentsDataContext dataContext)
        {
            this.serializationService = serializationService ?? throw new ArgumentNullException(nameof(serializationService));
            this.topicPublishingService = topicPublishingService ?? throw new ArgumentNullException(nameof(topicPublishingService));
            this.azureFileService = azureFileService ?? throw new ArgumentNullException(nameof(azureFileService));
            this.dataContext = dataContext;
        }

        public async Task SendIlrSubmissionEvent(long ukprn, short collectionYear, byte collectionPeriod, long jobId, bool success)
        {
            try
            {
                //dataContext.ClearJobId(jobId);  //TODO: Not sure why we'd remove the job before confirming it

                var subscriptionName = DcConfiguration.SubscriptionName;

                var messagePointer = Guid.NewGuid().ToString().Replace("-", string.Empty);
                var dto = new JobContextDto
                {
                    JobId = jobId,
                    KeyValuePairs = new Dictionary<string, object>
                    {
                        {JobContextMessageKey.Filename, messagePointer},
                        {JobContextMessageKey.FundingFm36Output, messagePointer},
                        {JobContextMessageKey.UkPrn, ukprn},
                        {JobContextMessageKey.CollectionYear, collectionYear },
                        {JobContextMessageKey.ReturnPeriod, collectionPeriod },
                        {JobContextMessageKey.Username, "PV2-Automated" }
                    },
                    SubmissionDateTimeUtc = DateTime.UtcNow,
                    TopicPointer = 1,
                    Topics = new List<TopicItemDto>
                    {
                        new TopicItemDto
                        {
                            SubscriptionName = subscriptionName,
                            Tasks = new List<TaskItemDto>
                            {
                                new TaskItemDto
                                {
                                    SupportsParallelExecution = false,
                                    Tasks = new List<string>()
                                }
                            }
                        },
                        new TopicItemDto
                        {
                            SubscriptionName = subscriptionName,
                            Tasks = new List<TaskItemDto>
                            {
                                new TaskItemDto
                                {
                                    SupportsParallelExecution = false,
                                    Tasks = new List<string>
                                    {
                                        success?"JobSuccess":"JobFailure"
                                    }
                                }
                            }
                        }
                    }
                };

                await topicPublishingService.PublishAsync(dto, new Dictionary<string, object> { { "To", "GenerateFM36Payments" } }, "GenerateFM36Payments");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task SendPeriodEndTask(short collectionYear, byte collectionPeriod, long jobId, string taskName)
        {
            try
            {
                dataContext.ClearJobId(jobId);

                var dto = new JobContextDto
                {
                    JobId = jobId,
                    KeyValuePairs = new Dictionary<string, object>
                    {
                        {JobContextMessageKey.UkPrn, 0 },
                        {JobContextMessageKey.Filename, string.Empty },
                        {JobContextMessageKey.CollectionYear, collectionYear },
                        {JobContextMessageKey.ReturnPeriod, collectionPeriod },
                        {JobContextMessageKey.Username, "PV2-Automated" }
                    },
                    SubmissionDateTimeUtc = DateTime.UtcNow,
                    TopicPointer = 0,
                    Topics = new List<TopicItemDto>
                    {
                        new TopicItemDto
                        {
                            SubscriptionName = "Payments",
                            Tasks = new List<TaskItemDto>
                            {
                                new TaskItemDto
                                {
                                    SupportsParallelExecution = false,
                                    Tasks = new List<string> { taskName }
                                }
                            }
                        }
                    }
                };

                await topicPublishingService.PublishAsync(dto, new Dictionary<string, object> { { "To", "Payments" } }, $"Payments_{taskName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task SendIlrSubmission(List<FM36Learner> learners, long ukprn, short collectionYear, byte collectionPeriod, long jobId)
        {
            try
            {
                dataContext.ClearJobId(jobId);

                var messagePointer = Guid.NewGuid().ToString().Replace("-", string.Empty);
                var ilrSubmission = new FM36Global
                {
                    UKPRN = (int)ukprn,
                    Year = collectionYear.ToString(),
                    Learners = learners
                };
                var json = serializationService.Serialize(ilrSubmission);

                var storageContainer = DcConfiguration.DcBlobStorageContainer;
                var subscriptionName = DcConfiguration.SubscriptionName;

                using (var stream = await azureFileService.OpenWriteStreamAsync(messagePointer, storageContainer, new CancellationToken()))
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(json);
                }

                var dto = new JobContextDto
                {
                    JobId = jobId,
                    KeyValuePairs = new Dictionary<string, object>
                    {
                        {JobContextMessageKey.FundingFm36Output, messagePointer},
                        {JobContextMessageKey.Filename, messagePointer},
                        {JobContextMessageKey.UkPrn, ukprn},
                        {JobContextMessageKey.Container, storageContainer },
                        {JobContextMessageKey.ReturnPeriod, collectionPeriod },
                        {JobContextMessageKey.Username, "PV2-Automated" }
                    },
                    SubmissionDateTimeUtc = DateTime.UtcNow,
                    TopicPointer = 0,
                    Topics = new List<TopicItemDto>
                    {
                        new TopicItemDto
                        {
                            SubscriptionName = subscriptionName,
                            Tasks = new List<TaskItemDto>
                            {
                                new TaskItemDto
                                {
                                    SupportsParallelExecution = false,
                                    Tasks = new List<string>()
                                }
                            }
                        }
                    }
                };

                await topicPublishingService.PublishAsync(dto, new Dictionary<string, object> { { "To", "GenerateFM36Payments" } }, "GenerateFM36Payments");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void AddDcConfig(ContainerBuilder builder)
        {
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();
            builder.Register(c =>
                    new AzureStorageFileServiceConfiguration
                    { ConnectionString = DcConfiguration.DcStorageConnectionString })
                .As<IAzureStorageFileServiceConfiguration>()
                .SingleInstance();

            builder.Register(c =>
            {
                var config = c.Resolve<IAzureStorageFileServiceConfiguration>();

                return new AzureStorageFileService(config);
            }).As<IFileService>().InstancePerLifetimeScope();
            builder.Register(c => new TopicConfiguration(DcConfiguration.DcServiceBusConnectionString,
                    DcConfiguration.TopicName,
                    DcConfiguration.SubscriptionName, 10,
                    maximumCallbackTimeSpan: TimeSpan.FromMinutes(40)))
                .As<ITopicConfiguration>();

            builder.Register(c =>
            {
                var config = c.Resolve<ITopicConfiguration>();
                var serialisationService = c.Resolve<IJsonSerializationService>();
                return new TopicPublishService<JobContextDto>(config, serialisationService);
            }).As<ITopicPublishService<JobContextDto>>();
        }
    }
}
