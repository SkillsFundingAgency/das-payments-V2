using Autofac;
using SFA.DAS.Payments.ProviderPayments.LegacyExportService.Cache;

namespace SFA.DAS.Payments.ProviderPayments.LegacyExportService.Infrastructure
{
    public class LegacyExportServiceInstaller: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PaymentExportProgressCache>().AsImplementedInterfaces();
        }
    }
}