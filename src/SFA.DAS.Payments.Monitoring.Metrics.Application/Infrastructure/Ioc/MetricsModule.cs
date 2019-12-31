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

            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new DcMetricsDataContext(configHelper.GetConnectionString("DcEarningsConnectionString"));
                })
                .As<IDcMetricsDataContext>()
                .InstancePerLifetimeScope();
        }
    }
}