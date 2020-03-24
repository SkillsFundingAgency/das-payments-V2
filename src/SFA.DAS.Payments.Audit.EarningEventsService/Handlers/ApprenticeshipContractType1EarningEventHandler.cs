using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.Audit.EarningEventsService.Handlers
{
    public class ApprenticeshipContractType1EarningEventHandler: IHandleMessageBatches<ApprenticeshipContractType1EarningEvent>
    {
        private readonly IPaymentLogger logger;

        public ApprenticeshipContractType1EarningEventHandler(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(IList<ApprenticeshipContractType1EarningEvent> messages, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Got {messages.Count} act1 earning events");
        }
    }

    public class ApprenticeshipContractType2EarningEventHandler : IHandleMessageBatches<ApprenticeshipContractType2EarningEvent>
    {
        private readonly IPaymentLogger logger;

        public ApprenticeshipContractType2EarningEventHandler(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(IList<ApprenticeshipContractType2EarningEvent> messages, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Got {messages.Count} act2 earning events.");
        }
    }

    public class Act1FunctionalSkillEarningsEventHandler : IHandleMessageBatches<Act1FunctionalSkillEarningsEvent>
    {
        private readonly IPaymentLogger logger;

        public Act1FunctionalSkillEarningsEventHandler(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(IList<Act1FunctionalSkillEarningsEvent> messages, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Got {messages.Count} act1 functional skills earning events.");
        }
    }

    public class Act2FunctionalSkillEarningsEventHandler : IHandleMessageBatches<Act2FunctionalSkillEarningsEvent>
    {
        private readonly IPaymentLogger logger;

        public Act2FunctionalSkillEarningsEventHandler(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(IList<Act2FunctionalSkillEarningsEvent> messages, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Got {messages.Count} act2 functional skills earning events.");
        }
    }
}