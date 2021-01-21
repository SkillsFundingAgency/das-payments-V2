using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ProviderAdjustments.Domain;

namespace SFA.DAS.Payments.ProviderAdjustments.Application.Repositories
{
    public interface IProviderAdjustmentRepository
    {
        Task<List<ProviderAdjustment>> GetCurrentProviderAdjustments(int academicYear);
        Task<List<ProviderAdjustment>> GetPreviousProviderAdjustments(int academicYear);
        Task AddProviderAdjustments(List<ProviderAdjustment> payments);
    }

    public class ProviderAdjustmentRepository : IProviderAdjustmentRepository
    {
        private readonly HttpClient client;
        private readonly IPaymentsDataContext dataContext;
        private readonly IPaymentLogger logger;
        private readonly IMapper mapper;
        private readonly IBulkWriter<ProviderAdjustment> bulkWriter;
        private readonly string apiClientId;
        private readonly string apiScope;
        private readonly string apiTenantId;
        private readonly string apiPassword;

        private readonly int pageSize;

        public ProviderAdjustmentRepository(
            IBulkWriter<ProviderAdjustment> bulkWriter, 
            IPaymentsDataContext dataContext,
            IPaymentLogger logger,
            IConfigurationHelper configHelper,
            IMapper mapper)
        {
            this.bulkWriter = bulkWriter;
            this.dataContext = dataContext;
            var certificateThumbprint = configHelper.GetSetting("EasCertificateThumbprint");

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, certificate, cetChain, policyErrors) =>
                {
                    var thumbprintMatches = certificate.GetCertHashString()?.Equals(certificateThumbprint);
                    if (thumbprintMatches.HasValue && thumbprintMatches.Value)
                    {
                        return true;
                    }

                    if (policyErrors == SslPolicyErrors.None)
                    {
                        return true;
                    }

                    return false;
                };

            client = new HttpClient(handler){BaseAddress = new Uri(configHelper.GetSetting("EasApiEndpoint"))};
            
            
            apiClientId = configHelper.GetSetting("EasApiClientId");
            apiTenantId = configHelper.GetSetting("EasApiTenantId");
            apiScope = configHelper.GetSetting("EasApiScope");
            apiPassword = configHelper.GetSetting("EasApiPassword");
            

            pageSize = configHelper.GetSettingOrDefault("EasPageSize", 10000);
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<List<ProviderAdjustment>> GetCurrentProviderAdjustments(int academicYear)
        {
            logger.LogInfo("Getting Current Provider Adjustments - Getting Token");
            
            var token = await GetToken();
            logger.LogInfo("Token retrieved");

            var providerAdjustments = new List<ProviderAdjustment>();
            var pageNumber = 1;
           
            while(true)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/Eas/{academicYear}?pagenumber={pageNumber}&pagesize={pageSize}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                logger.LogInfo($"Getting page {pageNumber} of data from API");
                var httpResponse = await client.SendAsync(request).ConfigureAwait(false);

                logger.LogInfo($"Successfully connected to the API");
                
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                logger.LogDebug($"Response code: {httpResponse.StatusCode}, Reason: {httpResponse.ReasonPhrase}, " +
                                $"Request: {httpResponse.RequestMessage.RequestUri}");

                if (httpResponse.IsSuccessStatusCode)
                {
                    var batch = JsonConvert.DeserializeObject<List<ProviderAdjustment>>(responseContent);
                    if (batch.Count == 0)
                    {
                        logger.LogInfo($"No messages on page {pageNumber}");
                        break;
                    }
                    logger.LogInfo($"Successfully retrieved {batch.Count} records from API");
                    providerAdjustments.AddRange(batch);
                }
                else
                {
                    logger.LogError($"Error getting EAS records: {responseContent}, {httpResponse}");
                    throw new InvalidOperationException($"Error getting EAS records: {responseContent}");
                }

                pageNumber++;
            }

            logger.LogInfo($"Finished reading records from the API. Got {providerAdjustments.Count} records");
            return providerAdjustments;
        }

        private async Task<string> GetToken()
        {
            var authContext = new AuthenticationContext($"https://login.microsoftonline.com/{apiTenantId}");
            var clientCredential = new ClientCredential(apiClientId, apiPassword);

            var authResult = await authContext.AcquireTokenAsync(apiScope, clientCredential);

            if (authResult == null)
            {
                throw new AuthenticationException("Could not authenticate with the OAUTH2 claims provider after several attempts");
            }

            return authResult.AccessToken;
        }

        public async Task<List<ProviderAdjustment>> GetPreviousProviderAdjustments(int academicYear)
        {
            var databaseResults = await dataContext
                .ProviderAdjustments
                .Where(x => x.SubmissionAcademicYear == academicYear)
                .ToListAsync();
            var results = mapper.Map<List<ProviderAdjustment>>(databaseResults);
            return results;
        }

        public async Task AddProviderAdjustments(List<ProviderAdjustment> payments)
        {
            foreach (var providerAdjustment in payments)
            {
                await bulkWriter.Write(providerAdjustment, default(CancellationToken));
            }
            
            await bulkWriter.Flush(default(CancellationToken)).ConfigureAwait(false);
        }
    }
}
