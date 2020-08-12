using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.Audit.EarningEventsService.Handlers
{
    public class ApprenticeshipContractType1RedundancyEarningEventHandler : IHandleMessageBatches<ApprenticeshipContractType1RedundancyEarningEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IEarningEventStorageService storageService;

        public ApprenticeshipContractType1RedundancyEarningEventHandler(IPaymentLogger logger, IEarningEventStorageService storageService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task Handle(IList<ApprenticeshipContractType1RedundancyEarningEvent> messages, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Handling ACT1 Redundancy Earnings Event for Job(s): { string.Join(",", messages.Select(x => x.JobId).Distinct().ToArray()) }");
            var earningEvents = new List<EarningEvent>();
            earningEvents.AddRange(messages);
            await storageService.StoreEarnings(earningEvents, cancellationToken).ConfigureAwait(false);
            logger.LogDebug($"Finished Handling ACT1 Redundancy Earnings Event for Job(s): { string.Join(",", messages.Select(x => x.JobId).Distinct().ToArray()) }");
        }
    }
}