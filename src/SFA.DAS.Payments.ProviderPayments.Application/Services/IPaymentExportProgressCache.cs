using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IPaymentExportProgressCache
    {
        Task<int> GetPage(short academicYear, byte collectionPeriod);
        Task<int> IncrementPage(short academicYear, byte collectionPeriod);
    }
}
