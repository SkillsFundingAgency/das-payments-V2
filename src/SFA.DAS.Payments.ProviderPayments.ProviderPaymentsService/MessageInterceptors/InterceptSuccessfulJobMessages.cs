using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Messaging.PostProcessing;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Client;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.MessageInterceptors
{
    public class InterceptSuccessfulJobMessages: InterceptJobMessages, IInterceptSuccessfulMessages
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
            logger.LogVerbose($"Intercepted batch of successful messages. Count: {messages.Count}");
            var jobMessages = messages
                .OfType<PaymentModel>()
                .Select(payment => (payment.JobId, payment.FundingSourceEventId,
                    GetFundingSourceMessageType(payment)))
                .ToList();
            await jobMessageClient.ProcessedJobMessages(jobMessages);
            logger.LogDebug($"Intercepted and recorded batch of successful messages. Count: {messages.Count}");
        }
    }
}