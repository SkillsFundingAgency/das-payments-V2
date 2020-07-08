using Autofac;
using SFA.DAS.Payments.Audit.Application.Data.DataLock;
using SFA.DAS.Payments.Audit.Application.Mapping.DataLock;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.DataLock;

namespace SFA.DAS.Payments.Audit.Application.Infrastructure.Ioc
{
    public class DataLockAuditModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DataLockEventRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<DataLockEventMapper>()
                .As<IDataLockEventMapper>();

            builder.RegisterType<DataLockEventStorageService>()
                .As<IDataLockEventStorageService>()
                .InstancePerLifetimeScope();
        }
    }
}