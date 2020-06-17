using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class FoundLevyPayerEmployerAccountEventHandler : IHandleMessages<FoundLevyPayerEmployerAccount>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IApprenticeshipProcessor apprenticeshipProcessor;

        public FoundLevyPayerEmployerAccountEventHandler(IPaymentLogger paymentLogger, IApprenticeshipProcessor apprenticeshipProcessor )
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.apprenticeshipProcessor = apprenticeshipProcessor ?? throw new ArgumentNullException(nameof(apprenticeshipProcessor));
        }

        public async Task Handle(FoundLevyPayerEmployerAccount message, IMessageHandlerContext context)
        {
            paymentLogger.LogDebug($"Handling Found LevyPayer Employer Account Id: {message.AccountId}");
            await apprenticeshipProcessor.ProcessNonLevyPayerFlagForEmployer(message.AccountId, true).ConfigureAwait(false);
            paymentLogger.LogInfo($"Finished Handling Found LevyPayer Employer Account Id: {message.AccountId}");
        }
    }
}
