using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Serialization.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.Handlers
{
    public class JobContextMessageHandler : IMessageHandler<JobContextMessage>
    {
        private readonly IPaymentLogger logger;
        private readonly IFileService azureFileService;
        private readonly IJsonSerializationService serializationService;
        private readonly IEndpointInstanceFactory factory;
        private readonly IEarningsJobClientFactory jobClientFactory;
        private readonly ITelemetry telemetry;
        private readonly IBulkWriter<SubmittedLearnerAimModel> submittedAimWriter;
        private readonly SubmittedLearnerAimBuilder submittedLearnerAimBuilder;

        public JobContextMessageHandler(IPaymentLogger logger,
            IFileService azureFileService,
            IJsonSerializationService serializationService,
            IEndpointInstanceFactory factory,
            IEarningsJobClientFactory jobClientFactory,
            ITelemetry telemetry, 
            IBulkWriter<SubmittedLearnerAimModel> submittedAimWriter, 
            SubmittedLearnerAimBuilder submittedLearnerAimBuilder)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.azureFileService = azureFileService ?? throw new ArgumentNullException(nameof(azureFileService));
            this.serializationService = serializationService ?? throw new ArgumentNullException(nameof(serializationService));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.jobClientFactory = jobClientFactory ?? throw new ArgumentNullException(nameof(jobClientFactory));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            this.submittedAimWriter = submittedAimWriter;
            this.submittedLearnerAimBuilder = submittedLearnerAimBuilder;
        }


        public async Task<bool> HandleAsync(JobContextMessage message, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Processing Earning Event Service event for Job Id : {message.JobId}");
            try
            {
                using (var operation = telemetry.StartOperation("FM36Processing"))
                {
                    var collectionPeriod = int.Parse(message.KeyValuePairs[JobContextMessageKey.ReturnPeriod].ToString());
                    var fm36Output = await GetFm36Global(message, collectionPeriod, cancellationToken).ConfigureAwait(false);
                    var duration = await ProcessFm36Global(message, collectionPeriod, fm36Output, cancellationToken).ConfigureAwait(false);
                    await SendReceivedEarningsEvent(message.JobId, message.SubmissionDateTimeUtc, fm36Output.Year, collectionPeriod, fm36Output.UKPRN).ConfigureAwait(false);

                    telemetry.TrackEvent("Sent All ProcessLearnerCommand Messages",
                        new Dictionary<string, string>
                        {
                            { TelemetryKeys.CollectionPeriod, collectionPeriod.ToString()},
                            { TelemetryKeys.AcademicYear, fm36Output.Year},
                            { TelemetryKeys.ExternalJobId, message.JobId.ToString()},
                            { TelemetryKeys.Ukprn, fm36Output.UKPRN.ToString()},
                        },
                        new Dictionary<string, double>
                        {
                            { TelemetryKeys.Duration, duration}
                        });
                    telemetry.StopOperation(operation);
                    logger.LogInfo($"Successfully processed ILR Submission. Job Id: {message.JobId}, Ukprn: {fm36Output.UKPRN}, Submission Time: {message.SubmissionDateTimeUtc}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error while handling EarningService event", ex);
                throw;
            }
        }

        private async Task SendReceivedEarningsEvent(long jobId, DateTime ilrSubmissionDateTime, string academicYear, int collectionPeriod, long ukprn)
        {
            var message = new ReceivedProviderEarningsEvent
            {
                JobId = jobId,
                IlrSubmissionDateTime = ilrSubmissionDateTime,
                CollectionPeriod = new CollectionPeriod {AcademicYear = short.Parse(academicYear), Period = (byte) collectionPeriod},
                Ukprn = ukprn,
                EventTime = DateTimeOffset.UtcNow
            };

            var endpointInstance = await factory.GetEndpointInstance().ConfigureAwait(false);
            await endpointInstance.Publish(message).ConfigureAwait(false);
        }

        private async Task<FM36Global> GetFm36Global(JobContextMessage message, int collectionPeriod, CancellationToken cancellationToken )
        {
            FM36Global fm36Output;
            var fileReference = message.KeyValuePairs[JobContextMessageKey.FundingFm36Output].ToString();
            var container = message.KeyValuePairs[JobContextMessageKey.Container].ToString();
            logger.LogDebug($"Deserialising FM36Output for job: {message.JobId}, using file reference: {fileReference}, container: {container}");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var stream = await azureFileService.OpenReadStreamAsync(
                fileReference,
                container,
                cancellationToken))
            {
                fm36Output = serializationService.Deserialize<FM36Global>(stream);
            }
            stopwatch.Stop();
            logger.LogDebug($"Finished getting FM36Output for Job: {message.JobId}, took {stopwatch.ElapsedMilliseconds}ms.");
            telemetry.TrackEvent("Deserialize FM36Global",
                new Dictionary<string, string>
                {
                    { TelemetryKeys.CollectionPeriod, collectionPeriod.ToString()},
                    { TelemetryKeys.AcademicYear, fm36Output.Year},
                    { TelemetryKeys.ExternalJobId, message.JobId.ToString()},
                    { TelemetryKeys.Ukprn, fm36Output.UKPRN.ToString()},
                },
                new Dictionary<string, double>
                {
                    { TelemetryKeys.Duration, stopwatch.ElapsedMilliseconds}
                });
            return fm36Output;
        }

        private static ProcessLearnerCommand Build(FM36Learner learner, long jobId, DateTime ilrSubmissionDateTime, short academicYear, int collectionPeriod, long ukprn)
        {
            return new ProcessLearnerCommand
            {
                JobId = jobId,
                Learner = learner,
                RequestTime = DateTimeOffset.UtcNow,
                IlrSubmissionDateTime = ilrSubmissionDateTime,
                CollectionYear = academicYear,
                CollectionPeriod = collectionPeriod,
                Ukprn = ukprn
            };
        }

        private async Task<double> ProcessFm36Global(JobContextMessage message, int collectionPeriod, FM36Global fm36Output, CancellationToken cancellationToken)
        {
            logger.LogVerbose("Now building commands.");
            var startTime = DateTimeOffset.UtcNow;
            var commands = fm36Output
                .Learners
                .Select(learner => Build(learner, message.JobId, message.SubmissionDateTimeUtc, short.Parse(fm36Output.Year), collectionPeriod, fm36Output.UKPRN))
                .ToList();

            var jobStatusClient = jobClientFactory.Create();
            var messageName = typeof(ProcessLearnerCommand).FullName;
            logger.LogVerbose($"Now sending the start job command for job: {message.JobId}");
            await jobStatusClient.StartJob(message.JobId, fm36Output.UKPRN, message.SubmissionDateTimeUtc, short.Parse(fm36Output.Year), (byte)collectionPeriod,
                commands.Select(cmd => new GeneratedMessage { StartTime = startTime, MessageId = cmd.CommandId, MessageName = messageName }).ToList(), startTime);
            logger.LogDebug($"Now sending the process learner commands for job: {message.JobId}");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var endpointInstance = await factory.GetEndpointInstance();
            foreach (var learnerCommand in commands)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        logger.LogWarning($"Cancellation requested, will now stop sending learners for job: {message.JobId}");
                        return stopwatch.ElapsedMilliseconds;
                    }

                    await endpointInstance.SendLocal(learnerCommand).ConfigureAwait(false);

                    var aims = submittedLearnerAimBuilder.Build(learnerCommand);
                    await Task.WhenAll(aims.Select(aim => submittedAimWriter.Write(aim, cancellationToken))).ConfigureAwait(false);

                    logger.LogVerbose($"Successfully sent ProcessLearnerCommand JobId: {learnerCommand.JobId}, Ukprn: {fm36Output.UKPRN}, LearnRefNumber: {learnerCommand.Learner.LearnRefNumber}, SubmissionTime: {message.SubmissionDateTimeUtc}, Collection Year: {fm36Output.Year}, Collection period: {collectionPeriod}");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error sending the command: ProcessLearnerCommand. Job Id: {message.JobId}, Ukprn: {fm36Output.UKPRN}, Error: {ex.Message}", ex);
                    throw;
                }
            }

            await submittedAimWriter.Flush(cancellationToken).ConfigureAwait(false);

            stopwatch.Stop();
            logger.LogDebug($"Took {stopwatch.ElapsedMilliseconds}ms to send {commands.Count} Process Learner Commands for Job: {message.JobId}");
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
