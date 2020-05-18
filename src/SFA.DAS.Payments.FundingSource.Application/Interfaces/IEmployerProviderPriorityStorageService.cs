using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IEmployerProviderPriorityStorageService
    {
        Task StoreEmployerProviderPriority(EmployerChangedProviderPriority providerPriorityEvent);
    }
}