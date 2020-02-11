using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private readonly string apiUsername;
        private readonly string apiPassword;

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
            
            apiUsername = configHelper.GetSetting("EasApiUsername");
            apiPassword = configHelper.GetSetting("EasApiPassword");
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<List<ProviderAdjustment>> GetCurrentProviderAdjustments(int academicYear)
        {
            logger.LogInfo("Getting Current Provider Adjustments - Getting Token");
            
            var token = await GetToken();
            logger.LogInfo("Token retrieved");

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/Eas/{academicYear}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            logger.LogInfo("Getting data from API");
            var httpResponse = await client.SendAsync(request).ConfigureAwait(false);

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.IsSuccessStatusCode)
            {
                logger.LogInfo("Successfully retrieved records from API");
                return JsonConvert.DeserializeObject<List<ProviderAdjustment>>(responseContent); 
            }
            
            logger.LogError($"Error getting EAS records: {responseContent}, {httpResponse}");
            throw new InvalidOperationException($"Error getting EAS records: {responseContent}");
        }

        public async Task<string> GetToken()
        {
            var body = $"{{\"userName\":\"{apiUsername}\", \"password\": \"{apiPassword}\"}}";
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(body));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var httpResponse = await client.PostAsync("api/v1/token", content);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.IsSuccessStatusCode)
            {
                return responseContent;
            }
            
            throw new InvalidOperationException($"Error getting API token: {responseContent}");
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
