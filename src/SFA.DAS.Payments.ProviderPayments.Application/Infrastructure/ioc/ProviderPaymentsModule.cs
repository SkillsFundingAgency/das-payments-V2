using Autofac;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ProviderPayments.Application.Data;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Domain.Services;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Infrastructure.ioc
{
    public class ProviderPaymentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HandleIlrSubmissionService>().AsImplementedInterfaces();
            builder.RegisterType<ValidateIlrSubmission>().AsImplementedInterfaces();
            builder.RegisterType<ProviderPaymentsRepository>().AsImplementedInterfaces();

            builder.RegisterType<ProviderPaymentDataTable>()
                .As<IPaymentsEventModelDataTable<ProviderPaymentEventModel>>();

            builder.RegisterType<ProviderPaymentsEventModelBatchProcessor>()
                .As<IPaymentsEventModelBatchProcessor<ProviderPaymentEventModel>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ProviderPaymentsService>()
                .As<IProviderPaymentsService>();

            builder.RegisterType<ProviderPeriodEndService>()
                .As<IProviderPeriodEndService>();

            builder.RegisterType<ProcessAfterMonthEndPaymentService>()
                .As<IProcessAfterMonthEndPaymentService>();

            builder.RegisterType<LegacyPaymentsRepository>().AsImplementedInterfaces();
            builder.RegisterType<PaymentExportService>().AsImplementedInterfaces();
            builder.RegisterType<PaymentMapper>().AsImplementedInterfaces();
            
            builder.RegisterType<CompletionPaymentService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.Register(ctx =>
                {
                    var configHelper = ctx.Resolve<IConfigurationHelper>();
                    var dbContext = new ProviderPaymentsDataContext(configHelper.GetConnectionString("PaymentsConnectionString"));
                    return dbContext;
                })
                .As<IProviderPaymentsDataContext>()
                .InstancePerDependency();
        }
    }
}
