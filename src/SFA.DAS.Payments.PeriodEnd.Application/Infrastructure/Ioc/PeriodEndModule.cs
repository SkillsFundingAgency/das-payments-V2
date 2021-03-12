using Autofac;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.PeriodEnd.Application.Handlers;
using SFA.DAS.Payments.PeriodEnd.Application.Repositories;
using SFA.DAS.Payments.PeriodEnd.Application.Services;
using SFA.DAS.Payments.PeriodEnd.Data;

namespace SFA.DAS.Payments.PeriodEnd.Application.Infrastructure.Ioc
{
    public class PeriodEndModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PeriodEndJobContextMessageHandler>().As<IMessageHandler<JobContextMessage>>();
            builder.RegisterType<PeriodEndRepository>().As<IPeriodEndRepository>();
            builder.RegisterType<ProviderRequiringReprocessingService>().As<IProviderRequiringReprocessingService>();

            builder.Register((c, p) =>
            {
                var config = c.Resolve<IConfigurationHelper>();
                return new PeriodEndDataContext(config.GetConnectionString("PaymentsConnectionString"));
            }).As<IPeriodEndDataContext>();
        }
    }
}