using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class FoundNotLevyPayerEmployerAccountEventHandler : IHandleMessages<FoundNotLevyPayerEmployerAccount>
    {
        private readonly IApprenticeshipProcessor apprenticeshipProcessor;
        private readonly IPaymentLogger paymentLogger;

        public FoundNotLevyPayerEmployerAccountEventHandler(IApprenticeshipProcessor apprenticeshipProcessor  ,IPaymentLogger paymentLogger)
        {
            this.apprenticeshipProcessor = apprenticeshipProcessor;
            this.paymentLogger = paymentLogger;
        }

        public async Task Handle(FoundNotLevyPayerEmployerAccount message, IMessageHandlerContext context)
        {
            paymentLogger.LogDebug($"Handling Found NotLevyPayer Employer Account Id: {message.AccountId}");
            await apprenticeshipProcessor.ProcessNonLevyPayerFlagForEmployer(message.AccountId, false).ConfigureAwait(false);
            paymentLogger.LogInfo($"Finished Handling Found NotLevyPayer Employer Account Id: {message.AccountId}");
        }
    }
}
