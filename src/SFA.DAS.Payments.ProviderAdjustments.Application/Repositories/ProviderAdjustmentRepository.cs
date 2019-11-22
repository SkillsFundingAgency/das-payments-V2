using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog.Enrichers;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ProviderAdjustments.Domain;

namespace SFA.DAS.Payments.ProviderAdjustments.Application.Repositories
{
    public interface IProviderAdjustmentRepository
    {
        Task<List<ProviderAdjustment>> GetCurrentProviderAdjustments();
        Task<List<ProviderAdjustment>> GetPreviousProviderAdjustments();
        Task AddProviderAdjustments(List<ProviderAdjustment> payments);
    }

    public class ProviderAdjustmentRepository : IProviderAdjustmentRepository
    {
        private readonly HttpClient client;
        private readonly IPaymentsDataContext dataContext;
        private readonly IMapper mapper;
        private readonly IBulkWriter<ProviderAdjustment> bulkWriter;
        private readonly string apiUsername;
        private readonly string apiPassword;
       
        public ProviderAdjustmentRepository(
            IBulkWriter<ProviderAdjustment> bulkWriter, 
            IPaymentsDataContext dataContext, 
            IConfigurationHelper configHelper,
            IMapper mapper)
        {
            this.bulkWriter = bulkWriter;
            this.dataContext = dataContext;
            client = new HttpClient{BaseAddress = new Uri(configHelper.GetSetting("EasApiEndpoint"))};
            apiUsername = configHelper.GetSetting("EasApiUsername");
            apiPassword = configHelper.GetSetting("EasApiPassword");
            this.mapper = mapper;
        }

        public async Task<List<ProviderAdjustment>> GetCurrentProviderAdjustments()
        {
            var token = await GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var httpResponse = await client.GetStringAsync("api/v1/Eas/1920").ConfigureAwait(false);
            var results = JsonConvert.DeserializeObject<List<ProviderAdjustment>>(httpResponse);
            return results;
        }

        public async Task<string> GetToken()
        {
            var body = $"{{\"userName\":\"{apiUsername}\", \"apiPassword\": \"{apiPassword}\"}}";
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(body));

            var httpResponse = await client.PostAsync($"api/v1/token", content);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.IsSuccessStatusCode)
            {
                return responseContent;
            }
            
            throw new InvalidOperationException($"Error getting API token: {responseContent}");
        }

        public async Task<List<ProviderAdjustment>> GetPreviousProviderAdjustments()
        {
            var results = mapper.Map<List<ProviderAdjustment>>(await dataContext.ProviderAdjustments.ToListAsync());
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
