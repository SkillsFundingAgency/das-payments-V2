using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
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
        private readonly IStreamableKeyValuePersistenceService azureStorageService;
        private readonly ITopicPublishService<JobContextDto> topicPublishingService;

        public DcHelper(IContainer container)
        {
            azureStorageService = container.Resolve<IStreamableKeyValuePersistenceService>();
            serializationService = container.Resolve<IJsonSerializationService>();
            topicPublishingService = container.Resolve<ITopicPublishService<JobContextDto>>();
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

                using (var stream = new MemoryStream())
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(json);

                    writer.Flush();
                    stream.Position = 0;

                    Console.WriteLine($"ILR Submission: {json}");
                    await azureStorageService
                        .SaveAsync(messagePointer, stream)
                        .ConfigureAwait(true);
                }

                var dto = new JobContextDto
                {
                    JobId = 1,
                    KeyValuePairs = new Dictionary<string, object>
                    {
                        {"FundingFm36Output", messagePointer},
                        {"Filename", "blah blah"},
                        {"UkPrn", ukprn},
                        {"Username", "Bob"}
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
                    new AzureStorageKeyValuePersistenceConfig(DcConfiguration.AzureStorageConnectionString,
                        DcConfiguration.DcBlobStorageContainer)).As<IAzureStorageKeyValuePersistenceServiceConfig>()
                .SingleInstance();

            builder.RegisterType<AzureStorageKeyValuePersistenceService>()
                .Keyed<IKeyValuePersistenceService>(0)
                .As<IStreamableKeyValuePersistenceService>().InstancePerLifetimeScope();

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
