using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Audit.Application.Data.EarningEvent;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.Audit.EarningEventsService.Handlers
{
    public class ApprenticeshipContractType2EarningEventHandler : IHandleMessageBatches<ApprenticeshipContractType2EarningEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IEarningEventStorageService storageService;

        public ApprenticeshipContractType2EarningEventHandler(IPaymentLogger logger, IEarningEventStorageService storageService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task Handle(IList<ApprenticeshipContractType2EarningEvent> messages, CancellationToken cancellationToken)
        {
            var earningEvents = new List<EarningEvent>();
            earningEvents.AddRange(messages);
            await storageService.StoreEarnings(earningEvents, cancellationToken).ConfigureAwait(false);
        }
    }

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
            var earningEvents = new List<EarningEvent>();
            earningEvents.AddRange(messages);
            await storageService.StoreEarnings(earningEvents, cancellationToken).ConfigureAwait(false);
        }
    }

    public class Act1RedundancyFunctionalSkillEarningsEventHandler : IHandleMessageBatches<Act1RedundancyFunctionalSkillEarningsEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IEarningEventStorageService storageService;

        public Act1RedundancyFunctionalSkillEarningsEventHandler(IPaymentLogger logger, IEarningEventStorageService storageService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task Handle(IList<Act1RedundancyFunctionalSkillEarningsEvent> messages, CancellationToken cancellationToken)
        {
            var earningEvents = new List<EarningEvent>();
            earningEvents.AddRange(messages);
            await storageService.StoreEarnings(earningEvents, cancellationToken).ConfigureAwait(false);
        }
    }

    public class Act2RedundancyFunctionalSkillEarningsEventHandler : IHandleMessageBatches<Act2RedundancyFunctionalSkillEarningsEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IEarningEventStorageService storageService;

        public Act2RedundancyFunctionalSkillEarningsEventHandler(IPaymentLogger logger, IEarningEventStorageService storageService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task Handle(IList<Act2RedundancyFunctionalSkillEarningsEvent> messages, CancellationToken cancellationToken)
        {
            var earningEvents = new List<EarningEvent>();
            earningEvents.AddRange(messages);
            await storageService.StoreEarnings(earningEvents, cancellationToken).ConfigureAwait(false);
        }
    }

    public class ApprenticeshipContractType2RedundancyEarningEventHandler : IHandleMessageBatches<ApprenticeshipContractType2RedundancyEarningEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IEarningEventStorageService storageService;

        public ApprenticeshipContractType2RedundancyEarningEventHandler(IPaymentLogger logger, IEarningEventStorageService storageService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task Handle(IList<ApprenticeshipContractType2RedundancyEarningEvent> messages, CancellationToken cancellationToken)
        {
            var earningEvents = new List<EarningEvent>();
            earningEvents.AddRange(messages);
            await storageService.StoreEarnings(earningEvents, cancellationToken).ConfigureAwait(false);
        }
    }
}