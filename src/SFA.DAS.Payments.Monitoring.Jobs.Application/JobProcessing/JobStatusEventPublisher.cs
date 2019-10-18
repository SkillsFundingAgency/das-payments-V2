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
    }

    public class JobStatusEventPublisher: IJobStatusEventPublisher
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
            var submissionJobFinished = new SubmissionJobFinished
            {
                JobId = jobId,
                CollectionPeriod = collectionPeriod,
                Ukprn = ukprn,
                AcademicYear = academicYear,
                IlrSubmissionDateTime = ilrSubmissionTime,
                Succeeded = succeeded
            };
            logger.LogDebug($"Publishing SubmissionJobFinished event. Event: {submissionJobFinished.ToJson()}");
            var endpointInstance = await factory.GetEndpointInstance();
            await endpointInstance.Publish(submissionJobFinished).ConfigureAwait(false);
        }
    }
}