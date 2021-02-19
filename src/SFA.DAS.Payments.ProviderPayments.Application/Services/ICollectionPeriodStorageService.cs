using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface ICollectionPeriodStorageService
    {
        Task StoreCollectionPeriod(short academicYear, byte period, DateTime completionDateTime);
    }
}