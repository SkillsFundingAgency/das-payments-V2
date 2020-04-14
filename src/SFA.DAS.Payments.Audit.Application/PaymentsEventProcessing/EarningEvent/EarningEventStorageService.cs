using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Data.EarningEvent;
using SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent
{
    public interface IEarningEventStorageService
    {
        Task StoreEarnings(List<EarningEvents.Messages.Events.EarningEvent> earningEvents, CancellationToken cancellationToken);
        Task StoreEarnings(List<EarningEventModel> earningEvents, CancellationToken cancellationToken);
    }

    public class EarningEventStorageService : IEarningEventStorageService
    {
        private readonly IEarningEventMapper mapper;
        private readonly IPaymentLogger logger;
        private readonly IEarningEventRepository repository;
        private readonly IEarningsDuplicateEliminator duplicateEliminator;

        public EarningEventStorageService(IEarningEventMapper mapper, IPaymentLogger logger, IEarningEventRepository repository, IEarningsDuplicateEliminator duplicateEliminator)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.duplicateEliminator = duplicateEliminator ?? throw new ArgumentNullException(nameof(duplicateEliminator));
        }

        public async Task StoreEarnings(List<EarningEvents.Messages.Events.EarningEvent> earningEvents, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Removing duplicate earning events. Count: {earningEvents.Count}");
            var deDuplicatedEvents = duplicateEliminator.RemoveDuplicates(earningEvents);
            logger.LogDebug($"De-duplicated earning events count: {deDuplicatedEvents.Count}");
            var models = deDuplicatedEvents.Select(earningEvent => mapper.Map(earningEvent)).ToList();
            try
            {
                await repository.SaveEarningEvents(models, cancellationToken);
            }
            catch (Exception e)
            {
                var sqlException = e.GetException<SqlException>();
                if (!sqlException.IsUniqueKeyConstraint()) throw;
                logger.LogInfo($"Batch contained a duplicate earning.  Will store each individually and discard duplicate.");
                await repository.SaveEarningsIndividually(models.Select(model => mapper.Map(model)).ToList(), cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task StoreEarnings(List<EarningEventModel> models, CancellationToken cancellationToken)
        {
            //logger.LogDebug($"Removing duplicate earning events. Count: {models.Count}");
            //var deDuplicatedEvents = duplicateEliminator.RemoveDuplicates(earningEvents);
            //logger.LogDebug($"De-duplicated earning events count: {deDuplicatedEvents.Count}");
            //var models = deDuplicatedEvents.Select(earningEvent => mapper.Map(earningEvent)).ToList();
            try
            {
                await repository.SaveEarningEvents(models, cancellationToken);
            }
            catch (Exception e)
            {
                var sqlException = e.GetException<SqlException>();
                if (!sqlException.IsUniqueKeyConstraint()) throw;
                logger.LogInfo($"Batch contained a duplicate earning.  Will store each individually and discard duplicate.");
                await repository.SaveEarningsIndividually(models.Select(model => mapper.Map(model)).ToList(), cancellationToken).ConfigureAwait(false);
            }
        }
    }
}