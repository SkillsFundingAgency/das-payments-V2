using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Metrics.Application.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Data;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Infrastructure.Ioc
{
    public class MetricsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SubmissionSummaryFactory>()
                .As<ISubmissionSummaryFactory>()
                .SingleInstance();

            builder.RegisterType<PeriodEndSummaryFactory>()
                .As<IPeriodEndSummaryFactory>()
                .SingleInstance();

            builder.RegisterType<SubmissionMetricsService>()
                .As<ISubmissionMetricsService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PeriodEndMetricsService>()
                .As<IPeriodEndMetricsService>()
                .InstancePerLifetimeScope();

            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new DcMetricsDataContext(configHelper.GetConnectionString("DcEarnings2021ConnectionString"));
                })
                .Named<IDcMetricsDataContext>("DcEarnings2021DataContext")
                .InstancePerLifetimeScope();
                
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new DcMetricsDataContext(configHelper.GetConnectionString("DcEarnings1920ConnectionString"));
                })
                .Named<IDcMetricsDataContext>("DcEarnings1920DataContext")
                .InstancePerLifetimeScope();

            builder.RegisterType<DcMetricsDataContextFactory>()
                .As<IDcMetricsDataContextFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<MetricsQueryDataContextFactory>()
                .As<IMetricsQueryDataContextFactory>()
                .InstancePerLifetimeScope();

            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    var dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(
                        configHelper.GetConnectionString("PaymentsMetricsConnectionString"),
                        optionsBuilder => optionsBuilder.CommandTimeout(270)).Options;
                    return new MetricsQueryDataContext(dbContextOptions);
                })
                .As<IMetricsQueryDataContext>()
                .InstancePerDependency();

            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    var dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(
                        configHelper.GetConnectionString("PaymentsConnectionString"),
                        optionsBuilder => optionsBuilder.CommandTimeout(270)).Options;
                    return new MetricsPersistenceDataContext(dbContextOptions);
                })
                .As<IMetricsPersistenceDataContext>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SubmissionMetricsRepository>()
                .As<ISubmissionMetricsRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PeriodEndMetricsRepository>()
                .As<IPeriodEndMetricsRepository>()
                .InstancePerLifetimeScope();

        }
    }
}