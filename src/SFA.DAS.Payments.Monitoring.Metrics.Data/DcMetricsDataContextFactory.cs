using System;
using System.Collections.Generic;
using Autofac;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data
{
    public interface IDcMetricsDataContextFactory
    {
        IDcMetricsDataContext Create(short academicYear);
    }

    public class DcMetricsDataContextFactory : IDcMetricsDataContextFactory
    {
        private readonly ILifetimeScope lifetimeScope;

        public DcMetricsDataContextFactory(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public IDcMetricsDataContext Create(short academicYear)
        {
            var foundDcDataContext = lifetimeScope.TryResolveNamed($"DcEarnings{academicYear}DataContext", typeof(IDcMetricsDataContext), out var dcDataContext);

            if (!foundDcDataContext) throw new ApplicationException($"Error creating DcMetricsDataContext for Academic Year {academicYear}");

            return (IDcMetricsDataContext)dcDataContext;
        }
    }
}