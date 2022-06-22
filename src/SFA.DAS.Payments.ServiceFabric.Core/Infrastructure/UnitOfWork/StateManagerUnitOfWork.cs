using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using NServiceBus.UnitOfWork;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.UnitOfWork
{
    
    public interface IStateManagerUnitOfWork
    {
        Task Begin();
        Task End(Exception ex = null);
    }

    public class StateManagerUnitOfWork : IManageUnitsOfWork, IStateManagerUnitOfWork
    {
        private readonly IReliableStateManagerTransactionProvider transactionProvider;
        private readonly IPaymentLogger logger;
        
        private readonly IReliableStateManager stateManager;
        public StateManagerUnitOfWork(IReliableStateManagerProvider stateManagerProvider, 
            IReliableStateManagerTransactionProvider reliableStateManagerTransactionProvider,
            IPaymentLogger logger)
        {
            stateManager = stateManagerProvider?.Current ?? throw new ArgumentNullException(nameof(stateManagerProvider));
            this.transactionProvider = reliableStateManagerTransactionProvider ?? throw new ArgumentNullException(nameof(reliableStateManagerTransactionProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Begin()
        {
            logger.LogVerbose("Creating StateManager transaction.");
            ((ReliableStateManagerTransactionProvider)transactionProvider).Current = stateManager.CreateTransaction();
            logger.LogDebug($"Creating StateManager transaction.Transaction Id: {transactionProvider.Current.TransactionId}");
            return Task.CompletedTask;
        }

        public async Task End(Exception ex = null)
        {
            try
            {
                if (ex != null)
                {
                    logger.LogWarning($"Rolling back the StateManager transaction due to exception during message handling. Transaction Id: {transactionProvider.Current.TransactionId}, Exception: {ex.Message}");
                    transactionProvider.Current.Abort();
                    return;
                }
                logger.LogVerbose($"Completing state manager transaction. Transaction Id: {transactionProvider.Current.TransactionId}");
                await transactionProvider.Current.CommitAsync().ConfigureAwait(false);
                logger.LogDebug($"Completed StateManager transaction. TransactionId: {transactionProvider.Current.TransactionId}");
            }
            finally
            {
                transactionProvider.Current.Dispose();
            }
        }
    }
}
