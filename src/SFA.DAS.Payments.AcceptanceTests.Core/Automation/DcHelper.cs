using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class DcHelper
    {
        private readonly IJsonSerializationService serializationService;
        private readonly ITopicPublishService<JobContextDto> topicPublishingService;
        private readonly IFileService azureFileService;

        public DcHelper(IJsonSerializationService serializationService,
            ITopicPublishService<JobContextDto> topicPublishingService,
            IFileService azureFileService)
        {
            this.serializationService = serializationService ?? throw new ArgumentNullException(nameof(serializationService));
            this.topicPublishingService = topicPublishingService ?? throw new ArgumentNullException(nameof(topicPublishingService));
            this.azureFileService = azureFileService ?? throw new ArgumentNullException(nameof(azureFileService));
        }

        //TODO: Remove when integration with DC services in Test environment is working
        public async Task SendLearnerCommands(List<FM36Learner> learners, long ukprn, short collectionYear, 
            byte collectionPeriod, long jobId, DateTime ilrSubmissionTime)
        {
            try
            {
                var startTime = DateTimeOffset.UtcNow;
                var commands = learners.Select(learner => new ProcessLearnerCommand
                    {
                        JobId = jobId,
                        CollectionPeriod = collectionPeriod,
                        CollectionYear = collectionYear,
                        IlrSubmissionDateTime = ilrSubmissionTime,
                        SubmissionDate = ilrSubmissionTime,
                        Ukprn = ukprn,
                        Learner = learner
                    })
                    .ToList();
                var startedJob = new RecordStartedProcessingEarningsJob
                {
                    JobId = jobId,
                    CollectionPeriod = collectionPeriod,
                    CollectionYear = collectionYear,
                    StartTime = startTime,
                    Ukprn = ukprn,
                    IlrSubmissionTime = ilrSubmissionTime,
                    GeneratedMessages = commands.Select(command => new GeneratedMessage
                    {
                        StartTime = command.RequestTime,
                        MessageName = command.GetType().FullName,
                        MessageId = command.CommandId
                    }).ToList()
                };
                Console.WriteLine($"Sending job started message: {startedJob.ToJson()}");
                await TestSessionBase.MessageSession.Send(startedJob).ConfigureAwait(false);
                foreach (var processLearnerCommand in commands)
                {
                    await TestSessionBase.MessageSession.Send(processLearnerCommand).ConfigureAwait(false); 
                    Console.WriteLine($"sent process learner command: {processLearnerCommand.ToJson()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending the learner commands. Error: {ex.Message}");
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task SendIlrSubmission(List<FM36Learner> learners, long ukprn, short collectionYear, byte collectionPeriod, long jobId)
        {
            try
            {
                var messagePointer = Guid.NewGuid().ToString().Replace("-", string.Empty);
                var ilrSubmission = new FM36Global
                {
                    UKPRN = (int)ukprn,
                    Year = collectionYear.ToString(),
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
                    JobId = jobId,
                    KeyValuePairs = new Dictionary<string, object>
                    {
                        {JobContextMessageKey.FundingFm36Output, messagePointer},
                        {JobContextMessageKey.Filename, "blah blah"},
                        {JobContextMessageKey.UkPrn, ukprn},
                        {JobContextMessageKey.Username, "Bob"},
                        {JobContextMessageKey.Container, DcConfiguration.DcBlobStorageContainer },
                        {JobContextMessageKey.ReturnPeriod, collectionPeriod }
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
