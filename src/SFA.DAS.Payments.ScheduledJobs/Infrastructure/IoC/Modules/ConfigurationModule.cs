using Autofac;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration;

namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC.Modules
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new ScheduledJobsConfiguration
                    {
                        EndpointName = configHelper.GetSetting("EndpointName"),
                        ServiceBusConnectionString = configHelper.GetConnectionString("ServiceBusConnectionString"),
                        DasNServiceBusLicenseKey = configHelper.GetSetting("DasNServiceBusLicenseKey"),
                        LevyAccountBalanceEndpoint = configHelper.GetSetting("LevyAccountBalanceEndpoint"),
                        EarningAuditDataCleanUpQueue = configHelper.GetSetting("EarningAuditDataCleanUpQueue"),
                        DataLockAuditDataCleanUpQueue = configHelper.GetSetting("DataLockAuditDataCleanUpQueue"),
                        FundingSourceAuditDataCleanUpQueue = configHelper.GetSetting("FundingSourceAuditDataCleanUpQueue"),
                        RequiredPaymentAuditDataCleanUpQueue = configHelper.GetSetting("RequiredPaymentAuditDataCleanUpQueue"),
                        CurrentCollectionPeriod = configHelper.GetSetting("CurrentCollectionPeriod"),
                        CurrentAcademicYear = configHelper.GetSetting("CurrentAcademicYear"),
                        PreviousCollectionPeriod = configHelper.GetSetting("PreviousCollectionPeriod"),
                        PreviousAcademicYear = configHelper.GetSetting("PreviousAcademicYear"),
                        AccountApiBatchSize = configHelper.GetSettingOrDefault("AccountApiBatchSize", 1000),
                    };
                })
                .As<IScheduledJobsConfiguration>()
                .SingleInstance();
            
            builder.Register((c, p) =>
                   {
                       var configHelper = c.Resolve<IConfigurationHelper>();

                       return new AccountApiConfiguration
                       {
                           ApiBaseUrl = configHelper.GetSetting("AccountApiBaseUrl"),
                           ClientId = configHelper.GetSetting("AccountApiClientId"),
                           ClientSecret = configHelper.GetSetting("AccountApiClientSecret"),
                           IdentifierUri = configHelper.GetSetting("AccountApiIdentifierUri"),
                           Tenant = configHelper.GetSetting("AccountApiTenant")
                       };
                   })
                   .As<IAccountApiConfiguration>()
                   .SingleInstance();
        }
    }
}