using System;
using Autofac;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data
{
    public interface IMetricsQueryDataContextFactory
    {
        IMetricsQueryDataContext Create();
    }

    public class MetricsQueryDataContextFactory : IMetricsQueryDataContextFactory
    {
        private readonly ILifetimeScope lifetimeScope;

        public MetricsQueryDataContextFactory(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public IMetricsQueryDataContext Create()
        {
            return lifetimeScope.Resolve<IMetricsQueryDataContext>();
        }
    }
}