using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class PaymentModelBatchHandler : IHandleMessageBatches<PaymentModel>
    {
        private readonly IPaymentLogger logger;
        private readonly IProviderPaymentStorageService storageService;

        public PaymentModelBatchHandler(IPaymentLogger logger, IProviderPaymentStorageService storageService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task Handle(IList<PaymentModel> messages, CancellationToken cancellationToken)
        {
            await storageService.StoreProviderPayments(messages.ToList(), cancellationToken);
        }
    }
}