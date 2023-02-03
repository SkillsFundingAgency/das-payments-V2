using Autofac;


namespace SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper.Telemetry
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