﻿using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.DataLock;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Audit.DataLockService.Handlers
{
    public class SubmissionSucceededEventHandler : IHandleMessages<SubmissionJobSucceeded>
    {
        private readonly IPaymentLogger logger;
        private readonly ISubmissionEventProcessor submissionEventProcessor;

        public SubmissionSucceededEventHandler(ISubmissionEventProcessor submissionEventProcessor, IPaymentLogger logger)
        {
            this.submissionEventProcessor = submissionEventProcessor;
            this.logger = logger;
        }

        public async Task Handle(SubmissionJobSucceeded message, IMessageHandlerContext context)
        {
            var logString = $"{typeof(SubmissionSucceededEvent).Name}. UKPRN: {message.Ukprn} {message.AcademicYear}-R{message.CollectionPeriod:D2}, ILR Submission: {message.IlrSubmissionDateTime:s}, Job ID: {message.JobId}";

            logger.LogInfo("Handling " + logString);

            try
            {
                await submissionEventProcessor.ProcessSubmissionSucceededEvent(message).ConfigureAwait(false);
                logger.LogInfo("Finished handling " + logString);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling {logString}. {ex.Message}");
                throw;
            }
        }
    }
}
