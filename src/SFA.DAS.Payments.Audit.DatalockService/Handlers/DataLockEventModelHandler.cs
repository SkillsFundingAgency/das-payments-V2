using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Model.Core.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Audit.DataLockService.Handlers
{
    public class DataLockEventModelHandler : IHandleMessageBatches<DataLockEventModel>
    {
        private readonly IDataLockStorageService storageService;

        public DataLockEventModelHandler(IDataLockStorageService storageService)
        {
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task Handle(IList<DataLockEventModel> models, CancellationToken cancellationToken)
        {
            await storageService.StoreEarnings(models.ToList(), cancellationToken).ConfigureAwait(false);
        }
    }
}