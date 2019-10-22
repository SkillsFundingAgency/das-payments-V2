using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Repositories;
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
        private readonly string easApiEndpoint;

        public ProviderAdjustmentRepository(
            IBulkWriter<ProviderAdjustment> bulkWriter, 
            IPaymentsDataContext dataContext, 
            HttpClient client, 
            IMapper mapper)
        {
            this.bulkWriter = bulkWriter;
            this.dataContext = dataContext;
            this.client = client;
            this.mapper = mapper;
            easApiEndpoint = "";
        }

        public async Task<List<ProviderAdjustment>> GetCurrentProviderAdjustments()
        {
            var httpResponse = await client.GetStringAsync("requesturi").ConfigureAwait(false);
            var results = JsonConvert.DeserializeObject<List<ProviderAdjustment>>(httpResponse);
            return results;
        }

        public async Task<List<ProviderAdjustment>> GetPreviousProviderAdjustments()
        {
            var results = mapper.Map<List<ProviderAdjustment>>(dataContext.ProviderAdjustments);
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
