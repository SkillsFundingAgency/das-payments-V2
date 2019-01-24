using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.Audit.AcceptanceTests.Data;

namespace SFA.DAS.Payments.Audit.AcceptanceTests.Infrastructure.Ioc
{
    public class AuditModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<TestsConfiguration>();
                    return new AuditDataContext(configHelper.PaymentsConnectionString);
                }).InstancePerLifetimeScope();
            builder.RegisterBuildCallback(c => c.Resolve<AuditDataContext>());
        }
    }
}