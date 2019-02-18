using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using NServiceBus.UnitOfWork;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.Audit.Application.ServiceFabric.Infrastructure
{
    public class StateManagerUnitOfWork : IManageUnitsOfWork
    {
        private readonly IReliableStateManagerTransactionProvider reliableStateManagerTransactionProvider;
        private readonly IPaymentLogger logger;
        
        private readonly IReliableStateManager stateManager;
        public StateManagerUnitOfWork(IReliableStateManagerProvider stateManagerProvider, 
            IReliableStateManagerTransactionProvider reliableStateManagerTransactionProvider,
            IPaymentLogger logger)
        {
            stateManager = stateManagerProvider?.Current ?? throw new ArgumentNullException(nameof(stateManagerProvider));
            this.reliableStateManagerTransactionProvider = reliableStateManagerTransactionProvider ?? throw new ArgumentNullException(nameof(reliableStateManagerTransactionProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Begin()
        {
            logger.LogVerbose($"Creating state manager transaction.");
            ((ReliableStateManagerTransactionProvider)reliableStateManagerTransactionProvider).Current = stateManager.CreateTransaction();
            logger.LogDebug($"Creating state manager transaction.Transaction Id: {reliableStateManagerTransactionProvider.Current.TransactionId}");
            return Task.CompletedTask;
        }

        public async Task End(Exception ex = null)
        {
            try
            {
                if (ex != null)
                {
                    logger.LogWarning($"Rolling back the state manager transaction due to exception during message handling. Transaction Id: {reliableStateManagerTransactionProvider.Current.TransactionId}, Exception: {ex.Message}");
                    reliableStateManagerTransactionProvider.Current.Abort();
                    return;
                }
                logger.LogVerbose($"Completing state manager transaction. Transaction Id: {reliableStateManagerTransactionProvider.Current.TransactionId}");
                await reliableStateManagerTransactionProvider.Current.CommitAsync();
                logger.LogDebug($"Completed state manager transaction. TransactionId: {reliableStateManagerTransactionProvider.Current.TransactionId}");
            }
            finally
            {
                reliableStateManagerTransactionProvider.Current.Dispose();
            }
        }
    }
}
