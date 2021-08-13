using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Queueing.Interface;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.EarningEvents.Application.JobContext
{
    public class DasJobContextManager : IJobContextManager<JobContextMessage>
    {
        private readonly ITopicSubscriptionService<JobContextDto> topicSubscriptionService;
        private readonly IDasJobContextManagerService dasJobContextManagerService;
        private readonly IPaymentLogger logger;

        public DasJobContextManager(
            ITopicSubscriptionService<JobContextDto> topicSubscriptionService,
            IDasJobContextManagerService dasJobContextManagerService,
            IPaymentLogger logger
            )
        {
            this.topicSubscriptionService = topicSubscriptionService;
            this.dasJobContextManagerService = dasJobContextManagerService;
            this.logger = logger;
        }

        public void OpenAsync(CancellationToken cancellationToken)
        {
            logger.LogInfo("Opening Job Context Manager method invoked, Topic Subscription Subscribing");
            topicSubscriptionService.Subscribe(dasJobContextManagerService.StartProcessingJobContextMessage, cancellationToken);
        }

        public async Task CloseAsync()
        {
            logger.LogInfo("Closing Job Context Manager method invoked, Topic Subscription Unsubscribing");
            await topicSubscriptionService.UnsubscribeAsync();
        }
    }
}