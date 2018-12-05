using System;
using System.Threading.Tasks;
using System.Transactions;
using Autofac;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.ServiceFabric.Infrastructure;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.Audit.Application.ServiceFabric.PaymentsEventProcessing
{
    public class BatchScope: IBatchScope
    {
        private readonly ILifetimeScope lifetimeScope;
        private readonly IReliableStateManagerTransactionProvider transactionProvider;
        private readonly TransactionScope transactionScope;
        public BatchScope(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            var stateManager = lifetimeScope.Resolve<IReliableStateManagerProvider>().Current;
            transactionProvider = lifetimeScope.Resolve<IReliableStateManagerTransactionProvider>();
            ((ReliableStateManagerTransactionProvider)transactionProvider).Current = stateManager.CreateTransaction();
            transactionScope = new TransactionScope();
        }

        public void Dispose()
        {
            transactionProvider.Current.Dispose();
            transactionScope.Dispose();
            ((ReliableStateManagerTransactionProvider)transactionProvider).Current = null;
            lifetimeScope?.Dispose();
        }

        public IPaymentsEventModelBatchProcessor<T> GetBatchProcessor<T>() where T: PaymentsEventModel
        {
            return lifetimeScope.Resolve<IPaymentsEventModelBatchProcessor<T>>();
        }

        public void Abort()
        {
            transactionProvider.Current.Abort();
        }

        public async Task Commit()
        {
            transactionScope.Complete();
            await transactionProvider.Current.CommitAsync();
        }
    }
}