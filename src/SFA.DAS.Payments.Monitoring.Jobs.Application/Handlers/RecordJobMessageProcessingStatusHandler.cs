using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Handlers
{
    public class RecordJobMessageProcessingStatusHandler : IHandleMessages<RecordJobMessageProcessingStatus>
    {
        private readonly IPaymentLogger logger;
        private readonly IJobStepService jobStepService;
        private readonly ISqlExceptionService sqlExceptionService;
        private readonly int delayInSeconds;
        public RecordJobMessageProcessingStatusHandler(IPaymentLogger logger, IJobStepService jobStepService, IConfigurationHelper configurationHelper, ISqlExceptionService sqlExceptionService)
        {
            if (configurationHelper == null) throw new ArgumentNullException(nameof(configurationHelper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.jobStepService = jobStepService ?? throw new ArgumentNullException(nameof(jobStepService));
            this.sqlExceptionService = sqlExceptionService ?? throw new ArgumentNullException(nameof(sqlExceptionService));
            delayInSeconds = int.Parse(configurationHelper.GetSettingOrDefault("DelayedRetryTimeInSeconds", "5"));
        }

        public async Task Handle(RecordJobMessageProcessingStatus message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogVerbose(
                    $"Handling job message processed. DC Job Id: {message.JobId}, message name: {message.MessageName}, id: {message.Id}");
                await jobStepService.JobStepCompleted(message);
                logger.LogDebug(
                    $"Finished handling job message processed. DC Job Id: {message.JobId}, message name: {message.MessageName}, id: {message.Id}");
            }
            catch (DbUpdateException updateEx)
            {
                if (!sqlExceptionService.IsConstraintViolation(updateEx))
                    throw;
                logger.LogWarning($"Failed to store/update job details probably due to KEY violation for job: {message.JobId}, message id: {message.Id}, message name: {message.MessageName}. Error: {updateEx.Message}");
                var options = new SendOptions();
                options.DelayDeliveryWith(TimeSpan.FromSeconds(delayInSeconds));
                await context.Send(message, options).ConfigureAwait(false);
                context.DoNotContinueDispatchingCurrentMessageToHandlers();
                return;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error recording message processing status. Job id: {message.JobId}, message : {message.Id}, message name: {message.MessageName}. Error: {ex.Message}", ex);
                throw;
            }
        }
    }
}