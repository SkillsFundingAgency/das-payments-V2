using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Auditing.Interface;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Mapping.Interface;
using ESFA.DC.Queueing.Interface;
using Newtonsoft.Json;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.JobContextMessageHandling
{
    public class SubmissionJobFinishedHandler : IHandleMessages<SubmissionJobFinishedEvent>
    {
        private readonly IPaymentLogger logger;


        public SubmissionJobFinishedHandler(IPaymentLogger logger, IJobContextManager<JobContextDto>())
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        public async Task Handle(SubmissionJobFinishedEvent message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Handling SubmissionJobFinished event for Ukprn: {message.Ukprn}");
            bool dasJobStatus;

            switch (message)
            {
                case SubmissionJobSucceeded _:
                    dasJobStatus = true;
                    break;
                case SubmissionJobFailed _:
                    dasJobStatus = false;
                    break;
                default:
                    throw new InvalidOperationException("Unable to resolve job status");
            }

            await ContinueProcessingJobContextMessage(dasJobStatus, message.JobId);

            logger.LogInfo($"Finished handling SubmissionJobFinished event for Ukprn: {message.Ukprn}");

        }

    }
}