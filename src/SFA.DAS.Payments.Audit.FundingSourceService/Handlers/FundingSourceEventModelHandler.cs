using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.FundingSourceService.Handlers
{
    public class FundingSourceEventModelHandler : IHandleMessageBatches<FundingSourceEventModel>
    {
        private readonly IFundingSourceEventStorageService storageService;

        public FundingSourceEventModelHandler(IFundingSourceEventStorageService storageService)
        {
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task Handle(IList<FundingSourceEventModel> models, CancellationToken cancellationToken)
        {
            await storageService.StoreFundingSourceEvents(models.ToList(), cancellationToken).ConfigureAwait(false);
        }
    }
}