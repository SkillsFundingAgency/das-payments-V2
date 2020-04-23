using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.EarningEventsService.Handlers
{
    public class EarningEventModelHandler : IHandleMessageBatches<EarningEventModel>
    {
        private readonly IEarningEventStorageService storageService;

        public EarningEventModelHandler(IEarningEventStorageService storageService)
        {
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task Handle(IList<EarningEventModel> models, CancellationToken cancellationToken)
        {
            await storageService.StoreEarnings(models.ToList(), cancellationToken).ConfigureAwait(false);
        }
    }
}