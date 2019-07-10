using Autofac;
using SFA.DAS.Payments.Application.Batch;

namespace SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules
{
    public class BatchModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(BulkWriter<>)).AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
