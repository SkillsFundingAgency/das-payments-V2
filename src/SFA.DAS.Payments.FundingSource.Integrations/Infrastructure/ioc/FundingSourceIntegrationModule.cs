using Autofac;
using System.Collections.Generic;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Integrations.Services;


namespace SFA.DAS.Payments.FundingSource.Integrations.Infrastructure.Ioc
{
    public class FundingSourceIntegrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LevyFundingSourceIntegrationRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();

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
                    var batchSize = configHelper.GetSettingOrDefault("BatchSize",1000);
                    var repository = c.Resolve<ILevyFundingSourceIntegrationRepository>();
                    var accountApiClient = c.Resolve<IAccountApiClient>();
                    var logger = c.Resolve<IPaymentLogger>();

                    return new ManageLevyAccountBalanceService(repository, accountApiClient, logger, batchSize);
                })
                .As<IManageLevyAccountBalanceService>()
                .InstancePerLifetimeScope();
            }
    }
}