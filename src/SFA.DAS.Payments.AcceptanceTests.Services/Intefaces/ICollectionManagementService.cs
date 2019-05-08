namespace SFA.DAS.Payments.AcceptanceTests.Services.Intefaces
{
    using System.Threading.Tasks;

    public interface ICollectionManagementService
    {
        Task<ReturnPeriodModel> GetCurrentPeriodAsync(string collectionName);
    }
}
