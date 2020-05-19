using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.RequiredPayment;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.RequiredPaymentService.Handlers
{
    public class RequiredPaymentEventModelHandler : IHandleMessageBatches<RequiredPaymentEventModel>
    {
        private readonly IRequiredPaymentEventStorageService storageService;

        public RequiredPaymentEventModelHandler(IRequiredPaymentEventStorageService storageService)
        {
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task Handle(IList<RequiredPaymentEventModel> models, CancellationToken cancellationToken)
        {
            await storageService.StoreRequiredPayments(models.ToList(), cancellationToken).ConfigureAwait(false);
        }
    }
}