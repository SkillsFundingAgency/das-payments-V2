using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Messaging.PostProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Client;

namespace SFA.DAS.Payments.ProviderPayments.Application.Messaging
{
    public class InterceptSuccessfulJobMessages: IInterceptSuccessfulMessages
    {
        private readonly IPaymentLogger logger;
        private readonly IJobMessageClient jobMessageClient;

        public InterceptSuccessfulJobMessages(IPaymentLogger logger, IJobMessageClient jobMessageClient)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.jobMessageClient = jobMessageClient ?? throw new ArgumentNullException(nameof(jobMessageClient));
        }


        public async Task Process(Type groupType, List<object> messages, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Intercepted batch of successful messages. Count: {messages.Count}");
        }
    }
}