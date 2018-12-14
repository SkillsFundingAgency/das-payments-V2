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

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class DcHelper
    {
        private readonly IJsonSerializationService serializationService;
        private readonly ITopicPublishService<JobContextDto> topicPublishingService;
        private readonly IFileService azureFileService;

        public DcHelper(IContainer container)
        {
            serializationService = container.Resolve<IJsonSerializationService>();
            topicPublishingService = container.Resolve<ITopicPublishService<JobContextDto>>();
            azureFileService = container.Resolve<IFileService>();
        }

        public async Task SendIlrSubmission(List<FM36Learner> learners, long ukprn, string collectionYear)
        {
            try
            {
                var messagePointer = Guid.NewGuid().ToString().Replace("-", string.Empty);
                var ilrSubmission = new FM36Global
                {
                    UKPRN = (int)ukprn,
                    Year = collectionYear,
                    Learners = learners
                };
                var json = serializationService.Serialize(ilrSubmission);

                using (var stream = await azureFileService.OpenWriteStreamAsync(messagePointer, DcConfiguration.DcBlobStorageContainer, new CancellationToken()))
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(json);
                }

                var dto = new JobContextDto
                {
                    JobId = 1,
                    KeyValuePairs = new Dictionary<string, object>
                    {
                        {JobContextMessageKey.FundingFm36Output, messagePointer},
                        {JobContextMessageKey.Filename, "blah blah"},
                        {JobContextMessageKey.UkPrn, ukprn},
                        {JobContextMessageKey.Username, "Bob"},
                        {JobContextMessageKey.Container, DcConfiguration.DcBlobStorageContainer },
                        {JobContextMessageKey.ReturnPeriod, 1 }
                    },
                    SubmissionDateTimeUtc = DateTime.UtcNow,
                    TopicPointer = 0,
                    Topics = new List<TopicItemDto>
                    {
                        new TopicItemDto
                        {
                            SubscriptionName = "Validation",
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

                await topicPublishingService.PublishAsync(dto, new Dictionary<string, object>(), "Validation");
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
                    DcConfiguration.SubscriptionName, 1,
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
