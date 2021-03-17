using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface ICollectionPeriodStorageService
    {
        Task StoreCollectionPeriod(PeriodEndStoppedEvent message);
    }
}