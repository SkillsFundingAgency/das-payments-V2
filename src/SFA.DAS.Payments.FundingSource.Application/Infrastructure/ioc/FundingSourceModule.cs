using Autofac;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using Autofac.Integration.ServiceFabric;
using NServiceBus;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
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
            builder.RegisterType<PeriodEndService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<LevyAccountBulkCopyConfiguration>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<LevyAccountBulkCopyRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<FundingSourceEventGenerationService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SubmissionCleanUpService>().AsImplementedInterfaces().InstancePerLifetimeScope();

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
                    var accountApiClient = c.Resolve<IAccountApiClient>();
                    var logger = c.Resolve<IPaymentLogger>();
                    var bulkWriter = c.Resolve<ILevyAccountBulkCopyRepository>();
                    var endpointInstanceFactory = new EndpointInstanceFactory(CreateEndpointConfiguration(c));

                    return new ManageLevyAccountBalanceService(accountApiClient, logger, bulkWriter, batchSize, endpointInstanceFactory);
                })
                .As<IManageLevyAccountBalanceService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ProcessLevyAccountBalanceService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<LevyTransactionBatchStorageService>().AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.Register((c, p) =>
            {
                var config = c.Resolve<IConfigurationHelper>();
                return new FundingSourceDataContext(config.GetConnectionString("PaymentsConnectionString"));
            }).As<IFundingSourceDataContext>();

            builder.RegisterServiceFabricSupport();
        }


        private EndpointConfiguration CreateEndpointConfiguration(IComponentContext container)
        {
            var config = container.Resolve<IApplicationConfiguration>();
           
            var endpointConfiguration = new EndpointConfiguration(config.EndpointName);

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningMessagesAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false));
            conventions.DefiningCommandsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages.Commands") ?? false));
            conventions.DefiningEventsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages.Events") ?? false));

            var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
            persistence.ConnectionString(config.StorageConnectionString);

            endpointConfiguration.DisableFeature<NServiceBus.Features.TimeoutManager>();
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(config.ServiceBusConnectionString)
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .RuleNameShortener(ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);

            EndpointConfigurationEvents.OnConfiguringTransport(transport);
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.EnableCallbacks(makesRequests: false);
            return endpointConfiguration;

        }

    }
}