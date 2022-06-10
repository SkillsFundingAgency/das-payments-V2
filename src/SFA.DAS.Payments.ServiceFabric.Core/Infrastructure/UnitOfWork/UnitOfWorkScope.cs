using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.UnitOfWork
{
    public class UnitOfWorkScope : IUnitOfWorkScope
    {
        protected ILifetimeScope LifetimeScope { get; }
        private readonly IReliableStateManagerTransactionProvider transactionProvider;
        private readonly ITelemetry telemetry;
        private readonly IOperationHolder<RequestTelemetry> operation;

        public UnitOfWorkScope(ILifetimeScope lifetimeScope, string operationName)
        {
            LifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            
            transactionProvider = lifetimeScope.Resolve<IReliableStateManagerTransactionProvider>();
            var shouldLog = string.Compare(operationName, "AuditBatchProcessing",
                StringComparison.InvariantCultureIgnoreCase) == 0;

            var logger = lifetimeScope.Resolve<IPaymentLogger>();
            
            if (!shouldLog) logger.LogVerbose($"Creating UnitOfWork transaction for {operationName}.");
            
            var stateManager = lifetimeScope.Resolve<IReliableStateManagerProvider>().Current;
            ((ReliableStateManagerTransactionProvider)transactionProvider).Current = stateManager.CreateTransaction();
            
            if (shouldLog) return;

            logger.LogWarning($"Creating UnitOfWork transaction for {operationName}.Transaction Id: {transactionProvider.Current.TransactionId}");
            
            telemetry = lifetimeScope.Resolve<ITelemetry>();
            operation = telemetry.StartOperation(operationName);
        }

        public T Resolve<T>()
        {
            return LifetimeScope.Resolve<T>();
        }

        public void Dispose()
        {
            telemetry?.StopOperation(operation);
            operation?.Dispose();
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
        }
    }
}