using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using NServiceBus;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing
{
    public interface IJobStatusEventPublisher
    {
        Task SubmissionFinished(bool succeeded, long jobId, long ukprn, short academicYear, byte collectionPeriod, DateTime ilrSubmissionTime);
        Task PeriodEndJobFinished(JobModel jobModel, bool succeeded);
    }

    public class JobStatusEventPublisher : IJobStatusEventPublisher
    {
        private readonly IEndpointInstanceFactory factory;
        private readonly IPaymentLogger logger;

        public JobStatusEventPublisher(IEndpointInstanceFactory factory, IPaymentLogger logger)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SubmissionFinished(bool succeeded, long jobId, long ukprn, short academicYear, byte collectionPeriod, DateTime ilrSubmissionTime)
        {
            var submissionJobFinished = succeeded ? (SubmissionJobFinishedEvent)new SubmissionJobSucceeded() : new SubmissionJobFailed();
            submissionJobFinished.JobId = jobId;
            submissionJobFinished.CollectionPeriod = collectionPeriod;
            submissionJobFinished.Ukprn = ukprn;
            submissionJobFinished.AcademicYear = academicYear;
            submissionJobFinished.IlrSubmissionDateTime = ilrSubmissionTime;
            logger.LogDebug($"Publishing {submissionJobFinished.GetType().Name} event. Event: {submissionJobFinished.ToJson()}");
            var endpointInstance = await factory.GetEndpointInstance();
            await endpointInstance.Publish(submissionJobFinished).ConfigureAwait(false);
        }


        public async Task PeriodEndJobFinished(JobModel jobModel, bool succeeded)
        {
            var periodEndEvent = CreatePeriodEndEvent(jobModel.JobType, succeeded);
            periodEndEvent.JobId = jobModel.DcJobId.Value;
            periodEndEvent.CollectionPeriod = jobModel.CollectionPeriod;
            periodEndEvent.AcademicYear = jobModel.AcademicYear;

            logger.LogDebug(
                $"Publishing {periodEndEvent.GetType().Name} event. Event: {periodEndEvent.ToJson()}");
            var endpointInstance = await factory.GetEndpointInstance();
            await endpointInstance.Publish(periodEndEvent).ConfigureAwait(false);

        }

        private PeriodEndJobFinishedEvent CreatePeriodEndEvent(JobType jobType, bool succeeded)
        {
                if (jobType  == JobType.PeriodEndStartJob)
                    return succeeded ? (PeriodEndJobFinishedEvent) new PeriodEndStartJobSucceeded() : new PeriodEndStartJobFailed();
                if (jobType  == JobType.PeriodEndRunJob)
                    return succeeded ? (PeriodEndJobFinishedEvent) new PeriodEndRunJobSucceeded() : new PeriodEndRunJobFailed();
                if (jobType  == JobType.PeriodEndStopJob)
                    return succeeded ? (PeriodEndJobFinishedEvent) new PeriodEndStopJobSucceeded() : new PeriodEndStopJobFailed();
                throw new InvalidOperationException($"Unhandled period end job type: {jobType}");
        }


    }
}