using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Data.EarningEvent;
using SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent
{
    public interface IEarningEventStorageService
    {
        Task StoreEarnings(List<EarningEvents.Messages.Events.EarningEvent> earningEvents, CancellationToken cancellationToken);
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
            var deDuplicatedEvents = duplicateEliminator.RemoveDuplicates(earningEvents);
            var models = deDuplicatedEvents.Select(earningEvent => mapper.Map(earningEvent)).ToList();
            await repository.SaveEarningEvents(models, cancellationToken);
        }

    }
}