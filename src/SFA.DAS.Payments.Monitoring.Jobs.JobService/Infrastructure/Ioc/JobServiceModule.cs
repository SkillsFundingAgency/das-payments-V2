using Autofac;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Infrastructure.Ioc
{
    public class JobServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JobStorageService>()
                .As<IJobStorageService>()
                .InstancePerLifetimeScope();

            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new SubmissionWindowValidationClient(configHelper.GetSetting("SubmissionWindowValidationFunctionApiKey"), configHelper.GetSetting("SubmissionWindowValidationFunctionBaseUrl"));
                })
                .As<ISubmissionWindowValidationClient>()
                .InstancePerLifetimeScope();
        }
    }
}