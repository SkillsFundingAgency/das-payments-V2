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
        private readonly string operationName;

        //private readonly TransactionScope transactionScope;

        public UnitOfWorkScope(ILifetimeScope lifetimeScope, string operationName)
        {
            IReliableStateManagerTransactionProvider transactionProvider_ = null;
            IOperationHolder<RequestTelemetry> operation_ = null;

            telemetry = lifetimeScope.Resolve<ITelemetry>();

            telemetry.TrackAction($"{operationName} UnitOfWorkScope()", "", () =>
            {
                lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
                var stateManager = lifetimeScope.Resolve<IReliableStateManagerProvider>().Current;
                transactionProvider_ = lifetimeScope.Resolve<IReliableStateManagerTransactionProvider>();

                telemetry.TrackAction($"{operationName} create scope", "", () =>
                {
                    ((ReliableStateManagerTransactionProvider)transactionProvider_).Current = stateManager.CreateTransaction();
                });

                operation_ = telemetry.StartOperation(operationName);
                //transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            });

            this.LifetimeScope = lifetimeScope;
            this.operationName = operationName;
            this.transactionProvider = transactionProvider_;
            this.operation = operation_;
        }

        public T Resolve<T>()
        {
            return LifetimeScope.Resolve<T>();
        }

        public void Dispose() => Dispose(null);

        public void Dispose(ITelemetry telemetry = null)
        {
            telemetry?.StopOperation(operation);
            telemetry?.TrackAction($"{operationName} dispose operation", "", () =>
            {
                operation?.Dispose();
            });
            //transactionScope?.Dispose();
            telemetry?.TrackAction($"{operationName} dispose txProvider", "", () =>
            {
                transactionProvider.Current.Dispose();
            });
            ((ReliableStateManagerTransactionProvider)transactionProvider).Current = null;
            telemetry?.TrackAction($"{operationName} dispose lifetimeScope", "", () =>
            {
                LifetimeScope?.Dispose();
            });
        }

        public void Abort()
        {
            transactionProvider.Current.Abort();
        }

        public async Task Commit()
        {
            await telemetry?.TrackActionAsync($"{operationName} Commit", "", async () =>
            {
                await transactionProvider.Current.CommitAsync();
            });
        }
    }
}