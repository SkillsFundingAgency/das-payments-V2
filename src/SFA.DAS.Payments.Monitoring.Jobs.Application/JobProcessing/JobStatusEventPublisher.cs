using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using NServiceBus;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing
{
    public interface IJobStatusEventPublisher
    {
        Task SubmissionFinished(bool succeeded, long jobId, long ukprn, short academicYear, byte collectionPeriod, DateTime ilrSubmissionTime);
        Task PeriodEndStartFinished(bool succeeded, long jobId, short academicYear, byte collectionPeriod);
        Task PeriodEndRunFinished(bool succeeded, long jobId, short academicYear, byte collectionPeriod);
        Task PeriodEndStopFinished(bool succeeded, long jobId, short academicYear, byte collectionPeriod);
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


        public async Task PeriodEndStartFinished(bool succeeded, long jobId, short academicYear, byte collectionPeriod)
        {
            var periodEndStartFinished = succeeded ? (PeriodEndJobFinishedEvent) new PeriodEndStartJobSucceeded() : new PeriodEndStartJobFailed();
            periodEndStartFinished.JobId = jobId;
            periodEndStartFinished.CollectionPeriod = collectionPeriod;
            periodEndStartFinished.CollectionYear = academicYear;
    
            logger.LogDebug($"Publishing {periodEndStartFinished.GetType().Name} event. Event: {periodEndStartFinished.ToJson()}");
            var endpointInstance = await factory.GetEndpointInstance();
            await endpointInstance.Publish(periodEndStartFinished).ConfigureAwait(false);
        }

        public async Task PeriodEndRunFinished(bool succeeded, long jobId, short academicYear, byte collectionPeriod)
        {
            var periodEndRunFinished = succeeded ? (PeriodEndJobFinishedEvent) new PeriodEndRunJobSucceeded() : new PeriodEndRunJobFailed();
            periodEndRunFinished.JobId = jobId;
            periodEndRunFinished.CollectionPeriod = collectionPeriod;
            periodEndRunFinished.CollectionYear = academicYear;
    
            logger.LogDebug($"Publishing {periodEndRunFinished.GetType().Name} event. Event: {periodEndRunFinished.ToJson()}");
            var endpointInstance = await factory.GetEndpointInstance();
            await endpointInstance.Publish(periodEndRunFinished).ConfigureAwait(false);
        }

        
        public async Task PeriodEndStopFinished(bool succeeded, long jobId, short academicYear, byte collectionPeriod)
        {
            var periodEndStopFinished = succeeded ? (PeriodEndJobFinishedEvent) new PeriodEndStopJobSucceeded() : new PeriodEndStopJobFailed();
            periodEndStopFinished.JobId = jobId;
            periodEndStopFinished.CollectionPeriod = collectionPeriod;
            periodEndStopFinished.CollectionYear = academicYear;
    
            logger.LogDebug($"Publishing {periodEndStopFinished.GetType().Name} event. Event: {periodEndStopFinished.ToJson()}");
            var endpointInstance = await factory.GetEndpointInstance();
            await endpointInstance.Publish(periodEndStopFinished).ConfigureAwait(false);
        }


    }
}