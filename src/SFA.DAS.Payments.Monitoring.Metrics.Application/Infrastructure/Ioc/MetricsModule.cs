using System;
using Autofac;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Data;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Infrastructure.Ioc
{
    public class MetricsModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SubmissionSummaryFactory>()
                .As<ISubmissionSummaryFactory>()
                .SingleInstance();

            builder.RegisterType<SubmissionMetricsService>()
                .As<ISubmissionMetricsService>()
                .InstancePerLifetimeScope();

            builder.Register((c, p) =>
                {
                    try
                    {
                        var configHelper = c.Resolve<IConfigurationHelper>();
                        return new DcMetricsDataContextConnectionStringProvider(configHelper.GetConnectionString("DcEarningsConnectionString"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                })
                .As<IDcMetricsDataContextConnectionStringProvider>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DcMetricsDataContext.Factory>();

            //builder.RegisterType<DcMetricsDataContext>()
            //    .As<IDcMetricsDataContext>()
            //    .InstancePerDependency()
            //    .WithParameter("connectionString", )

            //builder.Register((c, p) =>
            //    {
            //        var configHelper = c.Resolve<IConfigurationHelper>();
            //        //var dcMetricsDataContextFactory = c.Resolve<DcMetricsDataContext.Factory>();
            //        //return dcMetricsDataContextFactory.Invoke(configHelper.GetConnectionString("DcEarningsConnectionString"), )
            //        return new DcMetricsDataContext(configHelper.GetConnectionString("DcEarningsConnectionString"), 1920);
            //    })
            //    .As<IDcMetricsDataContext>()
            //    .InstancePerLifetimeScope();

        builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new MetricsQueryDataContext(configHelper.GetConnectionString("PaymentsMetricsConnectionString"));
                })
                .As<IMetricsQueryDataContext>()
                .InstancePerLifetimeScope();

            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new MetricsPersistenceDataContext(configHelper.GetConnectionString("PaymentsConnectionString"));
                })
                .As<IMetricsPersistenceDataContext>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SubmissionMetricsRepository>()
                .As<ISubmissionMetricsRepository>()
                .InstancePerLifetimeScope();

            //builder.RegisterType<SubmissionMetricsService>()
            //    .As<ISubmissionMetricsService>()
            //    .InstancePerLifetimeScope();

            builder.Register((c, p) =>
                {
                    try
                    {
                        c.Resolve<IDcMetricsDataContextConnectionStringProvider>();
                        c.Resolve<DcMetricsDataContext.Factory>();
                        
                        return (ISubmissionMetricsService)null;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                })
                .As<ISubmissionMetricsService>()
                .InstancePerLifetimeScope();
        }
    }
}