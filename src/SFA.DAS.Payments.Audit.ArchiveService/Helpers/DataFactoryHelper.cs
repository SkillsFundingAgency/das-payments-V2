using System.Threading.Tasks;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Identity.Client;
using Microsoft.Rest;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.Configuration;

namespace SFA.DAS.Payments.Audit.ArchiveService.Helpers
{
    public static class DataFactoryHelper
    {
        public static async Task<DataFactoryManagementClient> CreateClient(IPeriodEndArchiveConfiguration config)
        {
            // Authenticate and create a data factory management client
            var app = ConfidentialClientApplicationBuilder.Create(config.ApplicationId)
                .WithAuthority(config.AuthorityUri + config.TenantId)
                .WithClientSecret(config.AuthenticationKey)
                .WithLegacyCacheCompatibility(false)
                .WithCacheOptions(CacheOptions.EnableSharedCacheOptions)
                .Build();

            var result = await app.AcquireTokenForClient(
                    new[] { config.ManagementUri })
                .ExecuteAsync();
            var cred = new TokenCredentials(result.AccessToken);

            return new DataFactoryManagementClient(cred)
            {
                SubscriptionId = config.SubscriptionId
            };
        }
    }
}