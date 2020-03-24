using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents;

namespace SFA.DAS.Payments.Audit.Application.Data.EarningEvent
{
    public class EarningEventStorageService
    {
        private readonly IEarningEventMapper mapper;
        private readonly IPaymentLogger logger;

        public EarningEventStorageService(IEarningEventMapper mapper, IPaymentLogger logger)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StoreEarnings(List<EarningEvents.Messages.Events.EarningEvent> earningEvent, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}