using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Model.Core.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.DataLock;

namespace SFA.DAS.Payments.Audit.DataLockService.Handlers
{
    public class DataLockEventModelHandler : IHandleMessageBatches<DataLockEventModel>
    {
        private readonly IDataLockEventStorageService storageService;

        public DataLockEventModelHandler(IDataLockEventStorageService storageService)
        {
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task Handle(IList<DataLockEventModel> models, CancellationToken cancellationToken)
        {
            await storageService.StoreDataLocks(models.ToList(), cancellationToken).ConfigureAwait(false);
        }
    }
}