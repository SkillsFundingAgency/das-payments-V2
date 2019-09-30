using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.UnitOfWork
{
    public class UnitOfWorkScope : IUnitOfWorkScope
    {
        protected  ILifetimeScope LifetimeScope { get; }
        private readonly IReliableStateManagerTransactionProvider transactionProvider;
        private readonly ITelemetry telemetry;
        private readonly IOperationHolder<RequestTelemetry> operation;
        //private readonly TransactionScope transactionScope;

        public UnitOfWorkScope(ILifetimeScope lifetimeScope, string operationName)
        {
            this.LifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            var stateManager = lifetimeScope.Resolve<IReliableStateManagerProvider>().Current;
            transactionProvider = lifetimeScope.Resolve<IReliableStateManagerTransactionProvider>();
            ((ReliableStateManagerTransactionProvider)transactionProvider).Current = stateManager.CreateTransaction();
            telemetry = lifetimeScope.Resolve<ITelemetry>();
            operation = telemetry.StartOperation(operationName);
            //transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        }

        public T Resolve<T>()
        {
            return LifetimeScope.Resolve<T>();
        }

        public void Dispose()
        {
            telemetry?.StopOperation(operation);
            operation?.Dispose();
            //transactionScope?.Dispose();
            transactionProvider.Current.Dispose();
            ((ReliableStateManagerTransactionProvider)transactionProvider).Current = null;
            LifetimeScope?.Dispose();
        }

        public void Abort()
        {
            transactionProvider.Current.Abort();
        }

        public async Task Commit()
        {
            await transactionProvider.Current.CommitAsync();
            //transactionScope?.Complete();
        }
    }
}