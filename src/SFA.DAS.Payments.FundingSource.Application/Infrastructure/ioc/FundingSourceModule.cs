using Autofac;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using System.Collections.Generic;
using Autofac.Integration.ServiceFabric;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;

namespace SFA.DAS.Payments.FundingSource.Application.Infrastructure.Ioc
{
    public class FundingSourceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ValidateRequiredPaymentEvent>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<CoInvestedFundingSourcePaymentEventMapper>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SfaFullyFundedPaymentProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SfaFullyFundedFundingSourcePaymentEventMapper>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<IncentiveRequiredPaymentProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<LevyAccountRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<PaymentProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<LevyPaymentProcessor>().As<ILevyPaymentProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<CoInvestedPaymentProcessor>().As<ICoInvestedPaymentProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<EmployerCoInvestedPaymentProcessor>().As<IEmployerCoInvestedPaymentProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<SfaCoInvestedPaymentProcessor>().As<ISfaCoInvestedPaymentProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<LevyBalanceService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ReliableCollectionCache<CalculatedRequiredLevyAmount>>()
                .AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ReliableCollectionCache<List<string>>>()
                .AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<RequiredLevyAmountFundingSourceService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SortableKeyGenerator>().AsImplementedInterfaces();
            builder.RegisterType<LevyMessageRoutingService>().AsImplementedInterfaces();

            builder.Register(c => new CoInvestedFundingSourceService
            (
                new List<ICoInvestedPaymentProcessorOld>()
                {
                    new SfaCoInvestedPaymentProcessor(c.Resolve<IValidateRequiredPaymentEvent>()),
                    new EmployerCoInvestedPaymentProcessor(c.Resolve<IValidateRequiredPaymentEvent>())
                },
                c.Resolve<ICoInvestedFundingSourcePaymentEventMapper>()
            )).As<ICoInvestedFundingSourceService>().InstancePerLifetimeScope();

            builder.RegisterServiceFabricSupport();

            builder.RegisterServiceFabricSupport();
        }
    }
}