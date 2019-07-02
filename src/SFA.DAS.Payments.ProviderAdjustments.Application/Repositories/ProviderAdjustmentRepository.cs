using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<List<ProviderAdjustment>> GetCurrentProviderAdjustments()
        {
            return new List<ProviderAdjustment>();
        }

        public async Task<List<ProviderAdjustment>> GetPreviousProviderAdjustments()
        {
            return new List<ProviderAdjustment>();
        }

        public Task AddProviderAdjustments(List<ProviderAdjustment> payments)
        {
            throw new System.NotImplementedException();
        }
    }
}
