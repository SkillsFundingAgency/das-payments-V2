﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using TechTalk.SpecFlow;
using NServiceBus;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Monitoring.AcceptanceTests.Handlers;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Jobs
{
    [Binding]
    public class JobsSteps : StepsBase
    {
        protected JobsDataContext DataContext => Scope.Resolve<JobsDataContext>();
        protected JobModel Job
        {
            get => Get<JobModel>();
            set => Set(value);
        }

        public List<GeneratedMessage> GeneratedMessages
        {
            get => Get<List<GeneratedMessage>>();
            set => Set(value);
        }

        public JobsCommand JobDetails
        {
            get => Get<JobsCommand>("job_command");
            set => Set(value,"job_command");
        }

        public JobsSteps(ScenarioContext context) : base(context)
        {
        }

        protected string PartitionEndpointName => $"sfa-das-payments-monitoring-jobs{JobDetails.JobId % 20}";


        [Given(@"the payments are for the current collection year")]
        public void GivenThePaymentsAreForTheCurrentCollectionYear()
        {
            SetCurrentCollectionYear();
        }

        [Given(@"the current collection period is R(.*)")]
        [Given(@"the current processing period is (.*)")]
        public void GivenTheCurrentProcessingPeriodIs(byte period)
        {
            CollectionPeriod = period;
        }

        [Given(@"the earnings event service has received a provider earnings job")]
        public void GivenTheEarningsEventServiceHasReceivedAProviderEarningsJob()
        {
            GeneratedMessages = new List<GeneratedMessage>
            {
                new GeneratedMessage {StartTime = DateTimeOffset.UtcNow, MessageName = "SFA.DAS.Payments.EarningEvents.Commands.Internal.ProcessLearnerCommand", MessageId = Guid.NewGuid()},
                new GeneratedMessage {StartTime = DateTimeOffset.UtcNow, MessageName = "SFA.DAS.Payments.EarningEvents.Commands.Internal.ProcessLearnerCommand", MessageId = Guid.NewGuid()},
                new GeneratedMessage {StartTime = DateTimeOffset.UtcNow, MessageName = "SFA.DAS.Payments.EarningEvents.Commands.Internal.ProcessLearnerCommand", MessageId = Guid.NewGuid()},
            };
            JobDetails = new RecordEarningsJob
            {
                JobId = TestSession.JobId,
                CollectionPeriod = CollectionPeriod,
                CollectionYear = 1819,
                Ukprn = TestSession.Ukprn,
                StartTime = DateTimeOffset.UtcNow,
                IlrSubmissionTime = DateTime.UtcNow.AddSeconds(-10),
                GeneratedMessages = GeneratedMessages,
            };

            Console.WriteLine($"Job details: {JobDetails.ToJson()}");
        }

        [Given(@"the earnings event service has received a large provider earnings job")]
        public void GivenTheEarningsEventServiceHasReceivedALargeProviderEarningsJob()
        {
            GeneratedMessages = new List<GeneratedMessage>();
            for (var i = 0; i < 1500; i++)
            {
                GeneratedMessages.Add(new GeneratedMessage { StartTime = DateTimeOffset.UtcNow, MessageName = "SFA.DAS.Payments.EarningEvents.Commands.Internal.ProcessLearnerCommand", MessageId = Guid.NewGuid() });
            }
            JobDetails = new RecordEarningsJob
            {
                JobId = TestSession.JobId,
                CollectionPeriod = CollectionPeriod,
                CollectionYear = 1819,
                Ukprn = TestSession.Ukprn,
                StartTime = DateTimeOffset.UtcNow,
                IlrSubmissionTime = DateTime.UtcNow.AddSeconds(-10),
                GeneratedMessages = GeneratedMessages,
            };
            Console.WriteLine($"Job details: {JobDetails.ToJson()}");
        }


        [Given(@"a provider earnings job has already been recorded")]
        public async Task GivenAProviderEarningsJobHasAlreadyBeenRecorded()
        {
            GeneratedMessages = new List<GeneratedMessage>
            {
                new GeneratedMessage {StartTime = DateTimeOffset.UtcNow, MessageName = "SFA.DAS.Payments.EarningEvents.Commands.Internal.ProcessLearnerCommand", MessageId = Guid.NewGuid()},
                new GeneratedMessage {StartTime = DateTimeOffset.UtcNow, MessageName = "SFA.DAS.Payments.EarningEvents.Commands.Internal.ProcessLearnerCommand", MessageId = Guid.NewGuid()},
                new GeneratedMessage {StartTime = DateTimeOffset.UtcNow, MessageName = "SFA.DAS.Payments.EarningEvents.Commands.Internal.ProcessLearnerCommand", MessageId = Guid.NewGuid()},
            };

            var earningsJob = new RecordEarningsJob
            {
                JobId = TestSession.JobId,
                CollectionPeriod = CollectionPeriod,
                CollectionYear = 1819,
                Ukprn = TestSession.Ukprn,
                StartTime = DateTimeOffset.UtcNow,
                IlrSubmissionTime = DateTime.UtcNow.AddSeconds(-10),
                GeneratedMessages = GeneratedMessages
            };
            JobDetails = earningsJob;

            Job = new JobModel
            {
                JobType = JobType.EarningsJob,
                StartTime = earningsJob.StartTime,
                CollectionPeriod = earningsJob.CollectionPeriod,
                AcademicYear = earningsJob.CollectionYear,
                Ukprn = earningsJob.Ukprn,
                DcJobId = JobDetails.JobId,
                IlrSubmissionTime = earningsJob.IlrSubmissionTime,
                Status = JobStatus.InProgress,
                LearnerCount = GeneratedMessages.Count
            };
            DataContext.Jobs.Add(Job);
            await DataContext.SaveChangesAsync();
            DataContext.JobSteps.AddRange(
                GeneratedMessages.Select(msg => 
                    new JobStepModel
                    {
                        JobId = Job.Id,
                        StartTime = msg.StartTime,
                        MessageName = msg.MessageName,
                        MessageId = msg.MessageId,
                        Status = JobStepStatus.Queued
                    }));
            await DataContext.SaveChangesAsync();
        }

        [Given(@"the period end service has received a period end start job")]
        public void GivenThePeriodEndServiceHasReceivedAPeriodEndStartJob()
        {
            CreatePeriodEndJob<RecordPeriodEndStartJob>();
        }

        private void CreatePeriodEndJob<T>() where T: RecordPeriodEndJob, new()
        {
            GeneratedMessages = new List<GeneratedMessage>
            {
                new GeneratedMessage {StartTime = DateTimeOffset.UtcNow, MessageName = typeof(RecordPeriodEndStartJob).FullName, MessageId = Guid.NewGuid()},
            };
            JobDetails = new T
            {
                JobId = TestSession.JobId,
                CollectionPeriod = CollectionPeriod,
                CollectionYear = 1819,
                StartTime = DateTimeOffset.UtcNow,
                GeneratedMessages = GeneratedMessages,
            };
            Console.WriteLine($"Job id: {TestSession.JobId}");

        }

        [Given(@"the period end service has received a period end run job")]
        public void GivenThePeriodEndServiceHasReceivedAPeriodEndRunJob()
        {
            CreatePeriodEndJob<RecordPeriodEndRunJob>();
        }

        [Given(@"the period end service has received a period end stop job")]
        public void GivenThePeriodEndServiceHasReceivedAPeriodEndStopJob()
        {
            CreatePeriodEndJob<RecordPeriodEndStopJob>();
        }

        [Given(@"the monitoring service has recorded the completion of an earnings job")]
        public async Task GivenTheMonitoringServiceHasRecordedTheCompletionOfAnEarningsJob()
        {
            GivenTheEarningsEventServiceHasReceivedAProviderEarningsJob();
            await WhenTheEarningsEventServiceNotifiesTheJobMonitoringServiceToRecordTheJob().ConfigureAwait(false);
            await WhenTheFinalMessagesForTheJobAreSuccessfullyProcessed().ConfigureAwait(false);
            await ThenTheJobMonitoringServiceShouldRecordTheJob().ConfigureAwait(false);
        }


        [When(@"the final messages for the job are successfully processed")]
        public async Task WhenTheFinalMessagesForTheJobAreSuccessfullyProcessed()
        {
            foreach (var generatedMessage in GeneratedMessages)
            {
                var message = new RecordJobMessageProcessingStatus
                {
                    JobId = JobDetails.JobId,
                    MessageName = generatedMessage.MessageName,
                    EndTime = DateTimeOffset.UtcNow,
                    Succeeded = true,
                    Id = generatedMessage.MessageId
                };
                Console.WriteLine($"Generated Message: {message.ToJson()}");
                await MessageSession.Send(PartitionEndpointName, message).ConfigureAwait(false);
            }
        }

        [When(@"the earnings event service notifies the job monitoring service to record the job")]
        public async Task WhenTheEarningsEventServiceNotifiesTheJobMonitoringServiceToRecordTheJob()
        {
            var recordEarningsJob = JobDetails as RecordEarningsJob;
            recordEarningsJob.GeneratedMessages = GeneratedMessages.Take(1000).ToList();
            await MessageSession.Send(PartitionEndpointName, JobDetails).ConfigureAwait(false);
            var skip = 1000;
            var batch = new List<GeneratedMessage>();
            while ( (batch = GeneratedMessages.Skip(skip).Take(1000).ToList()).Count>0)
            {
                await MessageSession.Send(PartitionEndpointName, new RecordEarningsJobAdditionalMessages
                {
                    GeneratedMessages = batch,
                    JobId = JobDetails.JobId,

                }).ConfigureAwait(false);
                skip += 1000;
            }
        }


        [When(@"the final messages for the job are failed to be processed")]
        public async Task WhenTheFinalMessagesForTheJobAreFailedToBeProcessed()
        {
            foreach (var generatedMessage in GeneratedMessages)
            {
                await MessageSession.Send(PartitionEndpointName, new RecordJobMessageProcessingStatus
                {
                    JobId = JobDetails.JobId,
                    MessageName = generatedMessage.MessageName,
                    EndTime = DateTimeOffset.UtcNow,
                    Succeeded = false,
                    Id = generatedMessage.MessageId
                });
            }
        }

        [When(@"Data-Collections confirms the successful completion of the job")]
        public async Task WhenData_CollectionsConfirmsTheSuccessfulCompletionOfTheJob()
        {
            var earningsJob = JobDetails as RecordEarningsJob  ?? throw new InvalidOperationException("Expected job to be a ");
            await MessageSession.Send(PartitionEndpointName, new RecordEarningsJobSucceeded
            {
                JobId = JobDetails.JobId,
                CollectionPeriod = CollectionPeriod,
                Ukprn = TestSession.Ukprn,
                AcademicYear = AcademicYear,
                IlrSubmissionDateTime = ((RecordEarningsJob)JobDetails).IlrSubmissionTime
            }).ConfigureAwait(false);
        }

        [When(@"Data-Collections confirms the failure of the job")]
        public async Task WhenData_CollectionsConfirmsTheFailureOfTheJob()
        {
            var earningsJob = JobDetails as RecordEarningsJob ?? throw new InvalidOperationException("Expected job to be a ");
            await MessageSession.Send(PartitionEndpointName, new RecordEarningsJobFailed
            {
                JobId = JobDetails.JobId,
                CollectionPeriod = CollectionPeriod,
                Ukprn = TestSession.Ukprn,
                AcademicYear = AcademicYear,
                IlrSubmissionDateTime = ((RecordEarningsJob)JobDetails).IlrSubmissionTime
            }).ConfigureAwait(false);
        }


        [Then(@"the job monitoring service should update the status of the job to show that it has completed with errors")]
        public async Task ThenTheJobMonitoringServiceShouldUpdateTheStatusOfTheJobToShowThatItHasCompletedWithErrors()
        {
            await WaitForIt(() =>
            {
                return DataContext.Jobs.Any(j => j.Id == Job.Id && j.Status == JobStatus.CompletedWithErrors);
            }, $"Status was not updated to Completed for job: {Job.Id}, Dc job id: {JobDetails.JobId}");
        }


        [Then(@"the job monitoring service should record the job")]
        public async Task ThenTheJobMonitoringServiceShouldRecordTheJob()
        {
            await WaitForIt(() =>
            {
                var job = DataContext.Jobs.AsNoTracking()
                    .FirstOrDefault(x => x.DcJobId == JobDetails.JobId);

                if (job == null)
                    return false;
                Job = job;
                Console.WriteLine($"Found job: {Job.Id}, status: {Job.Status}, start time: {job.StartTime}");
                return true;
            }, $"Failed to find job with dc job id: {JobDetails.JobId}");
        }

        [Then(@"the job monitoring service should also record the period end job messages")]
        [Then(@"the job monitoring service should also record the messages generated by earning events service")]
        public async Task ThenTheJobMonitoringServiceShouldAlsoRecordTheMessagesGeneratedByEarningEventsService()
        {
            await WaitForIt(() =>
            {
                foreach (var generatedMessage in GeneratedMessages)
                {
                    if (!DataContext.JobSteps.Any(step => step.JobId == Job.Id && step.MessageId == generatedMessage.MessageId))
                    {
                        Console.WriteLine($"Failed to find job step {generatedMessage.MessageId} for job: {Job.Id}");
                        return false;
                    }

                    Console.WriteLine($"Found job step: {generatedMessage.MessageId}");
                }
                Console.WriteLine($"Found all expected job steps for job : {Job.Id}, dc job id: {JobDetails.JobId}");
                return true;
            }, $"Failed to find the expected job steps for job: {Job.Id}");
        }

        [Then(@"the job monitoring service should update the status of the job to show that it has completed")]
        public async Task ThenTheJobMonitoringServiceShouldUpdateTheStatusOfTheJobToShowThatItHasCompleted()
        {
            await WaitForIt(() =>
            {
                var job = DataContext.Jobs.AsNoTracking()
                    .FirstOrDefault(x => x.DcJobId == JobDetails.JobId && x.Status == JobStatus.Completed);

                if (job == null)
                    return false;
                Job = job;
                Console.WriteLine($"Found job: {Job.Id}, status: {Job.Status}, start time: {job.StartTime}");
                return true;
            }, $"Failed to find job with dc job id: {JobDetails.JobId}");
        }

        [Then(@"the monitoring service should record the successful completion of the Data-Collections processes")]
        public async Task ThenTheMonitoringServiceShouldRecordTheSuccessfulCompletionOfTheJob()
        {
            await WaitForIt(() =>
            {
                var job = DataContext.Jobs.AsNoTracking()
                    .FirstOrDefault(x => x.DcJobId == JobDetails.JobId && x.DcJobSucceeded == true);

                if (job == null)
                    return false;

                Job = job;
                Console.WriteLine($"Found job: {Job.Id}, status: {Job.Status}, start time: {job.StartTime}");
                return true;
            }, $"Failed to find job with dc status completed and dc job id: {JobDetails.JobId}");
        }

        [Then(@"the monitoring service should notify other services that the job has completed successfully")]
        public async Task ThenTheMonitoringServiceShouldNotifyOtherServicesThatTheJobHasCompletedSuccessfully()
        {
            await WaitForIt(() => SubmissionJobSucceededHandler.ReceivedEvents.Any(ev => ev.JobId == JobDetails.JobId),
                $"Failed to receive the submission job succeeded event for job id: {JobDetails.JobId}").ConfigureAwait(false);
        }

        [Then(@"the monitoring service should record the failure of the Data-Collections processes")]
        public async Task ThenTheMonitoringServiceShouldRecordTheFailureOfTheData_CollectionsProcesses()
        {
            await WaitForIt(() =>
            {
                var job = DataContext.Jobs.AsNoTracking()
                    .FirstOrDefault(x => x.DcJobId == JobDetails.JobId && x.DcJobSucceeded.HasValue && !x.DcJobSucceeded.Value);

                if (job == null)
                    return false;

                Job = job;
                Console.WriteLine($"Found job: {Job.Id}, status: {Job.Status}, start time: {job.StartTime}");
                return true;
            }, $"Failed to find job with dc status failed and dc job id: {JobDetails.JobId}");
        }

        [Then(@"the monitoring service should notify other services that the job has failed")]
        public async Task ThenTheMonitoringServiceShouldNotifyOtherServicesThatTheJobHasFailed()
        {
            await WaitForIt(() => SubmissionJobFailedHandler.ReceivedEvents.Any(ev => ev.JobId == JobDetails.JobId),
                $"Failed to receive the submission job failed event for job id: {JobDetails.JobId}").ConfigureAwait(false);
        }

    }
}
