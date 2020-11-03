using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Data;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Function.Infrastructure.Configuration;

namespace SFA.DAS.Payments.Monitoring.Metrics.Function.Infrastructure.IoC.Modules
{
    public class FunctionsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SubmissionWindowValidationService>().As<ISubmissionWindowValidationService>().InstancePerLifetimeScope();
            builder.RegisterType<SubmissionMetricsRepository>().As<ISubmissionMetricsRepository>().InstancePerLifetimeScope();
            builder.RegisterType<SubmissionsSummary>().As<ISubmissionsSummary>().InstancePerLifetimeScope();

            builder.Register((c, p) =>
                {
                    var config = c.Resolve<ISubmissionMetricsConfiguration>();
                    var dbContextOptions = new DbContextOptionsBuilder()
                        .UseSqlServer(config.PaymentsConnectionString, 
                  optionsBuilder => optionsBuilder.CommandTimeout(270)).Options;
                    return new MetricsPersistenceDataContext(dbContextOptions);
                })
                .As<IMetricsPersistenceDataContext>()
                .InstancePerLifetimeScope();

            //NOTE: Not in use but required to get SubmissionMetricsRepository working
            builder.RegisterType<MetricsQueryDataContextFactory>()
                .As<IMetricsQueryDataContextFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SubmissionJobsService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SubmissionJobsRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.Register((c, p) =>
                {
                    var config = c.Resolve<ISubmissionMetricsConfiguration>();
                    var dbContextOptions = new DbContextOptionsBuilder()
                        .UseSqlServer(config.PaymentsConnectionString, 
                            optionsBuilder => optionsBuilder.CommandTimeout(270)).Options;
                    return new SubmissionJobsDataContext(dbContextOptions);
                })
                .As<ISubmissionJobsDataContext>()
                .InstancePerLifetimeScope();
        }
    }
}
