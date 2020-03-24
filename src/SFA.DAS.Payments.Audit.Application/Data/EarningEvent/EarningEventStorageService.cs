using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents;

namespace SFA.DAS.Payments.Audit.Application.Data.EarningEvent
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

        public EarningEventStorageService(IEarningEventMapper mapper, IPaymentLogger logger, IEarningEventRepository repository)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task StoreEarnings(List<EarningEvents.Messages.Events.EarningEvent> earningEvents, CancellationToken cancellationToken)
        {
            var models = earningEvents.Select(earningEvent => mapper.Map(earningEvent)).ToList();
            await repository.SaveEarningEvents(models, cancellationToken);
        }
    }
}