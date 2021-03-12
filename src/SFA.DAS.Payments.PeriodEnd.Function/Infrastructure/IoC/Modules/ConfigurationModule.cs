using Autofac;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.PeriodEnd.Function.Infrastructure.Configuration;

namespace SFA.DAS.Payments.PeriodEnd.Function.Infrastructure.IoC.Modules
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FunctionsConfigurationHelper>().As<IConfigurationHelper>().SingleInstance();

            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new SubmissionMetricsConfiguration
                    {
                        PaymentsConnectionString = configHelper.GetSetting("PaymentsConnectionString")
                    };
                })
                .As<ISubmissionMetricsConfiguration>()
                .SingleInstance();
        }
    }
}