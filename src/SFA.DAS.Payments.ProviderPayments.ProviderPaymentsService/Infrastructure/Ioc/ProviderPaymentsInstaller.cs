using Autofac;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Data;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Model;
using SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Cache;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Infrastructure.Ioc
{
    public class ProviderPaymentsInstaller: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IlrSubmissionCache>()
                .As<IDataCache<IlrSubmittedEvent>>()
                .InstancePerLifetimeScope();
;
        }
    }
}