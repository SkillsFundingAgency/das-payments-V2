using Autofac;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using System.Collections.Generic;
using Autofac.Integration.ServiceFabric;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
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
            builder.RegisterType<LevyFundingSourceRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<PaymentProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<LevyPaymentProcessor>().As<ILevyPaymentProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<TransferPaymentProcessor>().As<ITransferPaymentProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<CoInvestedPaymentProcessor>().As<ICoInvestedPaymentProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<EmployerCoInvestedPaymentProcessor>().As<IEmployerCoInvestedPaymentProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<SfaCoInvestedPaymentProcessor>().As<ISfaCoInvestedPaymentProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<LevyBalanceService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ReliableCollectionCache<CalculatedRequiredLevyAmount>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ReliableCollectionCache<List<string>>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<RequiredLevyAmountFundingSourceService>().AsImplementedInterfaces().InstancePerLifetimeScope();
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


            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    var accountApiConfig = new AccountApiConfiguration
                    {
                        ApiBaseUrl = configHelper.GetSetting("AccountApiBaseUrl"),
                        ClientId = configHelper.GetSetting("AccountApiClientId"),
                        ClientSecret = configHelper.GetSetting("AccountApiClientSecret"),
                        IdentifierUri = configHelper.GetSetting("AccountApiIdentifierUri"),
                        Tenant = configHelper.GetSetting("AccountApiTenant")
                    };

                    return accountApiConfig;
                })
                .As<IAccountApiConfiguration>()
                .InstancePerLifetimeScope();

            builder.RegisterType<AccountApiClient>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    var batchSize = configHelper.GetSettingOrDefault("BatchSize", 1000);
                    var repository = c.Resolve<ILevyFundingSourceRepository>();
                    var accountApiClient = c.Resolve<IAccountApiClient>();
                    var logger = c.Resolve<IPaymentLogger>();

                    return new ManageLevyAccountBalanceService(repository, accountApiClient, logger, batchSize);
                })
                .As<IManageLevyAccountBalanceService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ProcessLevyAccountBalanceService>().AsImplementedInterfaces().InstancePerLifetimeScope();



            builder.RegisterServiceFabricSupport();
        }
    }
}