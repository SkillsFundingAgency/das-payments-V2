using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Messaging.PostProcessing;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Client;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.MessageInterceptors
{
    public class InterceptFailedJobMessages: InterceptJobMessages, IInterceptFailedMessages
    {
        private readonly IPaymentLogger logger;
        private readonly IJobMessageClient jobMessageClient;

        public InterceptFailedJobMessages(IPaymentLogger logger, IJobMessageClient jobMessageClient)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.jobMessageClient = jobMessageClient ?? throw new ArgumentNullException(nameof(jobMessageClient));
        }

        public async Task Process(object message, Exception messageException, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Intercepted failed message. Error: {messageException.Message}");
            var payment = message as PaymentModel;
            if (payment == null)
                return;
            await jobMessageClient.ProcessingFailedForJobMessage(payment.JobId, payment.FundingSourceEventId, GetFundingSourceMessageType(payment));
        }
    }
}