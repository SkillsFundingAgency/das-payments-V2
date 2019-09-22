using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.JobContextMessageHandling.Infrastructure;
using SFA.DAS.Payments.JobContextMessageHandling.JobStatus;
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
        private readonly ISubmittedLearnerAimBuilder submittedLearnerAimBuilder;
        private readonly ISubmittedLearnerAimRepository submittedLearnerAimRepository;
        private readonly IJobStatusService jobStatusService;

        public JobContextMessageHandler(IPaymentLogger logger,
            IFileService azureFileService,
            IJsonSerializationService serializationService,
            IEndpointInstanceFactory factory,
            IEarningsJobClientFactory jobClientFactory,
            ITelemetry telemetry,
            IBulkWriter<SubmittedLearnerAimModel> submittedAimWriter,
            ISubmittedLearnerAimBuilder submittedLearnerAimBuilder,
            ISubmittedLearnerAimRepository submittedLearnerAimRepository,
            IJobStatusService jobStatusService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.azureFileService = azureFileService ?? throw new ArgumentNullException(nameof(azureFileService));
            this.serializationService = serializationService ?? throw new ArgumentNullException(nameof(serializationService));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.jobClientFactory = jobClientFactory ?? throw new ArgumentNullException(nameof(jobClientFactory));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            this.submittedAimWriter = submittedAimWriter;
            this.submittedLearnerAimBuilder = submittedLearnerAimBuilder;
            this.submittedLearnerAimRepository = submittedLearnerAimRepository;
            this.jobStatusService = jobStatusService;
        }


        public async Task<bool> HandleAsync(JobContextMessage message, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Processing Earning Event Service event for Job Id : {message.JobId}");
            try
            {
                if (await HandleSubmissionEvents(message)) return true;

                using (var operation = telemetry.StartOperation($"FM36Processing:{message.JobId}"))
                {
                    var stopwatch = Stopwatch.StartNew();
                    if (await jobStatusService.JobCurrentlyRunning(message.JobId))
                    {
                        logger.LogWarning($"Job {message.JobId} is already running.");
                        return false;
                    }

                    var collectionPeriod =
                        int.Parse(message.KeyValuePairs[JobContextMessageKey.ReturnPeriod].ToString());
                    var fileName = message.KeyValuePairs[JobContextMessageKey.Filename]?.ToString();
                    var fm36Output = await GetFm36Global(message, collectionPeriod, cancellationToken)
                        .ConfigureAwait(false);

                    if (fm36Output == null)
                    {
                        return true;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    await ClearSubmittedLearnerAims(collectionPeriod, fm36Output.Year, message.SubmissionDateTimeUtc,
                        fm36Output.UKPRN, cancellationToken).ConfigureAwait(false);

                    cancellationToken.ThrowIfCancellationRequested();

                    await ProcessFm36Global(message, collectionPeriod, fm36Output, fileName, cancellationToken)
                            .ConfigureAwait(false);
                    await SendReceivedEarningsEvent(message.JobId, message.SubmissionDateTimeUtc, fm36Output.Year,
                        collectionPeriod, fm36Output.UKPRN).ConfigureAwait(false);
                    stopwatch.Stop();
                    var duration = stopwatch.ElapsedMilliseconds;
                    telemetry.TrackEvent("Processed ILR Submission",
                        new Dictionary<string, string>
                        {
                            {TelemetryKeys.Count, fm36Output.Learners.Count.ToString()},
                            {TelemetryKeys.CollectionPeriod, collectionPeriod.ToString()},
                            {TelemetryKeys.AcademicYear, fm36Output.Year},
                            {TelemetryKeys.JobId, message.JobId.ToString()},
                            {TelemetryKeys.Ukprn, fm36Output.UKPRN.ToString()},
                        },
                        new Dictionary<string, double>
                        {
                            {TelemetryKeys.Duration, duration},
                            {TelemetryKeys.Count, fm36Output.Learners.Count},
                        });

                    if (cancellationToken.IsCancellationRequested)
                    {
                        logger.LogError($"Job {message.JobId} has been cancelled after job has started processing. Ukprn: {fm36Output.UKPRN}");
                        return false;
                    }

                    telemetry.StopOperation(operation);
                    if (fm36Output.Learners.Count == 0)
                    {
                        logger.LogWarning($"Received ILR with 0 FM36 learners. Ukprn: {fm36Output.UKPRN}, job id: {message.JobId}.");
                        return true;
                    }

                    if (await jobStatusService.WaitForJobToFinish(message.JobId, cancellationToken))
                    {
                        logger.LogInfo(
                            $"Successfully processed ILR Submission. Job Id: {message.JobId}, Ukprn: {fm36Output.UKPRN}, Submission Time: {message.SubmissionDateTimeUtc}");
                        return true;
                    }
                    logger.LogError($"Job failed to finished within the allocated time. Job Id: {message.JobId}");
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogError($"Cancellation token cancelled for job: {message.JobId}");
                return false;
            }
            catch (Exception ex)
            {
                logger.LogFatal($"Error while handling EarningService event.  Error: {ex.Message}", ex);
                return false; //TODO: change back to throw when DC code is a little more defensive
            }
        }

        private async Task<bool> HandleSubmissionEvents(JobContextMessage message)
        {
            if (message.Topics != null && message.Topics.Any())
            {
                if (message.TopicPointer > message.Topics.Count - 1)
                {
                    logger.LogError(
                        $"Topic Pointer points outside the number of items in the collection of Topics. JobId: {message.JobId}");
                    return true;
                }

                var subscriptionMessage = message.Topics[message.TopicPointer];

                if (subscriptionMessage != null && subscriptionMessage.Tasks.Any())
                {
                    if (subscriptionMessage.Tasks.Any(t => t.Tasks.Contains(SubmissionJob.JobSuccess)))
                    {
                        await HandleSubmissionEvent<SubmissionSucceededEvent>(message);
                        return true;
                    }

                    if (subscriptionMessage.Tasks.Any(t => t.Tasks.Contains(SubmissionJob.JobFailure)))
                    {
                        await HandleSubmissionEvent<SubmissionFailedEvent>(message);
                        return true;
                    }
                }
            }

            return false;
        }

        private async Task HandleSubmissionEvent<T>(JobContextMessage message) where T : SubmissionEvent, new()
        {
            var eventType = typeof(T).FullName;

            using (var operation = telemetry.StartOperation(eventType))
            {

                var submissionEvent = GetSubmissionEvent<T>(message);
                await SendSubmissionEvent(submissionEvent).ConfigureAwait(false);

                telemetry.TrackEvent($"Sent {eventType}",
                    new Dictionary<string, string>
                    {
                        { TelemetryKeys.CollectionPeriod, submissionEvent.CollectionPeriod.ToString()},
                        { TelemetryKeys.AcademicYear, submissionEvent.AcademicYear.ToString()},
                        { TelemetryKeys.JobId, submissionEvent.JobId.ToString()},
                        { TelemetryKeys.Ukprn, submissionEvent.Ukprn.ToString()},
                    },
                    new Dictionary<string, double>
                    {
                        { TelemetryKeys.Duration, 0}
                    });
                telemetry.StopOperation(operation);
                logger.LogInfo($"Successfully sent {eventType}. Job Id: {message.JobId}, Ukprn: {submissionEvent.Ukprn}, Submission Time: {submissionEvent.IlrSubmissionDateTime}");
            }
        }

        private static SubmissionEvent GetSubmissionEvent<T>(IJobContextMessage message) where T : SubmissionEvent, new()
        {
            return new T
            {
                AcademicYear = short.Parse(message.KeyValuePairs[JobContextMessageKey.CollectionYear].ToString()),
                CollectionPeriod = byte.Parse(message.KeyValuePairs[JobContextMessageKey.ReturnPeriod].ToString()),
                IlrSubmissionDateTime = message.SubmissionDateTimeUtc,
                JobId = message.JobId,
                Ukprn = long.Parse(message.KeyValuePairs[JobContextMessageKey.UkPrn].ToString()),
                EventTime = DateTimeOffset.UtcNow
            };

        }

        private async Task ClearSubmittedLearnerAims(int period, string academicYear, DateTime newIlrSubmissionDateTime, long ukprn, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Deleting aims for UKPRN {ukprn} {academicYear}-{period} submitted before {newIlrSubmissionDateTime}");

            var records = await submittedLearnerAimRepository.DeletePreviouslySubmittedAims(ukprn, (byte)period, short.Parse(academicYear), newIlrSubmissionDateTime, cancellationToken).ConfigureAwait(false);

            logger.LogInfo($"Deleted {records} aims for UKPRN {ukprn} {academicYear}-{period} submitted before {newIlrSubmissionDateTime}");
        }

        private async Task SendReceivedEarningsEvent(long jobId, DateTime ilrSubmissionDateTime, string academicYear, int collectionPeriod, long ukprn)
        {
            var message = new ReceivedProviderEarningsEvent
            {
                JobId = jobId,
                IlrSubmissionDateTime = ilrSubmissionDateTime,
                CollectionPeriod = new CollectionPeriod { AcademicYear = short.Parse(academicYear), Period = (byte)collectionPeriod },
                Ukprn = ukprn,
                EventTime = DateTimeOffset.UtcNow
            };

            var endpointInstance = await factory.GetEndpointInstance().ConfigureAwait(false);
            await endpointInstance.Publish(message).ConfigureAwait(false);
        }

        private async Task SendSubmissionEvent(SubmissionEvent submissionEvent)
        {
            var endpointInstance = await factory.GetEndpointInstance().ConfigureAwait(false);
            await endpointInstance.Publish(submissionEvent).ConfigureAwait(false);
        }

        private async Task<FM36Global> GetFm36Global(JobContextMessage message, int collectionPeriod, CancellationToken cancellationToken)
        {
            FM36Global fm36Output;
            var fileReference = message.Topics.Any(topic => topic.Tasks.Any(task => task.Tasks.Any(taskName => taskName.Equals(JobContextMessageConstants.Tasks.ProcessPeriodEndSubmission))))
                ? message.KeyValuePairs[JobContextMessageConstants.KeyValuePairs.FundingFm36OutputPeriodEnd].ToString()
                : message.KeyValuePairs[JobContextMessageConstants.KeyValuePairs.FundingFm36Output].ToString();
            var container = message.KeyValuePairs[JobContextMessageConstants.KeyValuePairs.Container].ToString();
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

            if (fm36Output == null)
            {
                logger.LogWarning($"No FM36Global data found for job: {message.JobId}, file reference: {fileReference}, container: {container}");

                telemetry.TrackEvent("Failed To Deserialise FM36Global",
                    new Dictionary<string, string>
                    {
                        { TelemetryKeys.CollectionPeriod, collectionPeriod.ToString()},
                        { TelemetryKeys.JobId, message.JobId.ToString()},
                    },
                    new Dictionary<string, double>
                    {
                        { TelemetryKeys.Duration, stopwatch.ElapsedMilliseconds}
                    });

                return null;
            }

            telemetry.TrackEvent("Deserialised FM36Global",
                new Dictionary<string, string>
                {
                    { TelemetryKeys.CollectionPeriod, collectionPeriod.ToString()},
                    { TelemetryKeys.AcademicYear, fm36Output.Year},
                    { TelemetryKeys.JobId, message.JobId.ToString()},
                    { TelemetryKeys.Ukprn, fm36Output.UKPRN.ToString()},
                },
                new Dictionary<string, double>
                {
                    { TelemetryKeys.Duration, stopwatch.ElapsedMilliseconds}
                });

            if (fm36Output.UKPRN == 0)
            {
                logger.LogWarning($"FM36LGlobal for job: {message.JobId}, file reference: {fileReference}, container: {container} contains no Ukprn property");
                return null;
            }

            if (string.IsNullOrWhiteSpace(fm36Output.Year))
            {
                logger.LogWarning($"FM36LGlobal for job: {message.JobId}, file reference: {fileReference}, container: {container} contains no Year property");
                return null;
            }

            if (fm36Output.Learners == null)
            {
                fm36Output.Learners = new List<FM36Learner>();
            }


            return fm36Output;
        }

        private static ProcessLearnerCommand Build(FM36Learner learner, long jobId, DateTime ilrSubmissionDateTime, short academicYear, int collectionPeriod, long ukprn, string fileName)
        {
            return new ProcessLearnerCommand
            {
                JobId = jobId,
                Learner = learner,
                RequestTime = DateTimeOffset.UtcNow,
                IlrSubmissionDateTime = ilrSubmissionDateTime,
                IlrFileName = fileName,
                CollectionYear = academicYear,
                CollectionPeriod = collectionPeriod,
                Ukprn = ukprn
            };
        }

        private async Task<double> ProcessFm36Global(JobContextMessage message, int collectionPeriod, FM36Global fm36Output, string ilrFileName, CancellationToken cancellationToken)
        {
            logger.LogVerbose("Now building commands.");
            var startTime = DateTimeOffset.UtcNow;
            var learners = fm36Output.Learners ?? new List<FM36Learner>();
            var commands = learners
                .Select(learner => Build(learner, message.JobId, message.SubmissionDateTimeUtc, short.Parse(fm36Output.Year), collectionPeriod, fm36Output.UKPRN, ilrFileName))
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
            var duration = stopwatch.ElapsedMilliseconds;
            telemetry.TrackEvent("Sent All ProcessLearnerCommand Messages",
                new Dictionary<string, string>
                {
                    {TelemetryKeys.Count, fm36Output.Learners.Count.ToString()},
                    {TelemetryKeys.CollectionPeriod, collectionPeriod.ToString()},
                    {TelemetryKeys.AcademicYear, fm36Output.Year},
                    {TelemetryKeys.JobId, message.JobId.ToString()},
                    {TelemetryKeys.Ukprn, fm36Output.UKPRN.ToString()},
                },
                new Dictionary<string, double>
                {
                    {TelemetryKeys.Duration, duration},
                    {TelemetryKeys.Count, fm36Output.Learners.Count},
                });
            logger.LogDebug($"Took {stopwatch.ElapsedMilliseconds}ms to send {commands.Count} Process Learner Commands for Job: {message.JobId}");
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
