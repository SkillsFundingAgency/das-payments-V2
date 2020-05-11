using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IEmployerProviderPriorityStorageService
    {
        Task StoreEmployerProviderPriority(EmployerChangedProviderPriority providerPriorityEvent);
    }
}