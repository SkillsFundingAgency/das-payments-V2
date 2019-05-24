using Autofac;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules
{
    public class PaymentDataContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var configHelper = c.Resolve<IConfigurationHelper>();
                return new PaymentsDataContext(configHelper.GetConnectionString("PaymentsConnectionString"));
            }).As<IPaymentsDataContext>();

            builder.RegisterType<SubmittedLearnerAimRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}