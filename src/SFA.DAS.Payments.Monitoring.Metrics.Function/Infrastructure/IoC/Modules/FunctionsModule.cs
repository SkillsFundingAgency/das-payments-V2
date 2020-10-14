using Autofac;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Function.Infrastructure.IoC.Modules
{
    public class FunctionsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SubmissionsSummaryMetricsService>().As<ISubmissionsSummaryMetricsService>().InstancePerLifetimeScope();
        }
    }
}
