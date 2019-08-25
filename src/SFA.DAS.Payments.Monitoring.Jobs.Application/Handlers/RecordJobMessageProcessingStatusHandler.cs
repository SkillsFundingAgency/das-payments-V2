using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Exceptions;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Handlers
{
    public class RecordJobMessageProcessingStatusHandler : IHandleMessages<RecordJobMessageProcessingStatus>
    {
        private readonly IPaymentLogger logger;
        private readonly IJobMessageService jobMessageService;
        private readonly ISqlExceptionService sqlExceptionService;
        private readonly int delayInSeconds;
        public RecordJobMessageProcessingStatusHandler(IPaymentLogger logger, IJobMessageService jobMessageService, IConfigurationHelper configurationHelper, ISqlExceptionService sqlExceptionService)
        {
            if (configurationHelper == null) throw new ArgumentNullException(nameof(configurationHelper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.jobMessageService = jobMessageService ?? throw new ArgumentNullException(nameof(jobMessageService));
            this.sqlExceptionService = sqlExceptionService ?? throw new ArgumentNullException(nameof(sqlExceptionService));
            delayInSeconds = int.Parse(configurationHelper.GetSettingOrDefault("DelayedRetryTimeInSeconds", "5"));
        }

        public async Task Handle(RecordJobMessageProcessingStatus message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogVerbose(
                    $"Handling job message processed. DC Job Id: {message.JobId}, message name: {message.MessageName}, id: {message.Id}");
                await jobMessageService.JobStepCompleted(message);
                logger.LogDebug(
                    $"Finished handling job message processed. DC Job Id: {message.JobId}, message name: {message.MessageName}, id: {message.Id}");
            }
            catch (DbUpdateException updateEx)
            {
                if (!sqlExceptionService.IsConstraintViolation(updateEx))
                    throw;
                var successfullyDeferred = await context.DeferDueToUpdateException(message, delayInSeconds);
                if (!successfullyDeferred)
                    throw new InvalidOperationException($"Failed to store/update job details probably due to KEY violation for job: {message.JobId}, message id: {message.Id}, message name: {message.MessageName}. Error: {updateEx.Message}", updateEx);
                logger.LogWarning($"Failed to store/update job details probably due to KEY violation for job: {message.JobId}, message id: {message.Id}, message name: {message.MessageName}. Error: {updateEx.Message}");
            }
            catch (DcJobNotFoundException jobNotFoundException)
            {
                var successfullyDeferred = await context.DeferDueToJobNotFound(message, delayInSeconds);
                if (!successfullyDeferred)
                    throw new InvalidOperationException($"Failed to find the job. Dc Job Id: {message.JobId}, message : {message.Id}, message name: {message.MessageName}. Error: {jobNotFoundException.Message}", jobNotFoundException);
                logger.LogWarning($"Failed to store/update job details as the the message is being handled before the job creation message.  Will retry message shortly. Error: {jobNotFoundException.Message}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error recording message processing status. Job id: {message.JobId}, message : {message.Id}, message name: {message.MessageName}. Error: {ex.Message}", ex);
            }
        }
    }
}