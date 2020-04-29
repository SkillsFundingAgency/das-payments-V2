using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public interface IProcessLevyAccountBalanceService
    {
        Task RefreshLevyAccountDetails(int pageNumber, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> GetTotalNumberOfPages();
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

        public async Task<int> GetTotalNumberOfPages()
        {
            return await this.accountBalanceService.GetTotalPageSize();
        }

        public async Task RefreshLevyAccountDetails(int pageNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                paymentLogger.LogInfo($"Starting to refresh page of Levy Accounts Details (Page {pageNumber})");

                try
                {
                    await accountBalanceService.RefreshLevyAccountDetails(pageNumber, cancellationToken);
                }
                catch (Exception e)
                {
                    paymentLogger.LogError($"Error While trying to refresh page of Levy Accounts Details (Page {pageNumber})", e);
                }

            }
            catch (TaskCanceledException e)
            {
                paymentLogger.LogError($"Levy Accounts Refresh Task was Canceled (Page {pageNumber})", e);
            }
            catch (Exception e)
            {
                paymentLogger.LogError($"Error While trying to refresh page of Levy Accounts Details (Page {pageNumber})", e);
                throw;
            }
        }
    }
}
