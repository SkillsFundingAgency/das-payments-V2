using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messaging.PostProcessing;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

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
            logger.LogDebug($"Intercepted batch of successful messages. Count: {messages.Count}");
            foreach (object message in messages)
            {
                var payment = message as PaymentModel;
                if (payment == null)
                    continue;
                await jobMessageClient.ProcessedJobMessage(payment.JobId, payment.FundingSourceEventId,
                    GetFundingSourceMessageType(payment), new List<GeneratedMessage>());
            }
        }
    }
}