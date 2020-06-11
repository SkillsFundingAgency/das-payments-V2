using Autofac;
using FluentValidation;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration;
using SFA.DAS.Payments.ScheduledJobs.Monitoring.LevyAccountData;

namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC.Modules
{
    public class LevyAccountValidationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AccountApiClient>().As<IAccountApiClient>().InstancePerLifetimeScope();

            builder.RegisterType<LevyAccountValidator>().As<IValidator<LevyAccountsDto>>().InstancePerLifetimeScope();
            builder.RegisterType<CombinedLevyAccountValidator>().As<IValidator<CombinedLevyAccountsDto>>().InstancePerLifetimeScope();


            builder.Register((c, p) =>
            {
                var config = c.Resolve<IScheduledJobsConfiguration>();
                var apiClient = c.Resolve<IAccountApiClient>();
                var logger = c.Resolve<IPaymentLogger>();

                return new DasLevyAccountApiWrapper(config.AccountApiBatchSize, apiClient, logger);
            }).As<IDasLevyAccountApiWrapper>().InstancePerLifetimeScope();
            
            builder.RegisterType<LevyAccountValidationService>().As<ILevyAccountValidationService>().InstancePerLifetimeScope();
        }
    }
}
