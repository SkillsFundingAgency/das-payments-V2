﻿using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;

namespace SFA.DAS.Payments.ServiceFabric.Core.Batch
{
    public class BatchScope: IBatchScope
    {
        private readonly ILifetimeScope lifetimeScope;
        private readonly IReliableStateManagerTransactionProvider transactionProvider;
        private readonly ITelemetry telemetry;
        private readonly IOperationHolder<RequestTelemetry> operation;
        public BatchScope(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            var stateManager = lifetimeScope.Resolve<IReliableStateManagerProvider>().Current;
            transactionProvider = lifetimeScope.Resolve<IReliableStateManagerTransactionProvider>();
            ((ReliableStateManagerTransactionProvider)transactionProvider).Current = stateManager.CreateTransaction();
            telemetry = lifetimeScope.Resolve<ITelemetry>();
            operation = telemetry.StartOperation("AuditBatchProcessing");
        }

        public void Dispose()
        {
            telemetry?.StopOperation(operation);
            operation?.Dispose();
            transactionProvider.Current.Dispose();
            ((ReliableStateManagerTransactionProvider)transactionProvider).Current = null;
            lifetimeScope?.Dispose();
        }

        public IBatchProcessor<T> GetBatchProcessor<T>()
        {
            return lifetimeScope.Resolve<IBatchProcessor<T>>();
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