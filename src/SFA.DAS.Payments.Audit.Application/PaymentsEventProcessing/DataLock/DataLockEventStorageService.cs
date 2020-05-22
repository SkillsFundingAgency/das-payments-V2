using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Data.DataLock;
using SFA.DAS.Payments.Audit.Application.Mapping.DataLock;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.DataLock
{
    public class DataLockEventStorageService 
    {
        private readonly IDataLockEventMapper mapper;
        private readonly IPaymentLogger logger;
        private readonly IDataLockEventRepository repository;

        public DataLockEventStorageService(IDataLockEventMapper mapper, IPaymentLogger logger, IDataLockEventRepository repository)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task StoreDataLocks(List<DataLockEventModel> models, CancellationToken cancellationToken)
        {
            try
            {
                await repository.SaveDataLockEvents(models, cancellationToken);
            }
            catch (Exception e)
            {
                if (!e.IsUniqueKeyConstraintException() && !e.IsDeadLockException()) throw;
                logger.LogInfo($"Batch contained a duplicate DataLock.  Will store each individually and discard duplicate.");
                await repository.SaveDataLocksIndividually(models.Select(model => mapper.Map(model)).ToList(), cancellationToken).ConfigureAwait(false);
            }
        }
    }
}