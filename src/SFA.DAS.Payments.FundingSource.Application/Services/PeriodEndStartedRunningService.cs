using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class PeriodEndStartedRunningService
    {
        private readonly ILevyFundingSourceRepository repository;
        private readonly IPaymentLogger logger;
        private readonly IMessageSession messageSession;

        public PeriodEndStartedRunningService(ILevyFundingSourceRepository repository, IPaymentLogger logger,
            IMessageSession messageSession)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
        }

        public async Task PeriodEndRunning(PeriodEndRunningEvent message)
        {
            var accounts = await repository.GetEmployerAccounts(CancellationToken.None).ConfigureAwait(false);
            foreach (var account in accounts)
            {
                logger.LogDebug($"Triggering period end for account: {account}");
                var levyPaymentsOnMonthEndCommand = new ProcessLevyPaymentsOnMonthEndCommand
                {

                };
                messageSession.SendLocal(levyPaymentsOnMonthEndCommand);

            }
        }
    }
}