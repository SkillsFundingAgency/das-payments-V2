namespace SFA.DAS.Payments.AcceptanceTests.Services
{
    using ESFA.DC.CollectionsManagement.Models;
    using ESFA.DC.Serialization.Interfaces;
    using SFA.DAS.Payments.AcceptanceTests.Services.BespokeHttpClient;
    using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;
    using System.Threading.Tasks;

    public class CollectionManagementService : ICollectionManagementService
    {
        private readonly IBespokeHttpClient _httpClient;
        private readonly IJsonSerializationService _serializationService;

        public CollectionManagementService(IBespokeHttpClient httpClient,
            IJsonSerializationService serializationService)
        {
            _httpClient = httpClient;
            _serializationService = serializationService;
        }

        public async Task<ReturnPeriodModel> GetCurrentPeriodAsync(string collectionName)
        {
            var data = await _httpClient.GetDataAsync($"returns-calendar/{collectionName}/current");
            ReturnPeriodModel result = null;

            if (!string.IsNullOrEmpty(data))
            {
                var returnPeriod = _serializationService.Deserialize<ReturnPeriod>(data);
                result = new ReturnPeriodModel(returnPeriod.PeriodNumber);
            }

            return result;
        }
    }
}
