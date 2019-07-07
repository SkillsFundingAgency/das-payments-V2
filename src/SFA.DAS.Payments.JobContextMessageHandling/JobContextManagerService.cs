using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.JobContextMessageHandling
{
    public interface IJobContextManagerService
    {
        Task RunAsync(CancellationToken cancellationToken);
    }

    public class JobContextManagerService: IJobContextManagerService
    {
        private readonly IPaymentLogger logger;
        private readonly IJobContextManager<JobContextMessage> jobContextManager;

        public JobContextManagerService(IPaymentLogger logger, IJobContextManager<JobContextMessage> jobContextManager)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.jobContextManager = jobContextManager ?? throw new ArgumentNullException(nameof(jobContextManager));
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var initialised = false;
            try
            {
                logger.LogDebug("Starting the Earning Events service.");
                jobContextManager.OpenAsync(cancellationToken);
                initialised = true;
                logger.LogInfo("Started the Period End service.");
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (Exception exception) when (!(exception is TaskCanceledException))
            {
                logger.LogError($"Error running DC job context manager. Error: {exception.Message}.", exception);
                throw;
            }
            finally
            {
                if (initialised)
                {
                    logger.LogDebug("Closing the job context manager.");
                    await jobContextManager.CloseAsync();
                    logger.LogInfo("Closed job context manager.");
                }
            }
        }
    }
}