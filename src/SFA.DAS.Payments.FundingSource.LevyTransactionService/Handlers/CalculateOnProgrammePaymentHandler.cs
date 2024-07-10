using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Messages.Commands;

namespace SFA.DAS.Payments.FundingSource.LevyTransactionService.Handlers
{
    public class CalculateOnProgrammePaymentHandler : IHandleMessageBatches<CalculateOnProgrammePayment>
    {
        private readonly IPaymentLogger logger;
        private readonly ILevyTransactionBatchStorageService levyTransactionBatchStorageService;
        
        public CalculateOnProgrammePaymentHandler(IPaymentLogger logger,
            ILevyTransactionBatchStorageService levyTransactionBatchStorageService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.levyTransactionBatchStorageService = levyTransactionBatchStorageService ?? throw new ArgumentNullException(nameof(levyTransactionBatchStorageService));
        }

        public async Task Handle(IList<CalculateOnProgrammePayment> messages, CancellationToken cancellationToken)
        {
            logger.LogInfo($"CalculateOnProgrammePaymentHandlerReceived {messages.Count} messages");
            await levyTransactionBatchStorageService.StoreLevyTransactions(messages, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
