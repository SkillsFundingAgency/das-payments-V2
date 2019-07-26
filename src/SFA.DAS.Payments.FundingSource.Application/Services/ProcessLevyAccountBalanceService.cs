using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public interface IProcessLevyAccountBalanceService
    {
        Task RunAsync(TimeSpan refreshInterval, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class ProcessLevyAccountBalanceService : IProcessLevyAccountBalanceService
    {
        private readonly IManageLevyAccountBalanceService accountBalanceService;
        private readonly IPaymentLogger paymentLogger;

        public ProcessLevyAccountBalanceService(IManageLevyAccountBalanceService accountBalanceService, IPaymentLogger paymentLogger)
        {
            this.accountBalanceService = accountBalanceService;
            this.paymentLogger = paymentLogger;
        }

        public async Task RunAsync(TimeSpan refreshInterval, CancellationToken cancellationToken = default(CancellationToken))
        {
            var isPeriodStarted = true; //TODO  read value from cache

            while (isPeriodStarted)
            {
                cancellationToken.ThrowIfCancellationRequested();

                paymentLogger.LogInfo("Starting to refresh all Levy Accounts Details");

                try
                {

                    await accountBalanceService.RefreshLevyAccountDetails(cancellationToken);
                }
                catch (Exception e)
                {
                    paymentLogger.LogError("Error While trying to refresh all Levy Accounts Details", e);
                }

                // isPeriodStarted = true //TODO  read value from cache

                await Task.Delay(refreshInterval, cancellationToken);
            }
        }
    }
}
